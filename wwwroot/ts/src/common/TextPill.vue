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
<style scoped>
.pill {
    font-family: "Goudy Regular", sans-serif;
    border-radius: 20px;
    font-size: 14px;
    padding: 2px 8px;
    margin: 4px 0;
    background-color: #f8ecbc;
    border: 1px solid #be9a67;
}
</style>
