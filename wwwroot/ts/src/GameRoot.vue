<template>
    <div>
        <div class="menu-bar">
            <span> {{ this.team?.name }} </span>
            <span> {{ this.balance }} Coins </span>
        </div>
        <div class="map-container">
            <sea-centre
                v-for="sea in this.seas"
                :key="sea.id"
                :name="sea.name"
                class="sea-centre"
                :style="{ left: `${sea.xCoord}%`, top: `${sea.yCoord}%` }"
            >
            </sea-centre>
            <img
                :src="'../../imgs/map-cropped.jpg'"
                class="map-background"
                ref="mapBackground"
            />
        </div>
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "./endpoints/team";
import { PurchaseEndpoint } from "./endpoints/purchase";
import { Sea, SeaEndpoint } from "./endpoints/sea";

type SeaCentre = Sea & {
    xCoord: number;
    yCoord: number;
};

interface Data {
    endpoints: {
        team: TeamEndpoint;
        purchase: PurchaseEndpoint;
        sea: SeaEndpoint;
    };
    team?: Team;
    balance?: number;
    seas?: SeaCentre[];
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
            seas: [],
            endpoints: {
                team: new TeamEndpoint(),
                purchase: new PurchaseEndpoint(),
                sea: new SeaEndpoint(),
            },
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        this.balance = await this.endpoints.purchase.getBalance();
        this.seas = await this.getSeaCentres();
    },
    methods: {
        async getSeaCentres(this: This): Promise<SeaCentre[]> {
            const seas = await this.endpoints.sea.getAllSeas();
            const mapImage = this.$refs.mapBackground;
            return seas.map((sea) => {
                if (!(sea.name in normalisedSeaCoords)) {
                    console.error(
                        `There is no position config for a sea named ${sea.name}`
                    );
                }
                const seaCentre: SeaCentre = {
                    xCoord: 100 * normalisedSeaCoords[sea.name][0],
                    yCoord: 100 * normalisedSeaCoords[sea.name][1],
                    ...sea,
                };
                return seaCentre;
            });
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
</style>
