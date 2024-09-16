<template>
    <div>
        <div
            class="message-button"
            title="Messages"
            @click="toggleShowMessages()"
        >
            <circle-icon
                class="notification"
                style="transform: translate(0%, -60%)"
                v-if="ui.teamIdsWithNotifications.length > 0"
            ></circle-icon>
            <quill-icon class="message-icon"> </quill-icon>
        </div>
        <div class="messages-root" v-show="ui.visibility === 'show'">
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
                    <circle-icon
                        class="notification"
                        style="transform: translate(40%, -120%)"
                        v-if="ui.teamIdsWithNotifications.includes(team.id)"
                    ></circle-icon>
                    {{ team.name }}
                </div>
            </div>
            <div class="messages" ref="messages">
                <div v-if="messages.length > 0">
                    <message-bubble
                        v-for="(message, index) in messages"
                        :key="index"
                        :message="message"
                        :recipient="thisTeam"
                    ></message-bubble>
                </div>
                <div v-else class="no-messages-container">
                    <span> No messages yet. Start a conversation! </span>
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
import { Connection } from "../endpoints/main";

interface Data {
    endpoints: {
        message: MessageEndpoint;
        team: TeamEndpoint;
    };
    ui: {
        visibility: "show" | "loading" | "hide";
        updateMessagesHandle?: number;
        updateNotificationsHandle?: number;
        selectedTeam?: Team;
        teamIdsWithNotifications: number[];
    };
    thisTeam?: Team;
    otherTeams: Team[];
    messages: Message[];
}

type This = VueThis<Data>;

const UpdateMessageIntervalMs = 10_000;
const MinInputHeightPx = 50;
const MaxInputHeightPx = 120;
const LastTeamOpenedCookie = "last-team-opened";
export default {
    data(): Data {
        return {
            endpoints: {
                message: new MessageEndpoint(),
                team: new TeamEndpoint(),
            },
            ui: {
                visibility: "hide",
                updateMessagesHandle: undefined,
                updateNotificationsHandle: undefined,
                selectedTeam: undefined,
                teamIdsWithNotifications: [],
            },
            thisTeam: undefined,
            otherTeams: [],
            messages: [],
        };
    },
    async mounted(this: This) {
        this.ui.updateNotificationsHandle = await Util.doAndRepeat(
            () => this.updateNotifications(),
            UpdateMessageIntervalMs
        );
    },
    methods: {
        async toggleShowMessages(this: This) {
            if (this.ui.visibility === "show") {
                this.ui.visibility = "hide";
            } else if (this.ui.visibility === "hide") {
                await this.loadMessageContainer();
            }
        },
        async loadMessageContainer(this: This) {
            this.ui.visibility = "loading";
            this.thisTeam = await this.endpoints.team.getTeam();
            const allTeams = await this.endpoints.team.getAllTeams();
            this.otherTeams = allTeams.filter(
                (team) => team.id !== this.thisTeam.id
            );

            if (Util.isHtmlElementRef(this.$refs.inputBox)) {
                const inputBox = this.$refs.inputBox as HTMLTextAreaElement;
                this.$refs.inputBox.addEventListener("input", () =>
                    this.resizeHeight(inputBox)
                );
            }

            const lastTeamOpenedId = parseInt(
                Util.getCookie(LastTeamOpenedCookie)
            );
            let lastTeamOpened = this.otherTeams[0];
            if (lastTeamOpenedId !== undefined && !isNaN(lastTeamOpenedId)) {
                lastTeamOpened =
                    this.otherTeams.find(
                        (team) => team.id === lastTeamOpenedId
                    ) ?? lastTeamOpened;
            }
            await this.onTeamTabClick(lastTeamOpened);
            this.ui.visibility = "show";
        },
        async onTeamTabClick(this: This, team: Team) {
            this.ui.selectedTeam = team;
            Util.setCookie(LastTeamOpenedCookie, `${team.id}`);
            if (this.ui.updateMessagesHandle !== undefined) {
                window.clearInterval(this.ui.updateMessagesHandle);
            }
            this.ui.updateMessagesHandle = await Util.doAndRepeat(
                () => this.updateMessages(team),
                UpdateMessageIntervalMs
            );
        },
        resizeHeight(this: This, inputBox: HTMLTextAreaElement) {
            inputBox.style.height = `${Util.clamp(
                MinInputHeightPx,
                inputBox.scrollHeight,
                MaxInputHeightPx
            )}px`;
        },
        async sendMessage(this: This) {
            const inputBox = this.$refs.inputBox as HTMLTextAreaElement;
            if (inputBox.value.trim().length > 0) {
                const response = await this.endpoints.message.writeMessage(
                    this.ui.selectedTeam,
                    inputBox.value
                );
                if (!Connection.isError(response)) {
                    this.updateMessages(this.ui.selectedTeam);
                    inputBox.value = "";
                }
            }
        },
        async updateMessages(this: This, team: Team) {
            this.messages = await this.endpoints.message.getMessagesBetween(
                team
            );
            await this.endpoints.message.markMessagesAsRead(this.messages);
            await this.updateNotifications();
        },
        async updateNotifications(this: This) {
            const notifications =
                await this.endpoints.message.getUnreadNotifications();
            this.ui.teamIdsWithNotifications = notifications.map(
                (notification) => notification.sender.id
            );
        },
    },
    destroyed(this: This) {
        for (const handle in [
            this.ui.updateMessagesHandle,
            this.ui.updateNotificationsHandle,
        ]) {
            window.clearInterval(handle);
        }
    },
};
</script>

<style lang="scss" scoped>
@import "../assets/style.scss";
@import url("https://fonts.googleapis.com/css2?family=Tangerine:wght@400;700&display=swap");

$root-border-radius: 12px;
$root-border-width: 1px;
$message-horizontal-shift: 50px;

.messages-root {
    position: fixed;
    bottom: $bottom-buttons-offset;
    right: 250px;
    z-index: $message-board-z-index;
    width: calc(350px + $message-horizontal-shift);
    height: 450px;

    display: grid;
    grid-template-rows: auto 1fr auto;

    border: $root-border-width solid $border-color;
    border-radius: $root-border-radius;
    background-color: $modal-background-color;
}

.message-icon {
    color: $font-color;
    width: 40px;
    height: 40px;
}

.message-button {
    position: fixed;
    display: inline-block;
    padding: 12px;
    bottom: $bottom-buttons-offset;
    right: 140px;
    z-index: $message-button-z-index;
    border-radius: 50%;
    border: 2px solid $border-color;
    background: $foreground-color;

    &:hover {
        background: $hover-color;
        cursor: pointer;
    }
}

.team-tabs {
    width: 100%;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(0, 1fr));
}

.team-tab {
    position: relative;
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
    overflow-y: scroll;
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

.notification {
    color: $notification-color;
    position: absolute;
    width: 20px;
    height: 20px;
    right: 0;
    z-index: $notification-z-index;
}

.send-icon {
    width: 20px;
    height: 20px;
}

.no-messages-container {
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}
</style>
