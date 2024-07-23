<template>
    <div class="circle">
        <!-- <span> {{ this.name }} </span> -->
        <div class="ship" v-for="(ship, index) in teamShips" :key="index">
            <img :src="'../../imgs/ship.png'" />
            <span> {{ ship.shipCount }} </span>
            <span> {{ ship.team.name }} </span>
        </div>
    </div>
</template>

<script lang="ts">
import { Team } from "./endpoints/team";
interface TeamShips {
    team: Team;
    shipCount: number;
}

export default {
    props: {
        name: String,
        teamShips: {
            type: Array<TeamShips>,
        },
    },
};
</script>

<style scoped>
.circle {
    position: absolute;
    border-radius: 50%;
    background: red;
    width: 50px;
    height: 50px;
    margin-left: -25px;
    margin-top: -25px;
}

.ship {
    position: relative;
    display: flex;
    flex-direction: column;
}
</style>
