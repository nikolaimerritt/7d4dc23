import Vue from "vue";
import GameHistory from "./GameHistory.vue";
import Common from "../common/main";

Vue.component("game-history", GameHistory);
Common.define();

new Vue({
    el: "#history",
});
