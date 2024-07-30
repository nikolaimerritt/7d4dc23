<template>
    <div>
        <object
            type="image/svg+xml"
            ref="imageContainer"
            :data="`/imgs/seas/${seaImages[name]}`"
            :class="['image-container', imageClass]"
        ></object>
        <div ref="shipContainer" class="ship-container" v-show="loaded">
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

interface Props {
    name: string;
    highlighted: boolean;
    teamShips: TeamShips[];
}

interface Data {
    seaImages: SeaImage;
    hover: boolean;
    loaded: boolean;
}

type This = VueThis<Data> & Props;

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
            loaded: false,
        };
    },
    async mounted(this: This) {
        await Util.sleep(500);
        const shipContainer = this.$refs.shipContainer;
        const imageObject = Util.getHtmlObjectContent(
            this.$refs.imageContainer
        );
        const pathObjects = Array.from(
            imageObject.querySelectorAll("svg path")
        ) as SVGPathElement[];
        for (const svgPath of pathObjects) {
            svgPath.addEventListener("mouseover", () => this.onHover(svgPath));
            svgPath.addEventListener("mouseleave", () =>
                this.onHoverExit(svgPath)
            );
            svgPath.addEventListener("mousedown", () => this.emitClick());
        }
        const largestRect = Util.maxBy(pathObjects, (svgPath) => {
            const rect = svgPath.getBoundingClientRect();
            return rect.width * rect.height;
        }).getBoundingClientRect();
        shipContainer.style.width = `${largestRect.width}px`;
        shipContainer.style.height = `${largestRect.height}px`;
        shipContainer.style.top = `${largestRect.top}px`;
        shipContainer.style.left = `${largestRect.left}px`;
        this.loaded = true;
    },
    methods: {
        onHover(this: This, path: SVGPathElement) {
            if (this.highlighted) {
                this.hover = true;
                path.style.cursor = "pointer";
            }
        },
        onHoverExit(this: This, path: SVGPathElement) {
            this.hover = false;
            path.style.cursor = "auto";
        },
        emitClick(this: This) {
            if (this.highlighted) {
                this.$emit("sea-centre-click", this.name);
            }
        },
    },
    computed: {
        imageClass(this: This) {
            if (this.hover) {
                return "state-hover";
            } else if (this.highlighted) {
                return "state-highlighted";
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
    z-index: 30;
    position: absolute;
    top: 0;
    left: 0;
}
.state-highlighted {
    filter: brightness(0) saturate(100%) invert(92%) sepia(72%) saturate(383%)
        hue-rotate(323deg) brightness(94%) contrast(89%);
}
.state-hover {
    filter: brightness(0) saturate(100%) invert(82%) sepia(20%) saturate(567%)
        hue-rotate(4deg) brightness(94%) contrast(92%);
}
.state-none {
    filter: opacity(0%);
}
</style>
