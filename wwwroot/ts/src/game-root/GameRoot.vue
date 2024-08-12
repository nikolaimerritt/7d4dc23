<template>
    <div class="horizontal-container">
        <div class="left-container">
            <div class="menu-bar">
                <div class="menu-section">
                    <text-button
                        v-if="balance !== undefined && balance > 0"
                        :text="'Purchase ships'"
                        @buttonClick="onPurchaseShipsClick()"
                    ></text-button>
                    <text-button
                        v-if="canMove"
                        :text="'Move ships'"
                        @buttonClick="onMoveShipsClick()"
                    >
                    </text-button>
                </div>
                <div class="menu-section">
                    <text-pill :text="`${this.balance} coins`"></text-pill>
                    <text-pill
                        v-if="
                            this.ui.round.timeRemaining !== undefined &&
                            this.ui.round.timeRemaining.seconds() > 0
                        "
                        :text="`Round ends in ${this.ui.round.timeRemaining.format(
                            'HH:mm:ss'
                        )}`"
                    ></text-pill>
                    <text-pill
                        v-else-if="
                            this.ui.round.timeRemaining !== undefined &&
                            this.ui.round.timeRemaining.seconds() === 0
                        "
                        :text="'Round has ended'"
                    ></text-pill>
                </div>
            </div>
            <div class="dialog" v-if="dialogText()">
                {{ dialogText() }}
            </div>
            <div class="map-container">
                <img
                    :src="'/imgs/seas/map-sepia-2.png'"
                    class="map-background"
                    ref="mapBackground"
                />
                <sea-centre
                    v-for="(seaCentre, index) in this.seaCentres"
                    :key="index"
                    :name="seaCentre.name"
                    :teamShips="seaCentre.teamShips"
                    :highlighted="isHighlighted(seaCentre)"
                    class="sea-centre"
                    @sea-centre-click="onSeaCentreClick(seaCentre)"
                    :style="{
                        transformOrigin: 'top left',
                        transform: `scale(${seaCentreScale})`,
                        top: `${100 * seaCentrePositions[seaCentre.name].top}%`,
                        left: `${
                            100 * seaCentrePositions[seaCentre.name].left
                        }%`,
                    }"
                >
                </sea-centre>
            </div>
        </div>
        <input-modal
            v-if="ui.purchase.showModal"
            :message="'How many points would you like to spend to purchase new ships?'"
            :buttonText="'Purchase'"
            :errorMessage="ui.purchase.error"
            @submission="onSubmitPurchase($event)"
            @clickOutside="resetActions()"
        ></input-modal>
        <input-modal
            v-if="ui.move.showModal"
            :message="'How many ships would you like to move?'"
            :buttonText="'Move'"
            :errorMessage="ui.move.error"
            @submission="onSubmitMove($event)"
            @clickOutside="resetActions()"
        >
        </input-modal>
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "../endpoints/team";
import { PurchaseEndpoint } from "../endpoints/purchase";
import { Sea, SeaEndpoint } from "../endpoints/sea";
import { OutcomeEndpoint } from "../endpoints/outcome";
import { MoveEndpoint } from "../endpoints/move";
import { Connection } from "../endpoints/main";
import { Round, RoundEndpoint } from "../endpoints/round";
import { Util, VueThis } from "../common/util";
import * as moment from "moment";

const updateTimeRemainingMs = 2_000;
const updateMapMs = 5_000;
type Action = "none" | "purchase" | "move";
type TeamShips = { team: Team; shipCount: number };
type SeaCentre = Sea & { teamShips: TeamShips[] };

type SeaCentrePositions = {
    [seaName: string]: { top: number; left: number };
};

interface Data {
    endpoints: {
        team: TeamEndpoint;
        purchase: PurchaseEndpoint;
        sea: SeaEndpoint;
        outcome: OutcomeEndpoint;
        move: MoveEndpoint;
        round: RoundEndpoint;
    };
    ui: {
        action: Action;
        purchase: {
            showModal: boolean;
            seaToPurchaseIn?: Sea;
            error: string;
        };
        move: {
            showModal: boolean;
            seaToMoveFrom?: Sea;
            seaToMoveTo?: Sea;
            error: string;
        };
        round: {
            timeRemaining?: moment.Moment;
            updateTimeRemainingHandle?: number;
        };
        map: {
            updateMapHandle: number;
        };
    };
    team?: Team;
    balance?: number;
    seaCentres: SeaCentre[];
    accessibleSeas: Sea[];
    canMove: boolean;
    round?: Round;
    seaCentrePositions: SeaCentrePositions;
    seaCentreScale: number | undefined;
}

type This = VueThis<Data>;

export default {
    data(): Data {
        return {
            team: undefined,
            balance: undefined,
            accessibleSeas: [],
            canMove: false,
            round: undefined,
            endpoints: {
                team: new TeamEndpoint(),
                purchase: new PurchaseEndpoint(),
                sea: new SeaEndpoint(),
                outcome: new OutcomeEndpoint(),
                move: new MoveEndpoint(),
                round: new RoundEndpoint(),
            },
            seaCentres: [],
            ui: {
                action: "none",
                purchase: {
                    showModal: false,
                    seaToPurchaseIn: undefined,
                    error: "",
                },
                move: {
                    showModal: false,
                    seaToMoveFrom: undefined,
                    seaToMoveTo: undefined,
                    error: "",
                },
                round: {
                    timeRemaining: undefined,
                    updateTimeRemainingHandle: undefined,
                },
                map: {
                    updateMapHandle: undefined,
                },
            },
            seaCentrePositions: {
                "North Pacific": {
                    top: 0.213,
                    left: 0,
                },
                "South Pacific": {
                    top: 0.515,
                    left: 0,
                },
                "North Atlantic": {
                    top: 0.211,
                    left: 0.115,
                },
                "South Atlantic": {
                    top: 0.53,
                    left: 0.223,
                },
                Southern: {
                    top: 0.862,
                    left: 0,
                },
                Indian: {
                    top: 0.393,
                    left: 0.47,
                },
                Arctic: {
                    top: 0,
                    left: 0,
                },
            },
            seaCentreScale: undefined,
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        await this.refreshMap();
        this.updateTimeRemaining();
        this.transformSeaCentres();
        this.ui.round.updateTimeRemainingHandle = window.setInterval(
            () => this.updateTimeRemaining(),
            updateTimeRemainingMs
        );
        this.ui.map.updateMapHandle = window.setInterval(
            async () => await this.refreshMap(),
            updateMapMs
        );
        window.addEventListener("resize", () => this.transformSeaCentres());
    },
    methods: {
        async refreshMap(this: This) {
            this.balance = await this.endpoints.purchase.getBalance();
            this.seaCentres = await this.getSeaCentres();
            this.accessibleSeas = await this.endpoints.sea.getAccessibleSeas();
            this.canMove = await this.endpoints.move.canMove();
            this.round = await this.endpoints.round.getCurrentRound();
        },
        async getSeaCentres(this: This): Promise<SeaCentre[]> {
            const latestOutcomes =
                await this.endpoints.outcome.getLatestOutcomes();
            const seas = await this.endpoints.sea.getAllSeas();
            const seaCentres: SeaCentre[] = [];
            for (const sea of seas) {
                const outcomesInSea = latestOutcomes.filter(
                    (outcome) =>
                        outcome.sea.id == sea.id && outcome.shipsAfter > 0
                );
                const seaCentre: SeaCentre = {
                    ...sea,
                    teamShips: outcomesInSea.map((outcome) => ({
                        team: outcome.team,
                        shipCount: outcome.shipsAfter,
                    })),
                };
                seaCentres.push(seaCentre);
            }
            return seaCentres;
        },
        transformSeaCentres(this: This) {
            const mapBackground = this.$refs.mapBackground as HTMLImageElement;
            this.seaCentreScale =
                mapBackground.width / mapBackground.naturalWidth;
            console.log(
                "transformSeaCentres",
                mapBackground,
                mapBackground.width,
                mapBackground.naturalWidth
            );
        },
        onSeaAreaHover(this: This, seaCentre: SeaCentre) {
            console.log("Sea area hovered over", seaCentre);
        },
        onPurchaseShipsClick(this: This) {
            if (this.ui.action === "none") {
                this.ui.action = "purchase";
            } else if (this.ui.action === "purchase") {
                this.resetActions();
            }
        },
        resetActions(this: This) {
            this.ui.action = "none";

            this.ui.purchase.showModal = false;
            this.ui.purchase.seaToPurchaseIn = undefined;
            this.ui.purchase.error = "";

            this.ui.move.showModal = false;
            this.ui.move.error = "";
            this.ui.move.seaToMoveFrom = undefined;
            this.ui.move.seaToMoveTo = undefined;
            this.ui.move.showModal = false;
        },
        onMoveShipsClick(this: This) {
            if (this.ui.action === "none") {
                this.ui.action = "move";
            } else if (this.ui.action === "move") {
                this.resetActions();
            }
        },
        onSeaCentreClick(this: This, seaCentre: SeaCentre) {
            if (this.ui.action === "none") {
                return;
            }
            if (this.ui.action === "purchase") {
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
        async onSubmitPurchase(this: This, pointsToSpend: string) {
            if (this.ui.purchase.showModal) {
                const points = parseInt(pointsToSpend);
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
                        this.resetActions();
                        await this.refreshMap();
                    }
                }
            }
        },
        async onSubmitMove(this: This, shipsToMove: string) {
            if (this.ui.move.showModal) {
                const ships = parseInt(shipsToMove);
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
                        this.resetActions();
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
                this.round.startCooldown,
                new Date()
            );
        },
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.round.updateTimeRemainingHandle);
        window.clearInterval(this.ui.map.updateMapHandle);
        window.removeEventListener("resize", () => this.transformSeaCentres());
    },
};
</script>
<style scoped>
.dialog {
    padding: 0 0 12px 12px;
}
.horizontal-container {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: center;
}
.left-container {
    width: 60%;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
}

.menu-bar {
    width: 100%;
    padding: 24px 20px 0px 20px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    column-gap: 32px;
    margin-bottom: 8px;
}

.menu-section {
    display: flex;
    flex-direction: row;
    gap: 40px;
}

.map-background {
    width: 100%;
    border-radius: 16px;
}

.map-container {
    position: relative;
    display: inline-block;
    border-radius: 16px;
    border: 4px solid #b18854;
    background: #e7daa1;
}

.sea-centre {
    position: absolute;
    z-index: 10;
}
</style>
