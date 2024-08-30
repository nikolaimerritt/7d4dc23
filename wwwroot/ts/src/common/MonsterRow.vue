<template>
    <div class="monster-row">
        <img
            v-for="(monster, index) in monsters"
            :key="index"
            class="monster"
            :src="`../../imgs/monsters/${monster.file}`"
            :style="{
                transform: monster.mirrored ? 'scale(-1, 1)' : '',
            }"
        />
    </div>
</template>

<script lang="ts">
import { Util, VueThis } from "./util";
interface Monster {
    file: string;
    mirrored: boolean;
}

const MonsterCount = 12;
const MonsterFiles = [
    "monster-1.png",
    "monster-2.png",
    "monster-3.png",
    "monster-4.png",
    "monster-5.png",
];

interface Data {
    monsters: Monster[];
}

type This = VueThis<Data>;

export default {
    data(): Data {
        return {
            monsters: [],
        };
    },
    mounted(this: This) {
        this.monsters = this.generateMonsters();
    },
    methods: {
        generateMonsters(this: This): Monster[] {
            let monsters: Monster[] = [];
            while (monsters.length < MonsterCount) {
                const filesCopy = [...MonsterFiles];
                Util.shuffleInPlace(filesCopy);
                if (monsters.length > 0) {
                    while (monsters.at(-1).file === filesCopy[0]) {
                        Util.shuffleInPlace(filesCopy);
                    }
                }
                const newMonsters: Monster[] = filesCopy.map((file) => ({
                    file,
                    mirrored: Math.random() < 0.5,
                }));
                monsters = monsters.concat(newMonsters);
            }
            return monsters.slice(0, MonsterCount);
        },
    },
};
</script>

<style scoped>
.monster-row {
    position: absolute;
    bottom: 0;
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
