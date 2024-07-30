<template>
    <div>
        <object
            type="image/svg+xml"
            ref="imageContainer"
            :data="`/imgs/seas/${seaImages[name]}`"
            :class="['image-container', imageClass]"
        ></object>
        <div ref="shipContainer" class="ship-container">
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
            const shipContainer = this.$refs.shipContainer;
            const imageObject = Util.getHtmlObjectContent(
                this.$refs.imageContainer
            );
            const pathObjects = Array.from(
                imageObject.querySelectorAll("svg path")
            ) as SVGPathElement[];
            for (const svgPath of pathObjects) {
                svgPath.addEventListener(
                    "mouseover",
                    () => (this.hover = true)
                );
                svgPath.addEventListener(
                    "mouseleave",
                    () => (this.hover = false)
                );
                svgPath.addEventListener("mousedown", () => this.emitClick());
            }
            console.log(
                "All rects",
                this.name,
                pathObjects.map((obje) => obje.getBoundingClientRect())
            );
            const largestRect = Util.maxBy(pathObjects, (svgPath) => {
                const rect = svgPath.getBoundingClientRect();
                return rect.width * rect.height;
            }).getBoundingClientRect();
            console.log("largest rect", this.name, largestRect);
            shipContainer.style.width = `${largestRect.width}px`;
            shipContainer.style.height = `${largestRect.height}px`;
            shipContainer.style.top = `${largestRect.top}px`;
            shipContainer.style.left = `${largestRect.left}px`;
        }, 300);
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
        imageClass(this: This) {
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
.ship-container {
    position: relative;
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
    flex-wrap: wrap;
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
