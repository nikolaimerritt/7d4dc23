import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import RoundList from "./RoundList.vue";
import TeamShip from "./TeamShip.vue";

Vue.component("sea-centre", SeaCentre);
Vue.component("round-list", RoundList);
Vue.component("game-root", GameRoot);
Vue.component("team-ship", TeamShip);

new Vue({
    el: "#gameRoot",
});
