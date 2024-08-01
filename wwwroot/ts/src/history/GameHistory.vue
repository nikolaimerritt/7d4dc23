<template>
    <div class="container">
        <div
            v-for="(history, index) in this.roundHistory"
            :key="`history-${index}`"
        >
            <h2>Round {{ index + 1 }}</h2>
            <h3>
                {{ toTimeString(history.round.startMoving) }} to
                {{ toTimeString(history.round.end) }}
            </h3>
            <div
                v-if="
                    history.moves.length > 0 ||
                    history.purchases.length > 0 ||
                    history.outcomes.length > 0
                "
            >
                <ul>
                    <li
                        v-for="(planning, index) in describeRoundPlanning(
                            history
                        )"
                        :key="`planning-${index}`"
                    >
                        {{ planning }}
                    </li>
                    <li
                        v-for="(outcomeDescription, index) in describeOutcomes(
                            history.outcomes
                        )"
                        :key="`outcome-description-${index}`"
                    >
                        {{ outcomeDescription }}
                    </li>
                </ul>
            </div>
            <div v-else class="nothing-happened">Nothing yet has happened.</div>
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
import { Team } from "../endpoints/team";
import { Util } from "../common/util";
import * as moment from "moment";

interface RoundHistory {
    round: Round;
    moves: Move[];
    purchases: Purchase[];
    outcomes: Outcome[];
}

interface SeaOutcomes {
    sea: Sea;
    outcomes: Outcome[];
}

interface TeamShipsCount {
    team: Team;
    shipsBefore: number;
    shipsAfter: number;
}

interface Victory {
    sea: Sea;
    winner: TeamShipsCount;
    losers: TeamShipsCount[];
}

interface SeaShipCount {
    sea: Sea;
    shipCount: number;
}

interface Hold {
    holder: Team;
    seas: SeaShipCount[];
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
                historyPollingMs: 10_000,
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
            const descriptions: string[] = [];
            for (const victory of this.victories(outcomes) as Victory[]) {
                descriptions.push(
                    `${victory.winner.team.name} conquers ${this.seaName(
                        victory.sea
                    )} with ${
                        victory.winner.shipsBefore
                    } ships, beating ${this.joinWithAnd(
                        victory.losers.map(
                            (loser) =>
                                `${loser.team.name}'s ${loser.shipsBefore} ships`
                        )
                    )}. ${victory.winner.team.name} has ${
                        victory.winner.shipsAfter
                    } ships remaining.`
                );
            }
            const holds = Util.sortByInPlace(
                this.holds(outcomes) as Hold[],
                (hold) => hold.holder.name
            );
            for (const hold of holds) {
                descriptions.push(
                    `${hold.holder.name} holds ${this.joinWithAnd(
                        hold.seas.map(
                            (sea) =>
                                `${this.seaName(sea.sea)} with ${
                                    sea.shipCount
                                } ships`
                        )
                    )}.`
                );
            }
            Util.sortByInPlace(descriptions, (descrition) => descrition);
            return descriptions;
        },
        victories(outcomes: Outcome[]): Victory[] {
            const seaOutcomes = this.seaOutcomes(outcomes) as SeaOutcomes[];
            const seaVictories = seaOutcomes.filter(
                (sea) =>
                    sea.outcomes.some((outcome) => outcome.shipsAfter > 0) &&
                    sea.outcomes.some((outcome) => outcome.shipsAfter === 0)
            );
            return seaVictories.map((victory) => ({
                sea: victory.sea,
                winner: this.toTeamShipsCount(
                    victory.outcomes.find((outcome) => outcome.shipsAfter > 0)
                ),
                losers: victory.outcomes
                    .filter((outcome) => outcome.shipsAfter === 0)
                    .map((outcome) => this.toTeamShipsCount(outcome)),
            }));
        },
        holds(outcomes: Outcome[]): Hold[] {
            const teams = Util.uniqueByKey(
                outcomes.map((outcome) => outcome.team),
                (team) => team.id
            );
            const holds: Hold[] = [];
            for (const team of teams) {
                const teamOutcomes = outcomes.filter(
                    (outcome) => outcome.team.id === team.id
                );
                const teamHoldOutcomes = teamOutcomes.filter(
                    (outcome) =>
                        outcomes.filter(
                            (otherOutcome) =>
                                otherOutcome.sea.id === outcome.sea.id
                        ).length === 1
                );
                const seaShipCounts: SeaShipCount[] = [];
                for (const teamHoldOutcome of teamHoldOutcomes) {
                    seaShipCounts.push({
                        sea: teamHoldOutcome.sea,
                        shipCount: teamHoldOutcome.shipsAfter,
                    });
                }
                if (seaShipCounts.length > 0) {
                    Util.sortByInPlace(
                        seaShipCounts,
                        (seaShipCount) => seaShipCount.sea.name
                    );
                    holds.push({
                        holder: team,
                        seas: seaShipCounts,
                    });
                }
            }
            return holds;
        },
        toTeamShipsCount(outcome: Outcome): TeamShipsCount {
            return {
                team: outcome.team,
                shipsBefore: outcome.shipsBefore,
                shipsAfter: outcome.shipsAfter,
            };
        },
        seaOutcomes(outcomes: Outcome[]): SeaOutcomes[] {
            const seas = Util.uniqueByKey(
                outcomes.map((outcome) => outcome.sea),
                (sea) => sea.id
            );
            return seas.map((sea) => ({
                sea,
                outcomes: outcomes.filter(
                    (outcome) => outcome.sea.id === sea.id
                ),
            }));
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
                const body = strings.slice(0, strings.length - 1);
                return `${body.join(", ")} and ${tail}`;
            }
        },
        toTimeString(date: Date): string {
            return moment(date).format("HH:mm");
        },
    },
    unmounted(this: This) {
        window.clearInterval(this.ui.historyPollingHandle);
    },
};
</script>
<style scoped>
.container {
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}

h2 {
    color: #2c2215;
    margin-bottom: 0;
    margin: 2rem 0 0 0;
}

h3 {
    color: black;
    font-size: 0.8rem;
    font-style: italic;
}

h3,
li,
.nothing-happened {
    font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}
.nothing-happened {
    font-style: italic;
}
</style>
