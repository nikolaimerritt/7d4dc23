<template>
    <div class="circle">
        <!-- <span> {{ this.name }} </span> -->
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
    border-radius: 50%;
    background: red;
    width: 50px;
    height: 50px;
    margin-left: -25px;
    margin-top: -25px;
}
</style>
