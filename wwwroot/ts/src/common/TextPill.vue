<template>
    <div class="pill">
        {{ text }}
    </div>
</template>
<script lang="ts">
interface Props {
    text: string;
}

export default {
    props: {
        text: String,
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
.pill {
    font-family: "Goudy Regular", sans-serif;
    border-radius: 20px;
    font-size: 16px;
    padding: 2px 8px;
    margin: 4px 0;
    background-color: $light-button-color;
    border: 1px solid $border-color;
}
</style>
