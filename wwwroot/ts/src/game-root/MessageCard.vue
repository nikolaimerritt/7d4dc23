<template>
    <div
        class="root"
        :class="
            message.sender.id === recipient.id ? 'message-to' : 'message-from'
        "
    >
        <div class="message-header">
            <h2>{{ message.sender.name }}</h2>
            <div class="subheading">
                {{ formatMessageDate(message.creation) }}
            </div>
        </div>
        <div
            class="content"
            :class="truncateContent ? 'truncated-message' : 'full-message'"
        >
            {{ message.content }}
        </div>
        <div
            class="toggle-read-more subheading"
            v-show="contentTooLong"
            @click="onToggleReadMoreClick()"
        >
            {{ truncateContent ? "Read more" : "Read less" }}
        </div>
    </div>
</template>
<script lang="ts">
import Vue from "vue";
import { Message } from "../endpoints/message";
import * as moment from "moment";
import { VueThis } from "../common/util";
import { Team } from "../endpoints/team";

interface Data {
    contentTooLong: boolean;
    truncateContent: boolean;
}

interface Props {
    message: Message;
    recipient: Team;
}

type This = VueThis<Data & Props>;

const MaxContentLines = 4;
const MaxContentChars = 128;
export default {
    props: {
        message: {
            type: Object,
            required: true,
        },
        recipient: {
            type: Object,
            required: true,
        },
    },
    data(): Data {
        return {
            contentTooLong: false,
            truncateContent: false,
        };
    },
    mounted(this: This) {
        const linesInMessage = [...this.message.content.matchAll(/\n/g)].length;
        this.contentTooLong =
            linesInMessage > MaxContentLines ||
            this.message.content.length > MaxContentChars;
        if (this.contentTooLong) {
            this.truncateContent = true;
        }
    },
    methods: {
        onToggleReadMoreClick(this: This) {
            this.truncateContent = !this.truncateContent;
        },
        formatMessageDate(date: Date): string {
            const now = moment(new Date());
            const toFormat = moment(date);
            const daysDifference = now.diff(toFormat, "days");
            if (daysDifference < 1) {
                return toFormat.format("HH:mm");
            } else {
                const dateFormat = {
                    year: "numeric",
                    month: "short",
                    day: "numeric",
                    hour: "numeric",
                    minute: "numeric",
                } as const;
                return date.toLocaleDateString(undefined, dateFormat);
            }
        },
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
$message-horizontal-shift: 50px;
.root {
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

.message-header {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: flex-end;
    width: 100%;
    margin-bottom: 0.3rem;
}

h2 {
    font-family: "Pirate", cursive;
    font-weight: $pirate-font-weight;
    font-size: 1.7rem;
    margin: 0;
}

.subheading {
    font-style: normal;
    font-size: 0.75rem;
    margin: 0;
}

.message-to {
    margin-right: $message-horizontal-shift;
}

.message-from {
    margin-left: $message-horizontal-shift;
}

.content {
    overflow: hidden;
    overflow-wrap: anywhere;
    white-space: pre-line;
    max-height: 5em;
}

.content.full-message {
    max-height: none;
}

.toggle-read-more {
    cursor: pointer;
    padding-left: 2px;
}
</style>
