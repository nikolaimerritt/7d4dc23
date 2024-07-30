<template>
    <div class="ship">
        <div class="icon-container">
            <img :src="'../../imgs/ship.png'" />
            <span class="ship-count"> {{ shipCount }} </span>
        </div>
        <div class="team-name">{{ teamName }}</div>
    </div>
</template>

<script lang="ts">
export default {
    props: {
        teamName: String,
        shipCount: Number,
    },
};
</script>
<style scoped>
.ship {
    display: flex;
    width: 60px;
    flex-direction: column;
    align-items: center;
    z-index: 30;
}
.icon-container {
    position: absolute;
    width: min-content;
}

.ship-count {
    position: relative;
    top: -45px;
    display: inline-block;
    width: 100%;
    text-align: center;
}
.team-name {
    margin-top: 60px;
    text-align: center;
    width: max-content;
}
img {
    width: 60px;
}
</style>
