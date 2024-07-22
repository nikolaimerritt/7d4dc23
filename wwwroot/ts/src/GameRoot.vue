<template>
    <div>
        <div class="menu-bar">
            <span> {{ this.team?.name }} </span>
        </div>
        <img :src="'../../imgs/map-cropped.jpg'" class="map-background" />
    </div>
</template>

<script lang="ts">
import { TeamsEndpoint, Team } from "./endpoints/teams-endpoint";

interface Data {
    endpoints: {
        team: TeamsEndpoint;
    };
    team?: Team;
}

type This = Data;

export default {
    data() {
        return {
            team: undefined,
            endpoints: {
                team: new TeamsEndpoint(),
            },
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
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
    width: 90vw;
}

.menu-bar {
    display: flex;
}
</style>
