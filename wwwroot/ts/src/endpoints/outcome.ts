import { Connection } from "./main";
import { Sea } from "./sea";
import { Team, TeamEndpoint } from "./team";
import { Round, RoundEndpoint } from "./round";

export interface Outcome {
    id: number;
    round: Round;
    team: Team;
    sea: Sea;
    shipsBefore: number;
    shipsAfter: number;
}

export class OutcomeEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toOutcome(object: any): Outcome {
        const outcome: Outcome = {
            id: object.id,
            shipsBefore: object.shipsBefore,
            shipsAfter: object.shipsAfter,
            round: RoundEndpoint.toRound(object.round),
            team: TeamEndpoint.toTeam(object.team),
            sea: {
                id: object.sea.id,
                name: object.sea.name,
                adjacentSeas: [],
            },
        };
        return outcome;
    }

    public async getOutcomes(round: Round): Promise<Outcome[]> {
        const response = await this.connection.get("outcomes", {
            roundId: round.id,
        });
        return response.map(OutcomeEndpoint.toOutcome);
    }

    public async getLatestOutcomes(): Promise<Outcome[]> {
        const response = await this.connection.get("outcomes/virtual/latest");
        return response.map(OutcomeEndpoint.toOutcome);
    }
}
