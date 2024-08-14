<template>
    <div class="modal-wrapper" @click="emitClickOutside($event)">
        <div ref="modalBox" class="modal-box">
            <span class="message"> {{ message }} </span>
            <input v-model="inputText" />
            <text-button
                style="margin: 24px 0"
                :text="buttonText"
                @buttonClick="emitSubmission()"
            ></text-button>
            <span class="error-message" v-show="errorMessage">
                {{ errorMessage }}
            </span>
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
<style scoped>
.modal-wrapper {
    position: fixed;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 40;
    display: table-cell;
    vertical-align: middle;
}

.message {
    padding: 24px 12px 0 12px;
}

.error-message {
    padding: 0 0 12px 0;
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
    background-color: #fff4c7;
    border: 1px solid #b18854;
    text-align: center;
    gap: 10px;
}
</style>
