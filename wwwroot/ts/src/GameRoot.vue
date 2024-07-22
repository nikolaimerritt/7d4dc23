<template>
    <div>
        <img :src="'../../imgs/map-cropped.jpg'" class="map-background" />
    </div>
</template>

<script lang="ts">
export default {
    data() {
        return {
            clicks: 0,
        };
    },
    methods: {
        incrementClicks() {
            this.clicks++;
        },
    },
};
</script>
<style scoped>
.map-background {
    width: 90vw;
}
</style>
