<template>
    <div v-if="show">
        <div v-for="(instruction, index) in instructions" :key="index">
            <modal-wrapper
                v-if="instructionIndex === index"
                @clickOutside="nextInstruction()"
            >
                <div class="tutorial-content">
                    <div class="tutorial-text">
                        <h3>{{ instruction.title }}</h3>
                        <p>{{ instruction.body }}</p>
                    </div>
                    <text-button
                        @buttonClick="nextInstruction()"
                        class="tutorial-button"
                        :enabled="true"
                        :text="
                            index + 1 < instructions.length ? 'Next' : 'Finish'
                        "
                    >
                    </text-button>
                </div>
            </modal-wrapper>
        </div>
    </div>
</template>
<script lang="ts">
import { VueThis } from "../common/util";
interface Instruction {
    title: string;
    body: string;
}

const Instructions: Instruction[] = [
    {
        title: "Welcome to Pirate Conquest!",
        body: "In this swashbuckling turn-based game, the goal is to take control of the seven seas.",
    },
    {
        title: "Rounds",
        body: "In each round, you can spend points earned through solving flags to purchase pirate ships. You can also move ships that you already own.",
    },
    {
        title: "Combat",
        body: "At the end of a round, teams that are in the same sea battle it out. The team has the most amount of ships wins for that round. No other ships survive.",
    },
    {
        title: "Leaderboard",
        body: "At the end of a round, teams that have won control over a sea earn points on the leaderboard.",
    },
    {
        title: "Captain's Log",
        body: "The Captain's log records what happened in previous rounds, as well as when upcoming rounds will start.",
    },
    {
        title: "Messaging",
        body: "You can message other teams using the Captain's Quill below the map. Enjoy, and may the best pirate win!",
    },
];
interface Data {
    instructions: Instruction[];
    instructionIndex: number;
}

type This = VueThis<Data>;
export default {
    data(): Data {
        return {
            instructions: Instructions,
            instructionIndex: 0,
        };
    },
    props: {
        show: Boolean,
    },
    methods: {
        nextInstruction(this: This) {
            this.instructionIndex =
                (this.instructionIndex + 1) % this.instructions.length;
            if (this.instructionIndex === 0) {
                this.$emit("completed");
            }
        },
    },
};
</script>
<style lang="scss" scoped>
.tutorial-content {
    height: 200px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
}

.tutorial-text {
    height: 150px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: flex-start;
}

h3 {
    margin-top: 24px;
}

p {
    margin: 0 16px;
}

.tutorial-button {
    margin-bottom: 16px;
}
</style>
