<template>
    <div>
        <div class="root" v-show="ui.loaded">
            <div class="team-tabs">
                <div
                    class="team-tab"
                    v-for="(team, index) in otherTeams"
                    :key="index"
                    @click="onTeamTabClick(team)"
                    :class="{
                        'team-tab-selected': team.id === ui.selectedTeam?.id,
                    }"
                >
                    {{ team.name }}
                </div>
            </div>
            <div class="messages" ref="messages">
                <div
                    class="message"
                    v-for="(message, index) in messages"
                    :key="index"
                >
                    <h2>{{ message.sender.name }}</h2>
                    <h3>{{ message.creation.toLocaleString() }}</h3>
                    <div class="content">{{ message.content }}</div>
                </div>
            </div>
            <div class="input-area">
                <textarea maxlength="250" ref="inputBox"></textarea>
                <div class="send-icon-container" @click="sendMessage()">
                    <send-icon class="send-icon"></send-icon>
                </div>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
import { Util, VueThis } from "../common/util";
import { Message, MessageEndpoint } from "../endpoints/message";
import { Team, TeamEndpoint } from "../endpoints/team";
import TeamShipVue from "./TeamShip.vue";

interface Data {
    endpoints: {
        message: MessageEndpoint;
        team: TeamEndpoint;
    };
    ui: {
        loaded: boolean;
        updateHandle?: number;
        selectedTeam?: Team;
    };
    otherTeams: Team[];
    messages: Message[];
}

type This = VueThis<Data>;

const UpdateMessageIntervalMs = 10_000;
const TextAreaRowHeight = 24;
export default {
    data(): Data {
        return {
            endpoints: {
                message: new MessageEndpoint(),
                team: new TeamEndpoint(),
            },
            ui: {
                loaded: false,
                updateHandle: undefined,
                selectedTeam: undefined,
            },
            otherTeams: [],
            // Messages are stored in reverse order
            // so that we can display them in reverse order
            // and the last message is scrolled to.
            messages: [],
        };
    },
    async mounted(this: This) {
        const thisTeam = await this.endpoints.team.getTeam();
        const allTeams = await this.endpoints.team.getAllTeams();
        this.otherTeams = allTeams.filter((team) => team.id !== thisTeam.id);

        if (Util.isHtmlElementRef(this.$refs.inputBox)) {
            const inputBox = this.$refs.inputBox as HTMLTextAreaElement;
            this.$refs.inputBox.addEventListener("input", () =>
                this.resizeHeight(inputBox)
            );
        }
        this.ui.loaded = true;
    },
    methods: {
        async onTeamTabClick(this: This, team: Team) {
            this.ui.selectedTeam = team;
            this.messages = (
                await this.endpoints.message.getMessagesBetween(team)
            ).reverse();
            this.pollMessagesFrom(team);
        },
        resizeHeight(this: This, inputBox: HTMLTextAreaElement) {
            inputBox.style.height = `${TextAreaRowHeight}px`;
            inputBox.style.height = `${Math.min(
                5 * TextAreaRowHeight,
                inputBox.scrollHeight
            )}px`;
        },
        pollMessagesFrom(this: This, team: Team) {
            if (this.updateHandle !== undefined) {
                window.clearInterval(this.ui.updateHandle);
            }
            this.ui.updateHandle = window.setInterval(
                async () =>
                    (this.messages =
                        await this.endpoints.message.getMessagesBetween(team)),
                UpdateMessageIntervalMs
            );
        },
        async sendMessage(this: This) {
            const inputBox = this.$refs.inputBox as HTMLTextAreaElement;
            if (inputBox.value.trim().length > 0) {
                await this.endpoints.message.writeMessage(
                    this.ui.selectedTeam,
                    inputBox.value
                );
            }
        },
    },
    destroyed(this: This) {
        if (this.updateHandle !== undefined) {
            window.clearInterval(this.ui.updateHandle);
        }
    },
};
</script>

<style lang="scss" scoped>
@import "../assets/style.scss";
@import url("https://fonts.googleapis.com/css2?family=Tangerine:wght@400;700&display=swap");

$root-border-radius: 12px;
$root-border-width: 1px;

.root {
    width: 350px;
    height: 450px;

    display: grid;
    grid-template-rows: auto 1fr auto;

    border: $root-border-width solid $border-color;
    border-radius: $root-border-radius;
    background-color: $modal-background-color;
}

.team-tabs {
    width: 100%;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(0, 1fr));
}

.team-tab {
    display: flex;
    flex-direction: column;
    justify-content: center;
    padding: 8px;
    background: $background-color;
    border: $root-border-width solid $border-color;
    &:first-of-type {
        border-top-left-radius: $root-border-radius;
    }

    &:last-of-type {
        border-top-right-radius: $root-border-radius;
    }

    &:hover {
        background-color: $hover-color;
        cursor: pointer;
    }
}

.team-tab-selected {
    background-color: $button-color;
}

.messages {
    display: flex;
    flex-direction: column-reverse;
    overflow: scroll;
}

.message {
    width: -webkit-fill-available;
    width: -moz-available;
    padding: 12px;
    margin: 14px;
    z-index: inherit;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    background-color: $background-color;
    border: 1px solid $border-color;
    border-radius: 12px;
    overflow-anchor: none;

    &:last-child {
        overflow-anchor: auto;
    }
}

.input-area {
    width: -webkit-fill-available;
    width: -moz-available;
    display: flex;
    height: fit-content;
    overflow: auto;
    flex-direction: row;
    align-items: flex-end;
    justify-content: space-around;
    margin: 14px;
}

h2 {
    font-family: "Pirate", cursive;
    font-weight: 700;
    font-size: 2rem;
}

h3 {
    font-family: "Tangerine", cursive;
    font-style: normal;
    font-size: 1.2rem;
}

textarea {
    resize: none;
    width: 100%;
    margin-right: 12px;
}

.send-icon-container {
    padding: 12px;
    border-radius: 50%;
    padding: 5px;
    border: 1px solid $border-color;
    &:hover {
        background-color: $hover-color;
        cursor: pointer;
    }
}

.send-icon {
    width: 20px;
    height: 20px;
}
</style>
