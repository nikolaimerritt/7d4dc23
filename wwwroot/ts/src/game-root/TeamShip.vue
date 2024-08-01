<template>
    <div class="ship">
        <div class="icon-container">
            <!-- <img :src="'../../imgs/ship.svg'" color="red" /> -->
            <ShipSvg :style="{ color: shipColour() }" />
            <span class="ship-count"> {{ shipCount }} </span>
        </div>
        <div class="team-name">{{ teamName }}</div>
    </div>
</template>

<script lang="ts">
import ShipSvg from "../../../imgs/ship.svg";
import { Style } from "../config/style";

export default {
    props: {
        teamName: String,
        shipCount: Number,
    },
    methods: {
        shipColour() {
            return Style.teamColour(this.teamName);
        },
    },
    components: {
        ShipSvg,
    },
};
</script>
<style scoped>
.ship {
    display: flex;
    width: 60px;
    flex-direction: column;
    align-items: center;
    z-index: 30;
}
.icon-container {
    position: absolute;
    width: min-content;
}

svg {
    width: 60px;
    color: blue;
}

.recolour-image {
    overflow: hidden;
}

.recolour-image img {
    filter: drop-shadow(1px 1000px 0 red);
    transform: translateY(-999px);
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

span {
    color: #fff4c7;
}
</style>
