<template>
    <table>
        <tr class="header-row">
            <th>Rank</th>
            <th>Team</th>
            <th>Seas held</th>
        </tr>
        <tr
            class="body-row"
            v-for="(entry, index) in this.leaderboardEntries"
            :key="index"
        >
            <td>{{ entry.rank }}</td>
            <td>{{ entry.team.name }}</td>
            <td>{{ entry.seasHeld }}</td>
        </tr>
    </table>
</template>
<script lang="ts">
import {
    LeaderboardEndpoint,
    LeaderboardEntry,
} from "../endpoints/leaderboard";
import { VueThis } from "../common/util";
interface Data {
    endpoint: {
        leaderboard: LeaderboardEndpoint;
    };
    leaderboardEntries: LeaderboardEntry[];
}

type This = VueThis<Data>;

export default {
    data(): Data {
        return {
            endpoint: {
                leaderboard: new LeaderboardEndpoint(),
            },
            leaderboardEntries: [],
        };
    },
    async mounted(this: This) {
        this.leaderboardEntries =
            await this.endpoint.leaderboard.getLeaderboard();
        console.log("Leaderboard", this.leaderboardEntries);
    },
};
</script>
<style scoped></style>
