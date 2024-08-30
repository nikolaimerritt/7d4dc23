<template>
    <div class="modal-wrapper" @click="emitClickOutside($event)">
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
    </div>
</template>

<script lang="ts">
import { Util, VueThis } from "./util";

const ClickOutsideEvent = "clickOutside";
const SubmissionEvent = "submission";

interface Props {
    message: string;
    buttonText: string;
    errorMessage: string;
    show: boolean;
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
        emitClickOutside(this: This, event: PointerEvent) {
            if (!Util.isHtmlElementRef(this.$refs.modalBox)) {
                return;
            }
            if (!this.$refs.modalBox.contains(event.target as Node)) {
                this.$emit(ClickOutsideEvent, true);
            }
        },
        emitSubmission() {
            this.$emit(SubmissionEvent, this.inputText);
        },
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
.modal-wrapper {
    position: fixed;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: $modal-z-index;
    display: table-cell;
    vertical-align: middle;
}

.message {
    padding: 36px 12px 0 12px;
}

.error-message {
    height: 24px;
}

.modal-box {
    position: relative;
    top: 50%;
    left: 50%;
    width: 300px;
    height: fill-content;
    min-height: 200px;
    margin: -150px 0 0 -150px;
    border-radius: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background-color: $modal-background-color;
    border: 1px solid $border-color;
    text-align: center;
    gap: 10px;
}

.submit-button {
    margin-bottom: 20px;
}
</style>
