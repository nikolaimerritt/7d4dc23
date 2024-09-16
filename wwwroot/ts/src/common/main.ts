import Vue from "vue";
import InputModal from "./InputModal.vue";
import TextButton from "./TextButton.vue";
import MonsterRow from "./MonsterRow.vue";
import TextPill from "./TextPill.vue";
import ModalWrapper from "./ModalWrapper.vue";

export default class Common {
    public static define() {
        Vue.component("modal-wrapper", ModalWrapper);
        Vue.component("input-modal", InputModal);
        Vue.component("text-button", TextButton);
        Vue.component("monster-row", MonsterRow);
        Vue.component("text-pill", TextPill);
    }
}
