import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import TeamShip from "./TeamShip.vue";
import Common from "../common/main";

Common.define();

Vue.component("sea-centre", SeaCentre);
Vue.component("game-root", GameRoot);
Vue.component("team-ship", TeamShip);

new Vue({
    el: "#gameRoot",
});
