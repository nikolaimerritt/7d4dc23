"use strict";
exports.__esModule = true;
var vue_1 = require("vue");
var SeaMap_vue_1 = require("./SeaMap.vue");
vue_1["default"].component("sea-map", SeaMap_vue_1["default"]);
new vue_1["default"]({
    el: "#seaMap"
});
