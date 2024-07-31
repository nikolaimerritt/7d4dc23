<template>
    <div>
        <div v-for="(history, index) in this.roundHistory" :key="index">
            <span> {{ history.round.startMoving }} </span>
            <span> {{ JSON.stringify(history.moves) }} </span>
            <span> {{ JSON.stringify(history.purchases) }} </span>
            <span> {{ JSON.stringify(history.outcomes) }} </span>
        </div>
    </div>
</template>
<script lang="ts">
import { Move, MoveEndpoint } from "../endpoints/move";
import { Outcome, OutcomeEndpoint } from "../endpoints/outcome";
import { Purchase, PurchaseEndpoint } from "../endpoints/purchase";
import { Round, RoundEndpoint } from "../endpoints/round";
import { VueThis } from "../common/util";

interface RoundHistory {
    round: Round;
    moves: Move[];
    purchases: Purchase[];
    outcomes: Outcome[];
}
interface Data {
    roundHistory: RoundHistory[];
    endpoints: {
        move: MoveEndpoint;
        purchase: PurchaseEndpoint;
        outcome: OutcomeEndpoint;
        round: RoundEndpoint;
    };
    ui: {
        historyPollingMs: number;
        historyPollingHandle?: number;
    };
}

type This = VueThis<Data>;
export default {
    data(): Data {
        return {
            roundHistory: [],
            endpoints: {
                move: new MoveEndpoint(),
                purchase: new PurchaseEndpoint(),
                outcome: new OutcomeEndpoint(),
                round: new RoundEndpoint(),
            },
            ui: {
                historyPollingMs: 5_000,
                historyPollingHandle: undefined,
            },
        };
    },
    mounted(this: This) {
        this.ui.historyPollingHandle = window.setInterval(
            async () => await this.refreshHistory(),
            this.ui.historyPollingMs
        );
    },
    methods: {
        async refreshHistory(this: This) {
            for (const round of await this.endpoints.round.getRounds()) {
                const history = await this.getRoundHistory(round);
                const existingHistoryIndex = this.roundHistory.findIndex(
                    (existingHistory) => existingHistory.round.id === round.id
                );
                if (existingHistoryIndex === -1) {
                    this.roundHistory.push(history);
                } else {
                    this.roundHistory.splice(existingHistoryIndex, 1, history);
                }
            }
        },
        async getRoundHistory(this: This, round: Round): Promise<RoundHistory> {
            const history: RoundHistory = {
                round,
                moves: await this.endpoints.move.getMoves(round),
                purchases: await this.endpoints.purchase.getPurchases(round),
                outcomes: await this.endpoints.outcome.getOutcomes(round),
            };
            return history;
        },
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.historyPollingHandle);
    },
};
</script>
