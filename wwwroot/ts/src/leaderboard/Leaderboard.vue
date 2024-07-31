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
    ui: {
        leaderboardPollingMs: number;
        leaderboardPollingHandle?: number;
    };
}

type This = VueThis<Data>;

export default {
    data(): Data {
        return {
            endpoint: {
                leaderboard: new LeaderboardEndpoint(),
            },
            leaderboardEntries: [],
            ui: {
                leaderboardPollingMs: 5_000,
                leaderboardPollingHandle: undefined,
            },
        };
    },
    mounted(this: This) {
        this.ui.leaderboardPollingHandle = window.setInterval(
            async () =>
                (this.leaderboardEntries =
                    await this.endpoint.leaderboard.getLeaderboard()),
            this.ui.leaderboardPollingMs
        );
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.leaderboardPollingHandle);
    },
};
</script>
<style scoped></style>
