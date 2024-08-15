<template>
    <div class="message-container">
        <div class="message" v-for="(message, index) in messages" :key="index">
            <div class="sender">{{ message.sender.name }}</div>
            <div class="time">{{ message.creation }}</div>
            <div class="content">{{ message.content }}</div>
        </div>
    </div>
</template>
<script lang="ts">
import { VueThis } from "../common/util";
import { Message, MessageEndpoint } from "../endpoints/message";

interface Data {
    messages: Message[];
    messageEndpoint: MessageEndpoint;
    updateHandle: number | undefined;
}

type This = VueThis<Data>;

const UpdateMessageIntervalMs = 10_000;
export default {
    data(): Data {
        return {
            messages: [],
            messageEndpoint: new MessageEndpoint(),
            updateHandle: undefined,
        };
    },
    async mounted(this: This) {
        this.messages = await this.messageEndpoint.getMessages();
        this.updateHandle = window.setInterval(
            async () =>
                (this.messages = await this.messageEndpoint.getMessages()),
            UpdateMessageIntervalMs
        );
    },
    unmounted(this: This) {
        if (this.updateHandle !== undefined) {
            window.clearInterval(this.updateHandle);
        }
    },
};
</script>

<style scoped>
.message-container {
    width: 450px;
    height: 600px;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    align-items: center;
    border: 1px solid black;
    border-radius: 12px;
}

.message {
    display: flex;
    flex-direction: column;
    align-items: center;
}
</style>
