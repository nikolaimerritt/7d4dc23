import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import RoundList from "./RoundList.vue";

Vue.component("sea-centre", SeaCentre);
Vue.component("round-list", RoundList);
Vue.component("game-root", GameRoot);

new Vue({
    el: "#gameRoot",
});
