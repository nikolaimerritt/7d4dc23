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
            >
            </sea-centre>
            <img :src="'../../imgs/map-cropped.jpg'" class="map-background" />
        </div>
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "./endpoints/team";
import { PurchaseEndpoint } from "./endpoints/purchase";
import { Sea, SeaEndpoint } from "./endpoints/sea";

const seaCoords = {
    "North Pacific": [0.1, 0.2],
    "South Pacific": [0.3, 0.4],
};

interface Data {
    endpoints: {
        team: TeamEndpoint;
        purchase: PurchaseEndpoint;
        sea: SeaEndpoint;
    };
    team?: Team;
    balance?: number;
    seas?: Sea[];
}

type This = Data;

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
        this.seas = await this.endpoints.sea.getAllSeas();
    },
    methods: {
        incrementClicks() {
            this.clicks++;
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
    top: 50%;
    left: 50%;
    /* TO SELF: tweak this programatically to reposition */
    margin-top: -14px;
    margin-left: -50px;
}
</style>
