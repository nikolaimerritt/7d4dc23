import Vue from "vue";
import IndianSea from "./IndianSea.vue";
import SouthernSea from "./SouthernSea.vue";
import ArcticSea from "./ArcticSea.vue";
import NorthAtlanticSea from "./NorthAtlanticSea.vue";
import NorthPacificSea from "./NorthPacificSea.vue";
import SouthAtlanticSea from "./SouthAtlanticSea.vue";
import SouthPacificSea from "./SouthPacificSea.vue";
import QuillIcon from "./QuillIcon.vue";

export class Seas {
    public static define() {
        Vue.component("arctic-sea", ArcticSea);
        Vue.component("north-atlantic-sea", NorthAtlanticSea);
        Vue.component("north-pacific-sea", NorthPacificSea);
        Vue.component("south-atlantic-sea", SouthAtlanticSea);
        Vue.component("south-pacific-sea", SouthPacificSea);
        Vue.component("indian-sea", IndianSea);
        Vue.component("southern-sea", SouthernSea);
    }
}

export class Icons {
    public static define() {
        Vue.component("quill-icon", QuillIcon);
    }
}
