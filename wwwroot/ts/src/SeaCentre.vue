<template>
    <div class="circle">
        <span> {{ this.name }} </span>
    </div>
</template>

<script lang="ts">
export default {
    props: {
        name: String,
    },
};
</script>

<style scoped>
.circle {
    background-color: red;
    border-radius: 30px;
}
</style>
