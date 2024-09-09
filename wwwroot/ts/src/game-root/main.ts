import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import TeamShip from "./TeamShip.vue";
import Common from "../common/main";
import { Seas, Icons } from "../assets/main";
import MessageBoard from "./MessageBoard.vue";
import VueCookies from "vue-cookies";
import MessageCard from "./MessageCard.vue";

Vue.use(VueCookies, { expires: "7d" });

Common.define();
Seas.define();
Icons.define();

Vue.component("sea-centre", SeaCentre);
Vue.component("game-root", GameRoot);
Vue.component("team-ship", TeamShip);
Vue.component("message-board", MessageBoard);
Vue.component("message-card", MessageCard);

new Vue({
    el: "#gameRoot",
});
