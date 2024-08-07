<template>
    <div>
        <div
            class="background"
            :style="{
                background: 'url(../../../imgs/monster-a.png)',
                backgroundRepeat: 'tile',
                backgroundPosition: '50px 90px',
            }"
        ></div>
        <div class="centre-container">
            <div class="table-border">
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
                        <td>
                            <span
                                :style="{
                                    color: teamCircleColour(entry.team.name),
                                }"
                                >⬤</span
                            >
                            {{ entry.team.name }}
                        </td>
                        <td>{{ entry.seasHeld }}</td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
import {
    LeaderboardEndpoint,
    LeaderboardEntry,
} from "../endpoints/leaderboard";
import { VueThis } from "../common/util";
import { Style } from "../config/style";
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
    methods: {
        teamCircleColour(teamName: string): string {
            return Style.teamColour(teamName);
        },
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.leaderboardPollingHandle);
    },
};
</script>
<style scoped>
.background {
    position: absolute;
    top: 0;
    left: 0;
    z-index: -1;
    width: 100%;
    height: 100%;
    filter: brightness(0) saturate(100%) invert(70%) sepia(11%) saturate(1389%)
        hue-rotate(356deg) brightness(90%) contrast(83%) opacity(50%);
}

.centre-container {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: row;
    justify-content: center;
}

.table-border {
    height: fit-content;
    border: 4px solid #be9a67;
    background-color: #f8ecbc;
    padding: 16px 32px;
    margin-top: 10%;
    border-radius: 16px;
}

table {
    font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
    width: fit-content;
}

th {
    color: #2c2215;
    font-size: 2.5rem;
}

th,
td {
    padding-bottom: 0.8rem;
}

th:not(:last-child),
td:not(:last-child) {
    padding-right: 4.5rem;
}

td {
    font-size: 16px;
}
</style>
