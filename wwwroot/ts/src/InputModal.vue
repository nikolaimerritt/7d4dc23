<template>
    <div class="modal-wrapper" @click="emitClickOutside($event)">
        <div ref="modalBox" class="modal-box">
            <span> {{ message }} </span>
            <input v-model="inputText" />
            <text-button
                :text="buttonText"
                @buttonClick="emitSubmission()"
            ></text-button>
            <span v-show="errorMessage"> {{ errorMessage }} </span>
        </div>
    </div>
</template>

<script lang="ts">
import { VueThis } from "./util";

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
            if (!this.$refs.modalBox.contains(event.target as any)) {
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
    z-index: 20;
    display: table-cell;
    vertical-align: middle;
}

.modal-box {
    position: relative;
    top: 50%;
    left: 50%;
    width: 300px;
    height: 200px;
    margin-left: -150px;
    margin-right: -100px;
    border-radius: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background-color: #fff4c7;
    text-align: center;
    gap: 10px;
}
</style>
