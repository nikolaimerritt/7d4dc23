import Vue from "vue";
import GameLeaderboard from "./Leaderboard.vue";
import Common from "../common/main";

Vue.component("game-leaderboard", GameLeaderboard);
Common.define();

new Vue({
    el: "#leaderboard",
});
