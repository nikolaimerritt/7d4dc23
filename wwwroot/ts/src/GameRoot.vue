<template>
    <div>
        <div class="menu-bar">
            <span> {{ this.team?.name }} </span>
            <span> {{ this.balance }} Coins </span>
        </div>
        <img :src="'../../imgs/map-cropped.jpg'" class="map-background" />
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "./endpoints/team";
import { PurchaseEndpoint } from "./endpoints/purchase";

interface Data {
    endpoints: {
        team: TeamEndpoint;
        purchase: PurchaseEndpoint;
    };
    team?: Team;
    balance?: number;
}

type This = Data;

export default {
    data() {
        return {
            team: undefined,
            balance: undefined,
            endpoints: {
                team: new TeamEndpoint(),
                purchase: new PurchaseEndpoint(),
            },
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        this.balance = await this.endpoints.purchase.getBalance();
    },
    methods: {
        incrementClicks() {
            this.clicks++;
        },
    },
};
</script>
<style scoped>
.map-background {
    width: 100%;
}

.menu-bar {
    display: flex;
    column-gap: 32px;
}
</style>
./endpoints/teams
