<template>
    <modal-wrapper @clickOutside="$emit('clickOutside', $event)">
        <h3>{{ title }}</h3>
        <span class="message"> {{ message }} </span>
        <input v-model="inputText" />
        <span class="error-message">
            {{ errorMessage }}
        </span>
        <div class="submit-button">
            <text-button
                :text="buttonText"
                :enabled="true"
                @buttonClick="emitSubmission()"
            ></text-button>
        </div>
    </modal-wrapper>
</template>

<script lang="ts">
import { Util, VueThis } from "./util";

const SubmissionEvent = "submission";

interface Props {
    message: string;
    buttonText: string;
    errorMessage: string;
}

interface Data {
    inputText: string;
}

type This = VueThis<Data & Props>;
export default {
    props: {
        title: String,
        message: String,
        buttonText: String,
        errorMessage: String,
    },
    data() {
        return {
            inputText: "",
        };
    },
    methods: {
        emitSubmission() {
            this.$emit(SubmissionEvent, this.inputText);
        },
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
h3 {
    padding-top: 30px;
}

.message {
    padding: 0 12px;
}

.error-message {
    height: 24px;
}

.submit-button {
    margin-bottom: 20px;
}
</style>
