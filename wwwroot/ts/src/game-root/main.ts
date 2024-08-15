import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import TeamShip from "./TeamShip.vue";
import Common from "../common/main";
import { Seas } from "../seas/main";
import { Icons } from "../icons/main";
import MessageBoard from "./MessageBoard.vue";

Common.define();
Seas.define();
Icons.define();

Vue.component("sea-centre", SeaCentre);
Vue.component("game-root", GameRoot);
Vue.component("team-ship", TeamShip);
Vue.component("message-board", MessageBoard);

new Vue({
    el: "#gameRoot",
});
