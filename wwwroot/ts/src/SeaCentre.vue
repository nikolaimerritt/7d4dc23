<template>
    <div
        v-if="this.highlighted"
        class="circle"
        v-on:click="emitClick()"
        :class="this.highlightedClass"
    ></div>
    <div v-else>
        <div v-for="(ship, index) in teamShips" :key="index" class="ship">
            <div style="position: absolute">
                <img :src="'../../imgs/ship.png'" />
                <span class="ship-count"> {{ ship.shipCount }} </span>
            </div>
            <div class="team-name">{{ ship.team.name }}</div>
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
    display: flex;
    width: 60px;
    flex-direction: column;
    align-items: center;
}

.state-highlighted {
    background: lightgreen;
}
:not(.state-highlighted) {
    background: transparent;
}
.ship-count {
    position: relative;
    top: -45px;
    display: inline-block;
    width: 100%;
    text-align: center;
}

.team-name {
    margin-top: 60px;
    text-align: center;
    width: max-content;
}

img {
    width: 100%;
}
</style>
