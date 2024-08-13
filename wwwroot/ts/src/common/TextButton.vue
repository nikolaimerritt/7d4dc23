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
<style scoped>
.button {
    font-family: "Goudy Regular", sans-serif;
    border-radius: 20px;
    font-size: 16px;
    padding: 2px 8px;
    margin: 4px 0;
    cursor: pointer;
    border: 1px solid #816746;
}

.button:hover {
    background-color: #e1c795;
}

.button:not(hover) {
    background-color: #e4d39c;
}
</style>
