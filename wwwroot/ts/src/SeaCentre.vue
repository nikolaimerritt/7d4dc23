<template>
    <div class="circle" v-on:click="emitClick()" :class="this.actionClass">
        <div v-if="this.action === 'none'">
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
        action: String,
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
        actionClass(this) {
            if (this.action === "none") {
                return "action-none";
            } else {
                return "action-purchase";
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

.action-none {
    background: transparent;
}

.action-purchase {
    background: lightgreen;
}
</style>
