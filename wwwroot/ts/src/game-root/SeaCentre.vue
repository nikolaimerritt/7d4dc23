<template>
    <div ref="container" :data-sea-name="seaName()">
        <arctic-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-if="sea.name === 'Arctic'"
        >
        </arctic-sea>
        <north-atlantic-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-else-if="sea.name === 'North Atlantic'"
        >
        </north-atlantic-sea>
        <north-pacific-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-else-if="sea.name === 'North Pacific'"
        >
        </north-pacific-sea>
        <south-atlantic-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-else-if="sea.name === 'South Atlantic'"
        >
        </south-atlantic-sea>
        <south-pacific-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-else-if="sea.name === 'South Pacific'"
        >
        </south-pacific-sea>
        <indian-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-else-if="sea.name === 'Indian'"
        ></indian-sea>
        <southern-sea
            ref="seaImage"
            :class="['image-container', imageClass]"
            v-else-if="sea.name === 'Southern'"
        ></southern-sea>
        <div ref="shipContainer" style="position: relative">
            <div class="ship-container">
                <team-ship
                    v-for="(ship, index) in teamShips"
                    v-show="loaded"
                    :key="index"
                    :teamName="ship.team.name"
                    :shipCount="ship.shipCount"
                >
                </team-ship>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Team } from "../endpoints/team";
import { Util, VueThis } from "../common/util";
import NorthPacificSvg from "../../../imgs/seas/north-pacific-cropped.svg";
import SouthPacificSvg from "../../../imgs/seas/south-pacific-cropped.svg";
import NorthAtlanticSvg from "../../../imgs/seas/north-atlantic-cropped.svg";
import SouthAtlanticSvg from "../../../imgs/seas/south-atlantic-cropped.svg";
import SouthernSvg from "../../../imgs/seas/southern-cropped.svg";
import IndianSvg from "../../../imgs/seas/indian-cropped.svg";
import ArcticSvg from "../../../imgs/seas/arctic-cropped.svg";
import { Sea } from "../endpoints/sea";
interface TeamShips {
    team: Team;
    shipCount: number;
}

type SeaImage = { [seaName: string]: string };

interface Props {
    sea: Sea;
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
        sea: {
            type: Object,
        },
        highlighted: Boolean,
        teamShips: {
            type: Array<TeamShips>,
        },
    },
    components: {
        NorthPacificSvg,
        SouthPacificSvg,
        NorthAtlanticSvg,
        SouthAtlanticSvg,
        SouthernSvg,
        IndianSvg,
        ArcticSvg,
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
        if (Util.isComponentRef(this.$refs.seaImage)) {
            this.$refs.seaImage.$el.style.borderRadius = this.borderRadius();
            const svgPaths = Array.from(
                this.$refs.seaImage.$el.querySelectorAll("path")
            ) as SVGPathElement[];
            for (var path of svgPaths) {
                this.addMouseEvents(path);
            }
            const largestPath = Util.maxBy(
                svgPaths,
                (path) =>
                    path.getBoundingClientRect().width *
                    path.getBoundingClientRect().height
            );
            this.fitElementTo(
                this.$refs.shipContainer,
                largestPath,
                this.$refs.seaImage.$el
            );
            await Util.sleep(100);
        } else {
            console.log(
                "seaImage",
                this.$refs.seaImage,
                Util.isComponentRef(this.$refs.seaImage),
                this.$refs.seaImage instanceof HTMLElement
            );
        }

        this.loaded = true;
    },
    methods: {
        addMouseEvents(path: SVGPathElement) {
            path.addEventListener("mouseover", (event) =>
                this.onHover(path, event)
            );
            path.addEventListener("mouseleave", () => this.onHoverExit(path));
            path.addEventListener("mousedown", () => this.emitClick());
        },

        fitElementTo(
            toFit: HTMLElement,
            fitTo: HTMLElement,
            parent: HTMLElement
        ) {
            const leftOffset =
                fitTo.getBoundingClientRect().left -
                parent.getBoundingClientRect().left;
            const topOffset =
                fitTo.getBoundingClientRect().top -
                parent.getBoundingClientRect().top;
            toFit.style.width = `${fitTo.getBoundingClientRect().width}px`;
            toFit.style.height = `${fitTo.getBoundingClientRect().height}px`;
            toFit.style.left = `${leftOffset}px`;
            toFit.style.top = `${topOffset}px`;
        },
        onHover(this: This, path: SVGPathElement) {
            this.hover = true;
            if (this.highlighted) {
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
            if (this.sea.name === "Arctic") {
                return "16px 16px 0 0";
            } else if (this.sea.name === "Southern") {
                return "0 0 16px 16px";
            } else {
                return "";
            }
        },
        seaName(this: This) {
            return Util.seaNameTitleCase(this.sea);
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
            if (this.hover && this.highlighted) {
                return "state-highlighted-hover";
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
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
    flex-wrap: wrap;
    width: 100%;
    height: 100%;
}

.image-container {
    z-index: 30;
    position: absolute;
    top: 0;
    left: 0;
}
.state-highlighted-hover {
    filter: brightness(0) saturate(100%) invert(82%) sepia(27%) saturate(532%)
        hue-rotate(355deg) brightness(84%) contrast(86%);
}
.state-highlighted {
    filter: brightness(0) saturate(100%) invert(96%) sepia(43%) saturate(961%)
        hue-rotate(317deg) brightness(90%) contrast(86%);
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

.sea-name-tooltip {
    position: fixed;
    z-index: 50;
    left: 50px;
    top: 50px;
}
</style>
