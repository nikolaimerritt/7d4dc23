import { Connection } from "./main";
import { Sea } from "./sea";
import { Team } from "./team";

export interface Round {
    id: number;
    startMoving: Date;
    startFighting: Date;
    end: Date;
}

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
        const response = await this.connection.get("outcomes/latest");
        return response.map((item) => {
            const outcome: Outcome = {
                id: item.id,
                shipCount: item.shipCount,
                round: {
                    id: item.round.id,
                    startMoving: item.round.startMoving,
                    startFighting: item.round.startFighting,
                    end: item.round.end,
                },
                team: {
                    id: item.team.id,
                    name: item.team.name,
                    colourHexCode: item.team.colourHexCode,
                    startingSea: item.team.startingSea,
                },
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
