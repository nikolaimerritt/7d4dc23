<template>
    <div class="horizontal-container">
        <div class="left-container">
            <div class="menu-bar">
                <div class="menu-section">
                    <text-button
                        v-if="balance !== undefined && balance >= pointsPerShip"
                        :text="'Purchase ships'"
                        :enabled="ui.round.state === 'move'"
                        @buttonClick="onPurchaseShipsClick()"
                    ></text-button>
                    <text-button
                        :text="'Move ships'"
                        :enabled="ui.round.state === 'move' && canMove()"
                        @buttonClick="onMoveShipsClick()"
                    >
                    </text-button>
                </div>
                <div class="menu-section">
                    <text-pill
                        v-if="this.balance !== undefined"
                        :text="`${this.balance} points`"
                    ></text-pill>
                    <text-pill
                        v-if="this.ui.round.text"
                        :text="this.ui.round.text"
                    ></text-pill>
                </div>
            </div>
            <div class="dialog" v-if="dialogText()">
                {{ dialogText() }}
            </div>
            <div class="map-container">
                <img
                    :src="'/imgs/seas/map-sepia-2.png'"
                    class="map-background"
                    ref="mapBackground"
                />
                <sea-centre
                    v-for="(seaState, index) in this.seaStates"
                    :key="index"
                    :sea="seaState.sea"
                    :teamShips="seaState.teamShips"
                    :highlighted="isHighlighted(seaState.sea)"
                    :isFightingRound="ui.round.state === 'fighting'"
                    class="sea-centre"
                    @sea-centre-click="onSeaCentreClick(seaState)"
                    :style="{
                        transformOrigin: 'top left',
                        transform: `scale(${ui.seaCentreScale})`,
                        top: `${
                            100 * ui.seaCentreDrawConfig[seaState.sea.name].top
                        }%`,
                        left: `${
                            100 * ui.seaCentreDrawConfig[seaState.sea.name].left
                        }%`,
                    }"
                >
                </sea-centre>
            </div>
        </div>
        <message-board></message-board>
        <tutorial
            :show="ui.showTutorial"
            @completed="onCompletedTutorial()"
        ></tutorial>
        <div
            class="tutorial-button"
            title="Show Tutorial"
            @click="ui.showTutorial = true"
        >
            <question-icon class="tutorial-icon"></question-icon>
        </div>
        <slider-modal
            v-if="ui.purchase.showModal"
            :title="'Purchase Ships'"
            :message="'How many points would you like to spend to purchase new ships?'"
            :buttonText="'Purchase'"
            :errorMessage="ui.purchase.subtext"
            :slider="{ min: 0, max: balance, step: pointsPerShip }"
            @inputChange="onPurchaseInputChange($event)"
            @submission="onSubmitPurchase($event)"
            @clickOutside="resetActions()"
        ></slider-modal>
        <slider-modal
            v-if="ui.move.showModal"
            :title="'Move Ships'"
            :message="'How many ships would you like to move?'"
            :buttonText="'Move'"
            :errorMessage="ui.move.subtext"
            :slider="{
                min: 0,
                max: maxShipsThatCanBeMoved(),
                step: 1,
            }"
            @submission="onSubmitMove($event)"
            @inputChange="onMoveInputChange($event)"
            @clickOutside="resetActions()"
        >
        </slider-modal>
    </div>
</template>

<script lang="ts">
import { TeamEndpoint, Team } from "../endpoints/team";
import {
    SeaStateEndpoint,
    SeaState,
    TeamShipCount,
} from "../endpoints/sea-state";
import { PurchaseEndpoint } from "../endpoints/purchase";
import { Sea, SeaEndpoint } from "../endpoints/sea";
import { OutcomeEndpoint } from "../endpoints/outcome";
import { MoveEndpoint } from "../endpoints/move";
import { Connection } from "../endpoints/main";
import { Round, RoundEndpoint } from "../endpoints/round";
import { Util, VueThis } from "../common/util";
import * as moment from "moment";
import { MessageEndpoint } from "../endpoints/message";
import TeamShip from "./TeamShip.vue";
import MusicBox from "../common/music-box";

const updateRoundTextMs = 2_000;
const updateMapMs = 5_000;
const hasCompletedTutorialCookie = "completed-tutorial";
type Action = "none" | "purchase" | "move";
type RoundState = "move" | "fighting" | "ended";

type SeaCentreDrawConfig = {
    [seaName: string]: { top: number; left: number; drawOrder: number };
};

interface Data {
    endpoints: {
        team: TeamEndpoint;
        purchase: PurchaseEndpoint;
        sea: SeaEndpoint;
        outcome: OutcomeEndpoint;
        move: MoveEndpoint;
        round: RoundEndpoint;
        seaState: SeaStateEndpoint;
        messages: MessageEndpoint;
    };
    ui: {
        action: Action;
        purchase: {
            showModal: boolean;
            seaToPurchaseIn?: Sea;
            subtext: string;
        };
        move: {
            showModal: boolean;
            seaToMoveFrom?: SeaState;
            seaToMoveTo?: SeaState;
            subtext: string;
        };
        round: {
            text: string;
            state: RoundState;
            updateHandle?: number;
        };
        map: {
            updateMapHandle: number;
        };
        messages: {
            hasNotifications: boolean;
            showBoard: boolean;
        };
        showTutorial: boolean;
        seaCentreDrawConfig: SeaCentreDrawConfig;
        seaCentreScale: number | undefined;
    };
    team?: Team;
    balance?: number;
    pointsPerShip: number;
    seaStates: SeaState[];
    accessibleSeas: Sea[];
    rounds?: Round[];
}

type This = VueThis<Data>;

export default {
    data(): Data {
        return {
            team: undefined,
            balance: undefined,
            pointsPerShip: 5,
            accessibleSeas: [],
            rounds: [],
            endpoints: {
                team: new TeamEndpoint(),
                purchase: new PurchaseEndpoint(),
                sea: new SeaEndpoint(),
                outcome: new OutcomeEndpoint(),
                move: new MoveEndpoint(),
                round: new RoundEndpoint(),
                seaState: new SeaStateEndpoint(),
                messages: new MessageEndpoint(),
            },
            seaStates: [],
            ui: {
                action: "none",
                purchase: {
                    showModal: false,
                    seaToPurchaseIn: undefined,
                    subtext: "",
                },
                move: {
                    showModal: false,
                    seaToMoveFrom: undefined,
                    seaToMoveTo: undefined,
                    subtext: "",
                },
                round: {
                    text: "",
                    state: "ended",
                    updateHandle: undefined,
                },
                map: {
                    updateMapHandle: undefined,
                },
                messages: {
                    hasNotifications: false,
                    showBoard: false,
                },
                seaCentreScale: undefined,
                showTutorial: false,
                seaCentreDrawConfig: {
                    "North Atlantic": {
                        top: 0.211,
                        left: 0.115,
                        drawOrder: 7,
                    },
                    "South Atlantic": {
                        top: 0.53,
                        left: 0.223,
                        drawOrder: 6,
                    },
                    Indian: {
                        top: 0.393,
                        left: 0.47,
                        drawOrder: 5,
                    },
                    Southern: {
                        top: 0.862,
                        left: 0,
                        drawOrder: 4,
                    },
                    Arctic: {
                        top: 0,
                        left: 0,
                        drawOrder: 3,
                    },
                    "North Pacific": {
                        top: 0.213,
                        left: 0,
                        drawOrder: 2,
                    },
                    "South Pacific": {
                        top: 0.515,
                        left: 0,
                        drawOrder: 1,
                    },
                },
            },
        };
    },
    async mounted(this: This) {
        this.team = await this.endpoints.team.getTeam();
        this.ui.map.updateMapHandle = await Util.doAndRepeat(
            () => this.updateMap(),
            updateMapMs
        );
        this.transformSeaCentres();
        this.ui.round.updateHandle = await Util.doAndRepeat(
            () => this.updateRounds(),
            updateRoundTextMs
        );
        window.addEventListener("resize", () => this.transformSeaCentres());
        if (!this.hasCompletedTutorial()) {
            this.ui.showTutorial = true;
        }
        MusicBox.playIntro();
    },
    methods: {
        async updateMap(this: This) {
            this.balance = await this.endpoints.purchase.getBalance();
            this.seaStates = Util.sortBy(
                await this.endpoints.seaState.getSeaStates(),
                (seaState) =>
                    this.ui.seaCentreDrawConfig[seaState.sea.name]?.drawOrder
            );
            this.accessibleSeas = await this.endpoints.sea.getAccessibleSeas();
            this.rounds = await this.endpoints.round.getRounds();
            if (!this.ui.messages.showBoard) {
                this.ui.messages.hasNotifications =
                    (await this.endpoints.messages.getUnreadNotifications())
                        .length > 0;
            }
        },
        updateRounds(this: This) {
            const now = new Date();
            this.ui.round.text = this.roundText();
            const currentRound = this.rounds.find(
                (round) => round.startPlanning <= now && now < round.end
            );
            if (currentRound === undefined) {
                this.ui.round.state = "ended";
            } else if (now < currentRound.startCooldown) {
                this.ui.round.state = "move";
            } else {
                this.ui.round.state = "fighting";
            }
        },
        transformSeaCentres(this: This) {
            const mapBackground = this.$refs.mapBackground as HTMLImageElement;
            this.ui.seaCentreScale =
                mapBackground.width / mapBackground.naturalWidth;
        },
        onPurchaseShipsClick(this: This) {
            if (this.ui.action === "none") {
                this.ui.action = "purchase";
            } else if (this.ui.action === "purchase") {
                this.resetActions();
            }
        },
        resetActions(this: This) {
            this.ui.action = "none";

            this.ui.purchase.showModal = false;
            this.ui.purchase.seaToPurchaseIn = undefined;
            this.ui.purchase.subtext = "";

            this.ui.move.showModal = false;
            this.ui.move.subtext = "";
            this.ui.move.seaToMoveFrom = undefined;
            this.ui.move.seaToMoveTo = undefined;
            this.ui.move.showModal = false;
        },
        onMoveShipsClick(this: This) {
            if (this.ui.action === "none") {
                this.ui.action = "move";
            } else if (this.ui.action === "move") {
                this.resetActions();
            }
        },
        onSeaCentreClick(this: This, seaState: SeaState) {
            if (this.ui.action === "purchase") {
                this.ui.purchase.seaToPurchaseIn = seaState.sea;
                this.ui.purchase.showModal = true;
            } else if (this.ui.action === "move") {
                if (this.ui.move.seaToMoveFrom === undefined) {
                    this.ui.move.seaToMoveFrom = seaState;
                } else if (this.ui.move.seaToMoveTo === undefined) {
                    this.ui.move.seaToMoveTo = seaState;
                    this.ui.move.showModal = true;
                }
            }
        },
        onMessageButtonClick(this: This) {
            this.ui.messages.showBoard = !this.ui.messages.showBoard;
        },
        onPurchaseInputChange(this: This, points: number) {
            const shipCount = Math.floor(points / this.pointsPerShip);
            if (shipCount > 0) {
                const shipsPlural = shipCount === 1 ? "ship" : "ships";
                const pointsPlural = points === 1 ? "point" : "points";
                this.ui.purchase.subtext = `You are about to purhcase ${shipCount} ${shipsPlural} for ${points} ${pointsPlural}.`;
            } else {
                this.ui.purchase.subtext = "";
            }
        },
        onMoveInputChange(this: This, shipsToMove: number) {
            if (shipsToMove > 0) {
                const shipsPlural = shipsToMove === 1 ? "ship" : "ships";
                this.ui.move.subtext = `You are about to move ${shipsToMove} ${shipsPlural}.`;
            } else {
                this.ui.move.subtext = "";
            }
        },
        maxShipsThatCanBeMoved(this: This): number {
            const seaToMoveFrom = this.ui.move.seaToMoveFrom;
            return seaToMoveFrom?.teamShips?.find(
                (teamShip) => teamShip.team.id === this.team.id
            )?.shipCount;
        },
        async onSubmitPurchase(this: This, pointsToSpend: string) {
            if (this.ui.purchase.showModal) {
                const points = parseInt(pointsToSpend);
                if (isNaN(points)) {
                    this.ui.purchase.subtext =
                        "Please choose a valid number of points.";
                } else {
                    const result = await this.endpoints.purchase.purchaseShips(
                        this.ui.purchase.seaToPurchaseIn,
                        points
                    );
                    if (Connection.isError(result)) {
                        this.ui.purchase.subtext = result.error;
                    } else {
                        this.resetActions();
                        await this.updateMap();
                    }
                }
            }
        },
        async onSubmitMove(this: This, shipsToMove: string) {
            if (this.ui.move.showModal) {
                const ships = parseInt(shipsToMove);
                if (isNaN(ships)) {
                    this.ui.move.subtext =
                        "Please choose a valid number of ships to move.";
                } else {
                    const result = await this.endpoints.move.moveShips(
                        this.ui.move.seaToMoveFrom.sea,
                        this.ui.move.seaToMoveTo.sea,
                        ships
                    );
                    if (Connection.isError(result)) {
                        this.ui.move.subtext = result.error;
                    } else {
                        this.resetActions();
                        await this.updateMap();
                    }
                }
            }
        },
        isHighlighted(this: This, sea: Sea) {
            if (this.ui.action === "purchase") {
                return (
                    this.ui.purchase.seaToPurchaseIn === undefined &&
                    this.seaIsAccessible(sea)
                );
            } else if (this.ui.action === "move") {
                if (this.ui.move.seaToMoveFrom === undefined) {
                    return this.canMoveFromSea(sea);
                } else if (this.ui.move.seaToMoveTo === undefined) {
                    return (
                        sea.id !== this.ui.move.seaToMoveFrom.sea.id &&
                        this.seaIsAccessible(sea) &&
                        this.ui.move.seaToMoveFrom.sea.adjacentSeas.some(
                            (adjacentSea) => adjacentSea.id === sea.id
                        )
                    );
                } else {
                    return false;
                }
            } else {
                return false;
            }
        },
        seaIsAccessible(this: This, sea: Sea): boolean {
            return this.accessibleSeas.some(
                (accessibleSea) => sea.id === accessibleSea.id
            );
        },
        canMoveFromSea(this: This, sea: Sea): boolean {
            return this.seaStates.some(
                (seaState) =>
                    seaState.sea.id === sea.id &&
                    seaState.teamShips.some(
                        (teamShip) =>
                            teamShip.team.id === this.team.id &&
                            teamShip.shipCount > 0
                    )
            );
        },
        canMove(this: This): boolean {
            return this.seaStates.some((seaState) =>
                this.canMoveFromSea(seaState.sea)
            );
        },
        dialogText(this: This): string {
            if (this.ui.action === "move") {
                if (this.ui.move.seaToMoveFrom === undefined) {
                    return "Select a sea to move ships from.";
                } else if (this.ui.move.seaToMoveTo === undefined) {
                    return "Select a sea to move ships to.";
                }
            } else if (this.ui.action === "purchase") {
                if (this.ui.purchase.seaToPurchaseIn === undefined) {
                    return "Select a sea to purchase ships in.";
                }
            }
            return "";
        },
        roundText(this: This) {
            const now = new Date();
            const firstRound = Util.minBy(this.rounds, (round) =>
                round.startPlanning.getTime()
            );
            if (now <= firstRound.startPlanning) {
                return `Round 1 starts in ${Util.formatTimeBetween(
                    firstRound.startPlanning,
                    now
                )}`;
            }

            for (let i = 0; i < this.rounds.length; i++) {
                if (
                    this.rounds[i].startPlanning <= now &&
                    now <= this.rounds[i].startCooldown
                ) {
                    return `Round ${i + 1} ends in ${Util.formatTimeBetween(
                        this.rounds[i].startCooldown,
                        now
                    )}`;
                } else if (
                    i + 1 < this.rounds.length &&
                    this.rounds[i].startCooldown <= now &&
                    now <= this.rounds[i + 1].startPlanning
                ) {
                    return `Round ${i + 2} starts in ${Util.formatTimeBetween(
                        this.rounds[i + 1].startPlanning,
                        now
                    )}`;
                }
            }

            return `The game has ended`;
        },
        hasCompletedTutorial(): boolean {
            const cookie = Util.getCookie(hasCompletedTutorialCookie);
            return cookie !== undefined && cookie.length > 0;
        },
        onCompletedTutorial(this: This) {
            Util.setCookie(hasCompletedTutorialCookie, "true");
            this.ui.showTutorial = false;
        },
    },
    destroyed(this: This) {
        window.clearInterval(this.ui.round.updateHandle);
        window.clearInterval(this.ui.map.updateMapHandle);
        window.removeEventListener("resize", () => this.transformSeaCentres());
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
.dialog {
    padding: 0 0 12px 12px;
}
.horizontal-container {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: center;
}
.left-container {
    width: 60%;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
}

.menu-bar {
    width: 100%;
    padding: 24px 20px 0px 20px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    column-gap: 32px;
    margin-bottom: 8px;
}

.menu-section {
    display: flex;
    flex-direction: row;
    gap: 40px;
}

.map-background {
    width: 100%;
    border-radius: 16px;
}

.map-container {
    position: relative;
    display: inline-block;
    border-radius: 16px;
    border: 4px solid $border-color;
    background: $foreground-color;
}

.sea-centre {
    position: absolute;
    z-index: $sea-z-index;
}

.tutorial-icon {
    color: $font-color;
    width: 30px;
    height: 30px;
}

.tutorial-button {
    position: fixed;
    display: inline-block;
    padding: 4px;
    bottom: $bottom-buttons-offset;
    right: 60px;
    z-index: $message-button-z-index;
    border-radius: 50%;
    border: 2px solid $border-color;
    background: $foreground-color;

    &:hover {
        background: $hover-color;
        cursor: pointer;
    }
}
</style>
