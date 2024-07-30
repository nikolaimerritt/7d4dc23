<template>
    <div>
        <object
            type="image/svg+xml"
            ref="imageContainer"
            :data="`/imgs/seas/${seaImages[name]}`"
            :class="['image-container', imageClass]"
        ></object>
        <div ref="container" class="container">
            <team-ship
                v-for="(ship, index) in teamShips"
                :key="index"
                :teamName="ship.team.name"
                :shipCount="ship.shipCount"
            >
            </team-ship>
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
.container {
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
}

.image-container {
    position: absolute;
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
</style>
