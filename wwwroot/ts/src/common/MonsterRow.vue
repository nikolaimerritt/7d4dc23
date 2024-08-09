<template>
    <div class="monster-row">
        <img
            v-for="(monsterFile, index) in monsterFiles"
            :key="index"
            class="monster"
            :src="`../../imgs/monsters/${monsterFile}`"
        />
    </div>
</template>

<script lang="ts">
import { Util, VueThis } from "./util";
const MonsterCount = 10;

const MonsterFiles = [
    "monster-1.png",
    "monster-2.png",
    "monster-3.png",
    "monster-4.png",
    "monster-5.png",
    "monster-6.png",
    "monster-7.png",
];

interface Data {
    monsterFiles: string[];
}

type This = VueThis<Data>;

export default {
    data(): Data {
        return {
            monsterFiles: [],
        };
    },
    mounted(this: This) {
        this.monsterFiles = this.generateMonsters();
    },
    methods: {
        generateMonsters(this: This): string[] {
            let monsters: string[] = [];
            while (monsters.length < MonsterCount) {
                const filesCopy = [...MonsterFiles];
                Util.shuffleInPlace(filesCopy);
                if (monsters.length > 0) {
                    while (monsters.at(-1) === filesCopy[0]) {
                        Util.shuffleInPlace(filesCopy);
                    }
                }
                monsters = monsters.concat(filesCopy);
            }
            console.log("Generated monsters", monsters);
            return monsters.slice(0, MonsterCount);
        },
    },
};
</script>

<style scoped>
.monster-row {
    position: absolute;
    bottom: 70px;
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
}

.monster {
    height: 65px;
    filter: brightness(0) saturate(100%) invert(70%) sepia(11%) saturate(1389%)
        hue-rotate(356deg) brightness(90%) contrast(83%) opacity(50%);
}
</style>
