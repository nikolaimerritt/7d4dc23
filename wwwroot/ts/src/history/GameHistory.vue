<template>
    <div>
        <div
            v-for="(history, index) in this.roundHistory"
            :key="`history-${index}`"
        >
            <span> Round {{ history.round.id }} </span>
            <div
                v-for="planning in describeRoundPlanning(history)"
                :key="`planning-${planning.id}`"
            >
                {{ planning }}
            </div>
            <div
                v-for="(outcomeDescription, index) in describeOutcomes(
                    history.outcomes
                )"
                :key="`outcome-description-${index}`"
            >
                {{ outcomeDescription }}
            </div>
        </div>
    </div>
</template>
<script lang="ts">
import { Move, MoveEndpoint } from "../endpoints/move";
import { Outcome, OutcomeEndpoint } from "../endpoints/outcome";
import { Purchase, PurchaseEndpoint } from "../endpoints/purchase";
import { Round, RoundEndpoint } from "../endpoints/round";
import { VueThis } from "../common/util";
import { Sea } from "../endpoints/sea";
import { Util } from "../common/util";

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
    async mounted(this: This) {
        await this.refreshHistory();
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
        describeRoundPlanning(
            this: This,
            roundHistory: RoundHistory
        ): string[] {
            interface PlanningDescription {
                event: Move | Purchase;
                description: string;
            }
            const purchaseDescriptions = roundHistory.purchases.map(
                (purchase) =>
                    ({
                        event: purchase,
                        description: this.describePurchase(purchase),
                    } as PlanningDescription)
            );
            const moveDescriptions = roundHistory.moves.map(
                (move) =>
                    ({
                        event: move,
                        description: this.describeMove(move),
                    } as PlanningDescription)
            );
            const planningDescriptions =
                purchaseDescriptions.concat(moveDescriptions);
            Util.sortByInPlace(
                planningDescriptions,
                (planning) => planning.event.creation
            );
            return planningDescriptions.map((planning) => planning.description);
        },
        describeMove(this: This, move: Move): string {
            return `${move.team.name} moved ${
                move.shipCount
            } ships from ${this.seaName(move.fromSea)} to ${this.seaName(
                move.toSea
            )}.`;
        },
        describePurchase(this: This, purchase: Purchase): string {
            return `${purchase.team.name} bought ${
                purchase.shipCount
            } ships in ${this.seaName(purchase.sea)}.`;
        },
        describeOutcomes(this: This, outcomes: Outcome[]): string[] {
            const seas = Util.uniqueByKey(
                outcomes.map((outcome) => outcome.sea),
                (sea) => sea.id
            );
            const descriptions: string[] = [];
            for (const sea of seas) {
                const outcomesInSea = outcomes.filter(
                    (outcome) => outcome.sea.id === sea.id
                );
                const winningOutcome = outcomesInSea.find(
                    (outcome) => outcome.shipCount > 0
                );
                const losingOutcomes = outcomesInSea.filter(
                    (outcome) => outcome.shipCount === 0
                );
                if (winningOutcome) {
                    if (losingOutcomes.length === 0) {
                        descriptions.push(
                            `${winningOutcome.team.name} holds ${this.seaName(
                                sea
                            )}.`
                        );
                    } else {
                        const losingTeamNames = this.joinWithAnd(
                            losingOutcomes.map((outcome) => outcome.team.name)
                        );
                        descriptions.push(
                            `${
                                winningOutcome.team.name
                            } conquered ${this.seaName(
                                sea
                            )}, beating ${losingTeamNames}.`
                        );
                    }
                } else {
                    descriptions.push(`${sea.name} remains unclaimed.`);
                }
            }
            return descriptions;
        },
        seaName(this: This, sea: Sea): string {
            if (sea.name === "Indian" || sea.name === "Southern") {
                return `the ${sea.name} ocean`;
            } else {
                return `the ${sea.name}`;
            }
        },
        joinWithAnd(strings: string[]): string {
            if (strings.length === 0) {
                return "";
            } else if (strings.length === 1) {
                return strings[0];
            } else {
                const tail = strings[strings.length - 1];
                const body = strings.slice(0, strings.length - 2);
                return `${body.join(", ")} and ${tail}`;
            }
        },
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.historyPollingHandle);
    },
};
</script>
