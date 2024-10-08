import Vue from "vue";
import GameRoot from "./GameRoot.vue";
import SeaCentre from "./SeaCentre.vue";
import TeamShip from "./TeamShip.vue";
import Common from "../common/main";
import { Seas, Icons } from "../assets/main";
import MessageBoard from "./MessageBoard.vue";
import VueCookies from "vue-cookies";
import MessageBubble from "./MessageBubble.vue";
import Tutorial from "./Tutorial.vue";

Vue.use(VueCookies, { expires: "7d" });

Common.define();
Seas.define();
Icons.define();

Vue.component("tutorial", Tutorial);
Vue.component("sea-centre", SeaCentre);
Vue.component("team-ship", TeamShip);
Vue.component("message-board", MessageBoard);
Vue.component("message-bubble", MessageBubble);
Vue.component("game-root", GameRoot);

new Vue({
    el: "#gameRoot",
});
