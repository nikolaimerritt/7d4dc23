import { Connection } from "./main";
import { Sea } from "./sea";

export interface Team {
    id: number;
    name: string;
    colourHexCode: string;
    startingSea: Sea;
}

export class TeamEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public async getTeam(): Promise<Team> {
        const response = await this.connection.get("teams/self");
        const team: Team = {
            id: response.id,
            name: response.name,
            colourHexCode: response.colourHexCode,
            startingSea: response.startingSea,
        };
        return team;
    }
}
