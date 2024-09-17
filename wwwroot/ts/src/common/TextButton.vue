<template>
    <div class="button" :class="{ disabled: !enabled }" @click="onClick()">
        {{ text }}
    </div>
</template>
<script lang="ts">
import { VueThis } from "./util";

interface Props {
    text: string;
    enabled: boolean;
}

const ClickEvent = "buttonClick";
type This = VueThis<Props>;
export default {
    props: {
        text: String,
        enabled: Boolean,
    },
    methods: {
        onClick(this: This) {
            if (this.enabled) {
                this.$emit(ClickEvent);
            }
        },
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
.button {
    font-family: "Goudy Regular", sans-serif;
    border-radius: 20px;
    font-size: 16px;
    padding: 4px 8px 2px 8px;
    margin: 4px 0;
    cursor: pointer;
    border: 1px solid $border-color;

    &.disabled {
        cursor: default;
        background-color: $light-button-color;
    }
}

.button:not(.disabled):hover {
    background-color: $hover-color;
}

.button:not(hover) {
    background-color: $button-color;
}
</style>
