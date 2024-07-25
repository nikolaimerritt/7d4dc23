import { Connection } from "./main";

export interface Round {
    id: number;
    startMoving: Date;
    startFighting: Date;
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
            startMoving: new Date(object.startMoving),
            startFighting: new Date(object.startFighting),
            end: new Date(object.end),
        };
        console.log("toRound", object, round);
        return round;
    }

    public async getRounds(): Promise<Round[]> {
        const response = await this.connection.get("rounds");
        return response.map(RoundEndpoint.toRound);
    }

    public async getCurrentRound(): Promise<Round> {
        return await RoundEndpoint.toRound(
            await this.connection.get("rounds/current")
        );
    }
}
