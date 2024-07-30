import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import RoundList from "./RoundList.vue";
import TeamShip from "./TeamShip.vue";
import InputModal from "./InputModal.vue";
import TextButton from "./TextButton.vue";

Vue.component("sea-centre", SeaCentre);
Vue.component("round-list", RoundList);
Vue.component("game-root", GameRoot);
Vue.component("team-ship", TeamShip);
Vue.component("input-modal", InputModal);
Vue.component("text-button", TextButton);

new Vue({
    el: "#gameRoot",
});
