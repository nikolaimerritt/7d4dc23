import { Connection } from "./main";
import { Sea } from "./sea";

export interface Team {
    id: number;
    name: string;
    startingSea: Sea;
}

export class TeamEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toTeam(object: any): Team {
        const team: Team = {
            id: object.id,
            name: object.name,
            startingSea: object.startingSea,
        };
        return team;
    }

    public async getTeam(): Promise<Team> {
        const response = await this.connection.get("teams/self");
        return TeamEndpoint.toTeam(response);
    }

    public async getAllTeams(): Promise<Team[]> {
        const response = await this.connection.get("teams");
        return response.map(TeamEndpoint.toTeam);
    }
}
