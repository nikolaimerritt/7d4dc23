<template>
    <div
        class="root"
        :class="
            message.sender.id === recipient.id ? 'message-to' : 'message-from'
        "
    >
        <div class="message-header">
            <h2>{{ message.sender.name }}</h2>
            <h3>{{ formatMessageDate(message.creation) }}</h3>
        </div>
        <div class="content">
            {{ message.content }}
        </div>
    </div>
</template>
<script lang="ts">
import Vue from "vue";
import { Message } from "../endpoints/message";
import * as moment from "moment";

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
    methods: {
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

h3 {
    font-style: normal;
    font-size: 0.7rem;
    margin: 0;
}

.message-to {
    margin-right: $message-horizontal-shift;
}

.message-from {
    margin-left: $message-horizontal-shift;
}

.content {
    overflow-wrap: anywhere;
    white-space: pre-line;
}
</style>
