import Vue from "vue";
import InputModal from "../common/InputModal.vue";
import TextButton from "../common/TextButton.vue";

export default class Common {
    public static define() {
        Vue.component("input-modal", InputModal);
        Vue.component("text-button", TextButton);
    }
}
