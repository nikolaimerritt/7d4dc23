<template>
    <div>
        <div class="message-board">
            <div class="team-tabs">
                <div
                    class="team-tab"
                    v-for="(team, index) in otherTeams"
                    :key="index"
                    @click="onTeamTabClick(team)"
                >
                    {{ team.name }}
                </div>
            </div>
        </div>
        <div class="message-container" v-show="loaded">
            <div
                class="message"
                v-for="(message, index) in messages"
                :key="index"
            >
                <h2>{{ message.sender.name }}</h2>
                <h3>{{ message.creation.toLocaleString() }}</h3>
                <div class="content">{{ message.content }}</div>
            </div>
            <div class="input-area">
                <textarea maxlength="250" ref="inputBox"></textarea>
                <div class="send-icon-container">
                    <quill-icon class="send-icon"></quill-icon>
                </div>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
import { Util, VueThis } from "../common/util";
import { Message, MessageEndpoint } from "../endpoints/message";
import { Team, TeamEndpoint } from "../endpoints/team";

interface Data {
    otherTeams: Team[];
    selectedTeam: Team | undefined;
    messages: Message[];
    messageEndpoint: MessageEndpoint;
    teamEndpoint: TeamEndpoint;
    updateHandle: number | undefined;
    loaded: boolean;
}

type This = VueThis<Data>;

const UpdateMessageIntervalMs = 10_000;
export default {
    data(): Data {
        return {
            otherTeams: [],
            selectedTeam: undefined,
            messages: [],
            messageEndpoint: new MessageEndpoint(),
            teamEndpoint: new TeamEndpoint(),
            updateHandle: undefined,
            loaded: false,
        };
    },
    async mounted(this: This) {
        const thisTeam = await this.teamEndpoint.getTeam();
        const allTeams = await this.teamEndpoint.getAllTeams();
        this.otherTeams = allTeams.filter((team) => team.id !== thisTeam.id);

        if (Util.isHtmlElementRef(this.$refs.inputBox)) {
            const inputBox = this.$refs.inputBox as HTMLTextAreaElement;
            this.$refs.inputBox.addEventListener("input", () =>
                this.resizeHeight(inputBox)
            );
        }
        this.loaded = true;
    },
    methods: {
        async onTeamTabClick(this: This, team: Team) {
            this.selectedTeam = team;
            this.messages = await this.messageEndpoint.getMessagesBetween(team);
            this.pollMessagesFrom(team);
        },
        resizeHeight(this: This, inputBox: HTMLTextAreaElement) {
            inputBox.style.height = "24px";
            inputBox.style.height = `${inputBox.scrollHeight}px`;
        },
        pollMessagesFrom(this: This, team: Team) {
            if (this.updateHandle !== undefined) {
                window.clearInterval(this.updateHandle);
            }
            this.updateHandle = window.setInterval(
                async () =>
                    (this.messages =
                        await this.messageEndpoint.getMessagesBetween(team)),
                UpdateMessageIntervalMs
            );
        },
    },
    destroyed(this: This) {
        if (this.updateHandle !== undefined) {
            window.clearInterval(this.updateHandle);
        }
    },
};
</script>

<style lang="scss" scoped>
@import "../assets/style.scss";
@import url("https://fonts.googleapis.com/css2?family=Tangerine:wght@400;700&display=swap");

.team-tabs {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: center;
}

.team-tab {
    width: 100%;
    background: $background-color;
    border: 1px solid $font-color;
}

.message-container {
    width: 350px;
    height: 450px;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    align-items: center;
    border: 1px solid $border-color;
    border-radius: 12px;
    background-color: $modal-background-color;
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

.input-area {
    width: -webkit-fill-available;
    width: -moz-available;
    display: flex;
    height: fit-content;
    flex-direction: row;
    align-items: flex-end;
    justify-content: space-around;
    margin: 14px;
}

.send-icon-container {
    padding: 12px;
    border-radius: 50%;
    padding: 5px;
    border: 1px solid $border-color;
}

.send-icon {
    width: 20px;
    height: 20px;
}
</style>
