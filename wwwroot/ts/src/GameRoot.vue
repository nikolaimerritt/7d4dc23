<template>
    <div>
        <div class="menu-bar">
            <span> {{ this.team?.name }} </span>
            <span> {{ this.balance }} Coins </span>
            <span
                style="border-radius: 4px; background-color: beige"
                v-on:click="onPurchaseShipsClick()"
            >
                Purchase ships
            </span>
            <span
                style="border-radius: 4px; background-color: beige"
                v-on:click="onMoveShipsClick()"
            >
                Move ships
            </span>
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
                <input v-model="ui.purchase.pointsToSpendOnShips" />
                <button v-on:click="onSubmitPurchase()">Points to spend</button>
            </div>
        </div>
        <div v-if="ui.move.showModal" class="modal-wrapper">
            <div class="modal-box">
                <input v-model="ui.move.shipsToMove" />
                <button v-on:click="onSubmitMove()">Ships to move</button>
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
    };
    ui: {
        action: Action;
        purchase: {
            showModal: boolean;
            pointsToSpendOnShips: string;
            seaToPurchaseIn?: Sea;
        };
        move: {
            showModal: boolean;
            seaToMoveFrom?: Sea;
            seaToMoveTo?: Sea;
            shipsToMove: string;
        };
    };
    team?: Team;
    balance?: number;
    seaCentres: SeaCentre[];
    accessibleSeas: Sea[];
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
            endpoints: {
                team: new TeamEndpoint(),
                purchase: new PurchaseEndpoint(),
                sea: new SeaEndpoint(),
                outcome: new OutcomeEndpoint(),
                move: new MoveEndpoint(),
            },
            seaCentres: [],
            ui: {
                action: "none",
                purchase: {
                    showModal: false,
                    pointsToSpendOnShips: "",
                    seaToPurchaseIn: undefined,
                },
                move: {
                    showModal: false,
                    seaToMoveFrom: undefined,
                    seaToMoveTo: undefined,
                    shipsToMove: "",
                },
            },
            accessibleSeas: [],
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        await this.refreshMap();
    },
    methods: {
        async refreshMap(this: This) {
            this.balance = await this.endpoints.purchase.getBalance();
            this.seaCentres = await this.getSeaCentres();
            this.accessibleSeas = await this.endpoints.sea.getAccessibleSeas();
        },
        async getSeaCentres(this: This): Promise<SeaCentre[]> {
            const latestOutcomes =
                await this.endpoints.outcome.getLatestOutcomes();
            const seas = this.uniqueByKey(
                latestOutcomes.map((outcome) => outcome.sea),
                (sea) => sea.id
            ) as Sea[];
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
            console.log("seaCentres", seaCentres);
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
            if (!this.isHighlighted(seaCentre)) {
                alert("This sea cannot be used.");
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
                    alert("Please choose an integer number of points.");
                } else {
                    await this.endpoints.purchase.purchaseShips(
                        this.ui.purchase.seaToPurchaseIn,
                        points
                    );
                    this.ui.purchase.showModal = false;
                    this.ui.action = "none";
                    this.ui.purchase.showModal = false;
                    this.ui.purchase.pointsToSpendOnShips = "";
                    this.ui.purchase.seaToPurchaseIn = undefined;
                    await this.refreshMap();
                }
            }
        },
        async onSubmitMove(this: This) {
            if (this.ui.move.showModal) {
                const ships = parseInt(this.ui.move.shipsToMove);
                if (isNaN(ships)) {
                    alert("Please choose an integer number of ships to move.");
                } else {
                    await this.endpoints.move.moveShips(
                        this.ui.move.seaToMoveFrom,
                        this.ui.move.seaToMoveTo,
                        ships
                    );
                    this.ui.move.showModal = false;
                    this.ui.action = "none";
                    this.ui.move.seaToMoveFrom = undefined;
                    this.ui.move.seaToMoveTo = undefined;
                    this.ui.move.shipsToMove = "";
                    await this.refreshMap();
                }
            }
        },
        isHighlighted(this: This, sea: Sea) {
            if (this.ui.action === "purchase") {
                return this.accessibleSeas.some(
                    (accessibleSea) => sea.id === accessibleSea.id
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
                    return this.accessibleSeas.some(
                        (accessibleSea) => sea.id === accessibleSea.id
                    );
                } else {
                    return false;
                }
            } else {
                return false;
            }
        },
        uniqueByKey(items: object[], keySelector: (item) => object): object[] {
            return [
                ...new Map(
                    items.map((item) => [keySelector(item), item])
                ).values(),
            ];
        },
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
    /* top: 50%;
    left: 50%; */
    /* TO SELF: tweak this programatically to reposition */
    /* margin-top: -14px;
    margin-left: -50px; */
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
