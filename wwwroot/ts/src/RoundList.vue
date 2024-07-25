<template>
    <table v-if="rounds.length > 0">
        <tr>
            <td>Round start</td>
            <td>Round end</td>
        </tr>
        <tr v-for="round in rounds" :key="round.id">
            <td>{{ formateDate(round.startMoving) }}</td>
            <td>{{ formateDate(round.startFighting) }}</td>
        </tr>
    </table>
</template>

<script lang="ts">
import { Round } from "./endpoints/round";

export default {
    props: {
        rounds: {
            type: Array<Round>,
        },
    },
    methods: {
        formateDate(date: Date): string {
            return date.toLocaleTimeString();
        },
    },
};
</script>
