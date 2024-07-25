import { Connection } from "./main";
import { Sea } from "./sea";
import { Team, TeamEndpoint } from "./team";
import { Round, RoundEndpoint } from "./round";

export interface Outcome {
    id: number;
    round: Round;
    team: Team;
    sea: Sea;
    shipCount: number;
}

export class OutcomeEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public async getLatestOutcomes(): Promise<Outcome[]> {
        const response = await this.connection.get("outcomes/virtual/latest");
        return response.map((item) => {
            const outcome: Outcome = {
                id: item.id,
                shipCount: item.shipCount,
                round: RoundEndpoint.toRound(item.round),
                team: TeamEndpoint.toTeam(item.team),
                sea: {
                    id: item.sea.id,
                    name: item.sea.name,
                    adjacentSeas: [],
                },
            };
            return outcome;
        });
    }
}
