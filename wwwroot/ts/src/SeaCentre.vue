<template>
    <!-- TO SELF: debug -->
    <div>
        <object
            type="image/svg+xml"
            ref="imageContainer"
            :data="`/imgs/seas/${seaImages[name]}`"
            :class="imageClass"
            v-on:click="emitClick()"
        ></object>
        <div v-for="(ship, index) in teamShips" :key="index" class="ship">
            <div style="position: absolute">
                <img :src="'../../imgs/ship.png'" />
                <span class="ship-count"> {{ ship.shipCount }} </span>
            </div>
            <div class="team-name">{{ ship.team.name }}</div>
        </div>
    </div>

    <!-- <div
        v-if="this.highlighted"
        class="circle"
        v-on:click="emitClick()"
        :class="this.highlightedClass"
    ></div>
    <div v-else>
    </div> -->
</template>

<script lang="ts">
import { Team } from "./endpoints/team";
import { Util } from "./util";
interface TeamShips {
    team: Team;
    shipCount: number;
}

type SeaImage = { [seaName: string]: string };

interface Data {
    seaImages: SeaImage;
    hover: boolean;
}

export default {
    props: {
        name: String,
        highlighted: Boolean,
        teamShips: {
            type: Array<TeamShips>,
        },
    },
    data(): Data {
        return {
            seaImages: {
                "North Pacific": "north-pacific-cropped.svg",
                "South Pacific": "south-pacific-cropped.svg",
                "North Atlantic": "north-atlantic-cropped.svg",
                "South Atlantic": "south-atlantic-cropped.svg",
                Southern: "southern-cropped.svg",
                Indian: "indian-cropped.svg",
                Arctic: "arctic-cropped.svg",
            },
            hover: false,
        };
    },
    mounted() {
        window.setTimeout(() => {
            const imageObject = this.$refs.imageContainer.contentDocument;
            imageObject
                .querySelectorAll("svg path")
                .forEach((svgPath: SVGPathElement) => {
                    svgPath.addEventListener(
                        "mouseover",
                        () => (this.hover = true)
                    );
                    svgPath.addEventListener(
                        "mouseleave",
                        () => (this.hover = false)
                    );
                    svgPath.addEventListener("mousedown", () =>
                        this.emitClick()
                    );
                });
        }, 1000);
    },
    methods: {
        emitClick() {
            console.log("sea centre clicked", this.name, this.highlighted);
            if (this.highlighted) {
                this.$emit("sea-centre-click", this.name);
            }
        },
        onHover() {
            console.log(`Hovered over sea`, this.name);
        },
    },
    computed: {
        imageClass(this) {
            if (this.highlighted) {
                return "state-highlighted";
            } else if (this.hover) {
                return "state-hover";
            } else {
                return "state-none";
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
    filter: invert(82%) sepia(72%) saturate(2841%) hue-rotate(62deg)
        brightness(92%) contrast(128%);
}
.state-hover {
    filter: invert(56%) sepia(71%) saturate(3468%) hue-rotate(213deg)
        brightness(100%) contrast(90%);
}
.state-none {
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
