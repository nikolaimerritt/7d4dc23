<template>
<div>
    <div> Hello from the map! </div>
    <div> The button has been clicked {{ this.clicks }} times </div>
    <button v-on:click="this.incrementClicks"> Click me! </button>
</div>
</template>

<script lang="ts">

export default {
    data() {
        return {
            clicks: 0
        };
    },
    methods: {
        incrementClicks() {
            this.clicks++;
        }
    }
}

</script>