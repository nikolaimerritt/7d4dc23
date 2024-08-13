import { Connection } from "./main";
import { Round } from "./round";
import { Sea, SeaEndpoint } from "./sea";
import { Team, TeamEndpoint } from "./team";

export interface TeamShipCount {
    team: Team;
    shipCount: number;
}

export interface SeaState {
    sea: Sea;
    teamShips: TeamShipCount[];
}

export class SeaStateEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toSeaState(object: any): SeaState {
        return {
            sea: SeaEndpoint.toSea(object.sea),
            teamShips: object.teamShips.map(
                (entry) =>
                    ({
                        team: TeamEndpoint.toTeam(entry.team),
                        shipCount: entry.shipCount,
                    } as TeamShipCount)
            ),
        } as SeaState;
    }

    public async getSeaStates(): Promise<SeaState[]> {
        const response = await this.connection.get("sea-states");
        return response.map(SeaStateEndpoint.toSeaState);
    }
}
