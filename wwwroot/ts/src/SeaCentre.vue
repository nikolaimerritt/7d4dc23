<template>
    <div class="circle" v-on:click="emitClick()" :class="this.highlightedClass">
        <div v-if="!this.highlighted">
            <div class="ship" v-for="(ship, index) in teamShips" :key="index">
                <img :src="'../../imgs/ship.png'" />
                <span> {{ ship.shipCount }} </span>
                <span> {{ ship.team.name }} </span>
            </div>
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
        highlighted: Boolean,
        teamShips: {
            type: Array<TeamShips>,
        },
    },
    methods: {
        emitClick() {
            this.$emit("sea-centre-click", this.name);
        },
    },
    computed: {
        highlightedClass(this) {
            if (this.highlighted) {
                return "state-highlighted";
            } else {
                return "";
            }
        },
    },
};
</script>

<style scoped>
.circle {
    position: absolute;
    border-radius: 50%;
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

.state-highlighted {
    background: lightgreen;
}
:not(.state-highlighted) {
    background: transparent;
}
</style>
