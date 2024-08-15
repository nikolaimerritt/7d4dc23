<template>
    <div class="button" @click="$emit(clickEvent)">
        {{ text }}
    </div>
</template>
<script lang="ts">
import { VueThis } from "./util";

interface Props {
    text: string;
}

type This = VueThis<Props>;
export default {
    props: {
        text: String,
    },
    data() {
        return {
            clickEvent: "buttonClick",
        };
    },
};
</script>
<style lang="scss" scoped>
@import "../../../css/variables.scss";
.button {
    font-family: "Goudy Regular", sans-serif;
    border-radius: 20px;
    font-size: 16px;
    padding: 2px 8px;
    margin: 4px 0;
    cursor: pointer;
    border: 1px solid $border-color;
}

.button:hover {
    background-color: $hover-color;
}

.button:not(hover) {
    background-color: $button-color;
}
</style>
