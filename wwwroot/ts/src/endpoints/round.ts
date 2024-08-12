import { Connection } from "./main";

export interface Round {
    id: number;
    startPlanning: Date;
    startCooldown: Date;
    end: Date;
}

export class RoundEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toRound(object: any): Round {
        const round: Round = {
            id: object.id,
            startPlanning: new Date(object.startPlanning),
            startCooldown: new Date(object.startCooldown),
            end: new Date(object.end),
        };
        return round;
    }

    public async getRounds(): Promise<Round[]> {
        const response = await this.connection.get("rounds");
        return response.map(RoundEndpoint.toRound);
    }

    public async getCurrentRound(): Promise<Round | undefined> {
        var response = await this.connection.get("rounds/current");
        if (response) {
            return RoundEndpoint.toRound(response);
        } else {
            return undefined;
        }
    }
}
