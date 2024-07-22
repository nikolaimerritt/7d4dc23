import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";

Vue.component("sea-centre", SeaCentre);
Vue.component("game-root", GameRoot);

new Vue({
    el: "#gameRoot",
});
