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
import { Team } from "../endpoints/team";
import { Util, VueThis } from "../common/util";
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

type This = VueThis<Data & Props>;

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
        const imageObject = Util.getHtmlObjectContent(
            this.$refs.imageContainer
        );
        imageObject.querySelector("svg").style.borderRadius =
            this.borderRadius();

        const svgPaths = Array.from(
            imageObject.querySelectorAll("svg path")
        ) as SVGPathElement[];
        for (var path of svgPaths) {
            this.addMouseEvents(path);
        }

        const pathRects = svgPaths.map((path) => path.getBoundingClientRect());
        const largestRect = Util.maxBy(
            pathRects,
            (rect) => rect.width * rect.height
        );
        this.fitElementTo(this.$refs.shipContainer, largestRect);

        this.loaded = true;
    },
    methods: {
        addMouseEvents(path: SVGPathElement) {
            path.addEventListener("mouseover", () => this.onHover(path));
            path.addEventListener("mouseleave", () => this.onHoverExit(path));
            path.addEventListener("mousedown", () => this.emitClick());
        },
        fitElementTo(element: HTMLElement, rect: DOMRect) {
            element.style.width = `${rect.width}px`;
            element.style.height = `${rect.height}px`;
            element.style.top = `${rect.top}px`;
            element.style.left = `${rect.left}px`;
        },
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
                this.$emit("sea-centre-click");
            }
        },
        borderRadius(this: This): string {
            if (this.name === "Arctic") {
                return "16px 16px 0 0";
            } else if (this.name === "Southern") {
                return "0 0 16px 16px";
            } else {
                return "";
            }
        },
    },
    watch: {
        highlighted(newValue) {
            if (!newValue) {
                this.hover = false;
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

.rounded-top {
    border-radius: 16px 16px 0 0;
}
.rounded-bottom {
    border-radius: 0 0 16px 16px;
}
</style>
./common/util
