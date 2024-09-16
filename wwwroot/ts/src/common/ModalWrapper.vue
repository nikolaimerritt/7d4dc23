<template>
    <div class="modal-wrapper" @click="emitClickOutside($event)">
        <div ref="modalBox" class="modal-box">
            <slot></slot>
        </div>
    </div>
</template>
<script lang="ts">
import { Util, VueThis } from "./util";

type This = VueThis<{}>;
const ClickOutsideEvent = "clickOutside";
export default {
    methods: {
        emitClickOutside(this: This, event: PointerEvent) {
            if (!Util.isHtmlElementRef(this.$refs.modalBox)) {
                return;
            }
            if (!this.$refs.modalBox.contains(event.target as Node)) {
                this.$emit(ClickOutsideEvent, true);
            }
        },
    },
};
</script>
<style lang="scss" scoped>
@import "../assets/style.scss";
.modal-wrapper {
    position: fixed;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: $modal-z-index;
    display: table-cell;
    vertical-align: middle;
}

.modal-box {
    position: relative;
    top: 50%;
    left: 50%;
    width: 300px;
    height: fill-content;
    min-height: 200px;
    margin: -150px 0 0 -150px;
    border-radius: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background-color: $modal-background-color;
    border: 1px solid $border-color;
    text-align: center;
    gap: 10px;
}
</style>
