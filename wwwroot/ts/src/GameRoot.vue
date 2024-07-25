<template>
    <div>
        <div class="menu-bar">
            <span> {{ this.team?.name }} </span>
            <span> {{ this.balance }} Coins </span>
            <span
                v-if="balance > 0"
                style="border-radius: 4px; background-color: beige"
                v-on:click="onPurchaseShipsClick()"
            >
                Purchase ships
            </span>
            <span
                v-if="canMove"
                style="border-radius: 4px; background-color: beige"
                v-on:click="onMoveShipsClick()"
            >
                Move ships
            </span>
            <span v-if="this.ui.round.timeRemaining">
                {{ "Round ends in " + this.ui.round.timeRemaining }}
            </span>
            <span v-if="dialogText()"> {{ dialogText() }} </span>
        </div>
        <div class="map-container">
            <sea-centre
                v-for="(seaCentre, index) in this.seaCentres"
                :key="index"
                :name="seaCentre.name"
                :teamShips="seaCentre.teamShips"
                :highlighted="isHighlighted(seaCentre)"
                class="sea-centre"
                v-on:sea-centre-click="onSeaCentreClick(seaCentre)"
                :style="{
                    left: `${seaCentre.xCoord}%`,
                    top: `${seaCentre.yCoord}%`,
                }"
            >
            </sea-centre>
            <img
                :src="'../../imgs/map-cropped.jpg'"
                class="map-background"
                ref="mapBackground"
            />
        </div>
        <div v-if="ui.purchase.showModal" class="modal-wrapper">
            <div class="modal-box">
                <span>
                    How many points would you like to spend to buy new ships?
                </span>
                <input v-model="ui.purchase.pointsToSpendOnShips" />
                <button v-on:click="onSubmitPurchase()">Points to spend</button>
                <span v-if="ui.purchase.error">
                    {{ ui.purchase.error }}
                </span>
            </div>
        </div>
        <div v-if="ui.move.showModal" class="modal-wrapper">
            <div class="modal-box">
                <span> How many ships would you like to move? </span>
                <input v-model="ui.move.shipsToMove" />
                <button v-on:click="onSubmitMove()">Ships to move</button>
                <span v-if="ui.move.error">
                    {{ ui.move.error }}
                </span>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "./endpoints/team";
import { PurchaseEndpoint } from "./endpoints/purchase";
import { Sea, SeaEndpoint } from "./endpoints/sea";
import { OutcomeEndpoint, Outcome } from "./endpoints/outcome";
import { MoveEndpoint } from "./endpoints/move";
import { Connection } from "./endpoints/main";
import { LeaderboardEndpoint, LeaderboardEntry } from "./endpoints/leaderboard";
import { Round, RoundEndpoint } from "./endpoints/round";
import { Util } from "./util";
import { onUnmounted } from "vue";

const updateTimeRemainingMs = 2_000;
type Action = "none" | "purchase" | "move";
type TeamShips = { team: Team; shipCount: number };
type SeaCentre = Sea & {
    teamShips: TeamShips[];
    xCoord: number;
    yCoord: number;
};

interface Data {
    endpoints: {
        team: TeamEndpoint;
        purchase: PurchaseEndpoint;
        sea: SeaEndpoint;
        outcome: OutcomeEndpoint;
        move: MoveEndpoint;
        leaderboard: LeaderboardEndpoint;
        round: RoundEndpoint;
    };
    ui: {
        action: Action;
        purchase: {
            showModal: boolean;
            pointsToSpendOnShips: string;
            seaToPurchaseIn?: Sea;
            error: string;
        };
        move: {
            showModal: boolean;
            seaToMoveFrom?: Sea;
            seaToMoveTo?: Sea;
            shipsToMove: string;
            error: string;
        };
        round: {
            timeRemaining: string;
            updateTimeRemainingHandle?: number;
        };
    };
    team?: Team;
    balance?: number;
    seaCentres: SeaCentre[];
    accessibleSeas: Sea[];
    canMove: boolean;
    leaderboard: LeaderboardEntry[];
    round?: Round;
}

type This = Data & { [functionName: string]: Function } & { $refs: any };

const normalisedSeaCoords = {
    "North Pacific": [0.937, 0.538],
    "South Pacific": [0.157, 0.73],
    "North Atlantic": [0.358, 0.518],
    "South Atlantic": [0.444, 0.809],
    Southern: [0.628, 0.919],
    Indian: [0.694, 0.747],
    Arctic: [0.527, 0.153],
};

export default {
    data(): Data {
        return {
            team: undefined,
            balance: undefined,
            accessibleSeas: [],
            canMove: false,
            leaderboard: [],
            round: undefined,
            endpoints: {
                team: new TeamEndpoint(),
                purchase: new PurchaseEndpoint(),
                sea: new SeaEndpoint(),
                outcome: new OutcomeEndpoint(),
                move: new MoveEndpoint(),
                leaderboard: new LeaderboardEndpoint(),
                round: new RoundEndpoint(),
            },
            seaCentres: [],
            ui: {
                action: "none",
                purchase: {
                    showModal: false,
                    pointsToSpendOnShips: "",
                    seaToPurchaseIn: undefined,
                    error: "",
                },
                move: {
                    showModal: false,
                    seaToMoveFrom: undefined,
                    seaToMoveTo: undefined,
                    shipsToMove: "",
                    error: "",
                },
                round: {
                    timeRemaining: "",
                    updateTimeRemainingHandle: undefined,
                },
            },
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        await this.refreshMap();
        this.updateTimeRemaining();
        this.ui.round.updateTimeRemainingHandle = window.setInterval(
            () => this.updateTimeRemaining(),
            updateTimeRemainingMs
        );
    },
    methods: {
        async refreshMap(this: This) {
            this.balance = await this.endpoints.purchase.getBalance();
            this.seaCentres = await this.getSeaCentres();
            this.accessibleSeas = await this.endpoints.sea.getAccessibleSeas();
            this.canMove = await this.endpoints.move.canMove();
            this.round = await this.endpoints.round.getCurrentRound();
            this.leaderboard =
                await this.endpoints.leaderboard.getLeaderboard();
            console.log("Rounds", this.round);
            console.log("Leaderboard", this.leaderboard);
        },
        async getSeaCentres(this: This): Promise<SeaCentre[]> {
            const latestOutcomes =
                await this.endpoints.outcome.getLatestOutcomes();
            const seas = await this.endpoints.sea.getAllSeas();
            const seaCentres: SeaCentre[] = [];
            for (const sea of seas) {
                const outcomesInSea = latestOutcomes.filter(
                    (outcome) =>
                        outcome.sea.id == sea.id && outcome.shipCount > 0
                );
                const seaCentre: SeaCentre = {
                    xCoord: 100 * normalisedSeaCoords[sea.name][0],
                    yCoord: 100 * normalisedSeaCoords[sea.name][1],
                    ...sea,
                    teamShips: outcomesInSea,
                };
                seaCentres.push(seaCentre);
            }
            return seaCentres;
        },
        onPurchaseShipsClick(this: This) {
            if (this.ui.action === "none") {
                this.ui.action = "purchase";
            } else if (this.ui.action === "purchase") {
                this.ui.action = "none";
            }
        },
        onMoveShipsClick(this: This) {
            if (this.ui.action === "none") {
                this.ui.action = "move";
            } else if (this.ui.action === "move") {
                this.ui.action = "none";
            }
        },
        onSeaCentreClick(this: This, seaCentre: SeaCentre) {
            if (this.ui.action === "none") {
                return;
            }
            if (this.ui.action === "purchase") {
                this.ui.purchase.pointsToSpendOnShips = "";
                this.ui.purchase.seaToPurchaseIn = seaCentre;
                this.ui.purchase.showModal = true;
            } else if (this.ui.action === "move") {
                if (this.ui.move.seaToMoveFrom === undefined) {
                    this.ui.move.seaToMoveFrom = seaCentre;
                } else if (this.ui.move.seaToMoveTo === undefined) {
                    this.ui.move.seaToMoveTo = seaCentre;
                    this.ui.move.showModal = true;
                }
            }
        },
        async onSubmitPurchase(this: This) {
            if (this.ui.purchase.showModal) {
                const points = parseInt(this.ui.purchase.pointsToSpendOnShips);
                if (isNaN(points)) {
                    this.ui.purchase.error =
                        "Please choose a valid number of points.";
                } else {
                    const result = await this.endpoints.purchase.purchaseShips(
                        this.ui.purchase.seaToPurchaseIn,
                        points
                    );
                    if (Connection.isError(result)) {
                        this.ui.purchase.error = result.error;
                    } else {
                        this.ui.purchase.showModal = false;
                        this.ui.action = "none";
                        this.ui.purchase.showModal = false;
                        this.ui.purchase.pointsToSpendOnShips = "";
                        this.ui.purchase.seaToPurchaseIn = undefined;
                        this.ui.purchase.error = "";
                        await this.refreshMap();
                    }
                }
            }
        },
        async onSubmitMove(this: This) {
            if (this.ui.move.showModal) {
                const ships = parseInt(this.ui.move.shipsToMove);
                if (isNaN(ships)) {
                    this.ui.move.error =
                        "Please choose a valid number of ships to move.";
                } else {
                    const result = await this.endpoints.move.moveShips(
                        this.ui.move.seaToMoveFrom,
                        this.ui.move.seaToMoveTo,
                        ships
                    );
                    if (Connection.isError(result)) {
                        this.ui.move.error = result.error;
                    } else {
                        this.ui.move.showModal = false;
                        this.ui.action = "none";
                        this.ui.move.seaToMoveFrom = undefined;
                        this.ui.move.seaToMoveTo = undefined;
                        this.ui.move.shipsToMove = "";
                        this.ui.move.error = "";
                        await this.refreshMap();
                    }
                }
            }
        },
        isHighlighted(this: This, sea: Sea) {
            if (this.ui.action === "purchase") {
                return (
                    this.ui.purchase.seaToPurchaseIn === undefined &&
                    this.accessibleSeas.some(
                        (accessibleSea) => sea.id === accessibleSea.id
                    )
                );
            } else if (this.ui.action === "move") {
                if (this.ui.move.seaToMoveFrom === undefined) {
                    return this.seaCentres.some(
                        (seaCentre) =>
                            seaCentre.id === sea.id &&
                            seaCentre.teamShips.some(
                                (teamShip) =>
                                    teamShip.team.id === this.team.id &&
                                    teamShip.shipCount > 0
                            )
                    );
                } else if (this.ui.move.seaToMoveTo === undefined) {
                    return (
                        sea.id !== this.ui.move.seaToMoveFrom.id &&
                        this.ui.move.seaToMoveFrom.adjacentSeas.some(
                            (adjacentSea) => adjacentSea.id === sea.id
                        )
                    );
                } else {
                    return false;
                }
            } else {
                return false;
            }
        },
        dialogText(this: This): string {
            if (this.ui.action === "move") {
                if (this.ui.move.seaToMoveFrom === undefined) {
                    return "Select a sea to move ships from.";
                } else if (this.ui.move.seaToMoveTo === undefined) {
                    return "Select a sea to move ships to.";
                }
            } else if (this.ui.action === "purchase") {
                if (this.ui.purchase.seaToPurchaseIn === undefined) {
                    return "Select a sea to purchase ships in.";
                }
            }
            return "";
        },
        updateTimeRemaining(this: This) {
            this.ui.round.timeRemaining = Util.timeBetween(
                this.round.startFighting,
                new Date()
            );
        },
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.round.updateTimeRemainingHandle);
    },
};
</script>
<style scoped>
.menu-bar {
    display: flex;
    column-gap: 32px;
}

.map-background {
    width: 100%;
}

.map-container {
    position: relative;
    display: inline-block;
}

.sea-centre {
    position: absolute;
}

.modal-wrapper {
    position: fixed;
    z-index: 9999;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    display: table-cell;
    vertical-align: middle;
}

.modal-box {
    position: relative;
    top: 50%;
    left: 50%;
    width: 300px;
    height: 200px;
    margin-left: -150px;
    margin-right: -100px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background-color: white;
}
</style>
