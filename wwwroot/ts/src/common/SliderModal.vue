<template>
    <modal-wrapper @clickOutside="$emit('clickOutside', $event)">
        <h3>{{ title }}</h3>
        <span class="message"> {{ message }} </span>
        <div class="slider-container">
            <span class="slider-label"> {{ slider.min }} </span>
            <input
                type="range"
                :min="slider.min"
                :step="slider.step"
                :max="slider.max"
                v-model="selectedNumber"
            />
            <span class="slider-label"> {{ slider.max }} </span>
        </div>
        <span class="error-message">
            {{ errorMessage }}
        </span>
        <div class="submit-button">
            <text-button
                :text="buttonText"
                :enabled="selectedNumber > slider.min"
                @buttonClick="emitSubmission()"
            ></text-button>
        </div>
    </modal-wrapper>
</template>

<script lang="ts">
import { Util, VueThis } from "./util";

const SubmissionEvent = "submission";
const InputChangeEvent = "inputChange";

interface Slider {
    min: number;
    step: number;
    max: number;
}

interface Props {
    message: string;
    buttonText: string;
    errorMessage: string;
    slider: Slider;
}

interface Data {
    selectedNumber: number;
}

type This = VueThis<Data & Props>;
export default {
    props: {
        title: String,
        message: String,
        buttonText: String,
        errorMessage: String,
        slider: Object,
    },
    data(): Data {
        return {
            selectedNumber: 0,
        };
    },
    methods: {
        emitSubmission(this: This) {
            this.$emit(SubmissionEvent, this.selectedNumber);
        },
    },
    watch: {
        selectedNumber(this: This, newSelectedNumber) {
            this.$emit(InputChangeEvent, newSelectedNumber);
        },
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
h3 {
    padding-top: 30px;
}

.slider-container {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-content: center;
}

.slider-label {
    padding: 0 8px;
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
