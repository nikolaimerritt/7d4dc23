<template>
    <div class="backgeound">
        <div class="container">
            <table>
                <tr>
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
        </div>
    </div>
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
                leaderboardPollingMs: 10_000,
                leaderboardPollingHandle: undefined,
            },
        };
    },
    async mounted(this: This) {
        this.leaderboardEntries =
            await this.endpoint.leaderboard.getLeaderboard();
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
<style scoped>
.backgeound {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: row;
    align-items: center;
}

.container {
    width: 60%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}

table {
    font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
    width: fit-content;
    margin: 3rem 0 0 0;
}

th {
    color: #2c2215;
    font-size: 2.5rem;
}

th,
td {
    padding: 0 3.5rem 0.8rem 0;
}

td {
    font-size: 16px;
}
</style>
