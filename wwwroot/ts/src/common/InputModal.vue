<template>
    <modal-wrapper @clickOutside="$emit('clickOutside', $event)">
        <span class="message"> {{ message }} </span>
        <input v-model="inputText" />
        <span class="error-message">
            {{ errorMessage }}
        </span>
        <div class="submit-button">
            <text-button
                :text="buttonText"
                @buttonClick="emitSubmission()"
            ></text-button>
        </div>
    </modal-wrapper>
    <!-- <div class="modal-wrapper" @click="emitClickOutside($event)">
        <div ref="modalBox" class="modal-box">
            <span class="message"> {{ message }} </span>
            <input v-model="inputText" />
            <span class="error-message">
                {{ errorMessage }}
            </span>
            <div class="submit-button">
                <text-button
                    :text="buttonText"
                    @buttonClick="emitSubmission()"
                ></text-button>
            </div>
        </div>
    </div> -->
</template>

<script lang="ts">
import { Util, VueThis } from "./util";

const ClickOutsideEvent = "clickOutside";
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
.message {
    padding: 36px 12px 0 12px;
}

.error-message {
    height: 24px;
}

.submit-button {
    margin-bottom: 20px;
}
</style>
