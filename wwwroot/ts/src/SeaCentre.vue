<template>
    <div ref="container" class="container">
        <object
            type="image/svg+xml"
            ref="imageContainer"
            :data="`/imgs/seas/${seaImages[name]}`"
            :class="['image-container', imageClass]"
            v-on:click="emitClick()"
        ></object>
        <div v-for="(ship, index) in teamShips" :key="index" class="ship">
            <div class="icon-container">
                <img :src="'../../imgs/ship.png'" />
                <span class="ship-count"> {{ ship.shipCount }} </span>
            </div>
            <div class="team-name">{{ ship.team.name }}</div>
        </div>
    </div>
</template>

<script lang="ts">
import { Team } from "./endpoints/team";
import { Util, VueThis } from "./util";
interface TeamShips {
    team: Team;
    shipCount: number;
}

type SeaImage = { [seaName: string]: string };

interface Data {
    seaImages: SeaImage;
    hover: boolean;
}

type This = VueThis<Data>;

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
    mounted(this: This) {
        window.setTimeout(() => {
            const imageObject = Util.getHtmlObjectContent(
                this.$refs.imageContainer
            );
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
            const firstPath = imageObject
                .querySelector("svg path")
                .getBoundingClientRect();
            this.$refs.container.style.width = `${firstPath.width}px`;
            this.$refs.container.style.height = `${firstPath.height}px`;
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
.ship {
    display: flex;
    width: 60px;
    flex-direction: column;
    align-items: center;
    z-index: 30;
}

/* .container {
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
} */

.image-container {
    position: relative;
    top: 0;
    left: 0;
}
.state-highlighted {
    filter: brightness(80%);
}
.state-hover {
    filter: brightness(90%);
}
.state-none {
    filter: brightness(90%);
    /* background: transparent; */
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

.icon-container {
    position: absolute;
    width: min-content;
}

img {
    width: 60px;
}
</style>
