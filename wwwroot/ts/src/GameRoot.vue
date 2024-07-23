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
        </div>
        <div class="map-container">
            <sea-centre
                v-for="(seaCentre, index) in this.seaCentres"
                :key="index"
                :name="seaCentre.name"
                :teamShips="seaCentre.teamShips"
                :action="ui.action"
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
        <div v-if="ui.showPurchaseModal" class="modal-wrapper">
            <div class="modal-box">
                <input v-model="ui.pointsToSpendOnShips" />
                <button v-on:click="onSubmitPurchase()">Purchase</button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "./endpoints/team";
import { PurchaseEndpoint } from "./endpoints/purchase";
import { Sea, SeaEndpoint } from "./endpoints/sea";
import { OutcomeEndpoint, Outcome } from "./endpoints/outcome";

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
    };
    ui: {
        action: Action;
        showPurchaseModal: boolean;
        pointsToSpendOnShips: string;
        seaToPurchaseShipsIn?: Sea;
    };
    team?: Team;
    balance?: number;
    seaCentres: SeaCentre[];
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
            },
            seaCentres: [],
            ui: {
                action: "none",
                showPurchaseModal: false,
                pointsToSpendOnShips: "",
                seaToPurchaseShipsIn: undefined,
            },
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        this.balance = await this.endpoints.purchase.getBalance();
        this.seaCentres = await this.getSeaCentres();
    },
    methods: {
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
            }
        },
        onSeaCentreClick(this: This, seaCentre: SeaCentre) {
            if (this.ui.action === "purchase") {
                this.ui.action = "none";
                this.ui.pointsToSpendOnShips = "";
                this.ui.seaToPurchaseShipsIn = seaCentre;
                this.ui.showPurchaseModal = true;
            }
        },
        async onSubmitPurchase(this: This) {
            if (this.ui.showPurchaseModal) {
                this.ui.showPurchaseModal = false;
            }
            const points = parseInt(this.ui.pointsToSpendOnShips);
            if (isNaN(points)) {
                alert("Please choose an integer number of points.");
            } else {
                await this.endpoints.purchase.purchaseShips(
                    this.ui.seaToPurchaseShipsIn,
                    points
                );
                console.log(
                    "Purchase submitted",
                    this.ui.seaToPurchaseShipsIn,
                    this.ui.pointsToSpendOnShips
                );
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
