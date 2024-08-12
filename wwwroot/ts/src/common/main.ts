import Vue from "vue";
import InputModal from "../common/InputModal.vue";
import TextButton from "../common/TextButton.vue";
import MonsterRow from "../common/MonsterRow.vue";
import TextPill from "../common/TextPill.vue";

export default class Common {
    public static define() {
        Vue.component("input-modal", InputModal);
        Vue.component("text-button", TextButton);
        Vue.component("monster-row", MonsterRow);
        Vue.component("text-pill", TextPill);
    }
}
