<template>
    <div class="ship">
        <div class="icon-container">
            <ShipSvg :style="{ color: shipColour() }" />
            <span class="ship-count"> {{ shipCount }} </span>
            <img
                v-if="isFighting"
                :src="'../../../imgs/fire.gif'"
                class="fire"
            />
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
        isFighting: Boolean,
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
<style lang="scss" scoped>
@import "../assets/style.scss";
.ship {
    width: fit-content;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 0 12px;
    z-index: $ship-z-index;
}
.icon-container {
    position: absolute;
    width: min-content;
}

svg {
    width: 60px;
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

.fire {
    position: absolute;
    width: 40px;
    bottom: 20px;
    left: 10px;
}
.team-name {
    margin-top: 60px;
    text-align: center;
    width: max-content;
}

span {
    color: $foreground-color;
}
</style>
