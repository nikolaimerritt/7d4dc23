import { Connection, Fallible } from "./main";
import { Round, RoundEndpoint } from "./round";
import { Sea, SeaEndpoint } from "./sea";
import { Team, TeamEndpoint } from "./team";

export interface Move {
    id: number;
    round: Round;
    team: Team;
    fromSea: Sea;
    toSea: Sea;
    shipCount: number;
    creation: Date;
}
export class MoveEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toMove(object: any): Move {
        const move: Move = {
            id: object.id,
            round: RoundEndpoint.toRound(object.round),
            team: TeamEndpoint.toTeam(object.team),
            fromSea: SeaEndpoint.toSea(object.fromSea),
            toSea: SeaEndpoint.toSea(object.toSea),
            shipCount: object.shipCount,
            creation: new Date(object.creation),
        };
        return move;
    }

    public async getMoves(round: Round): Promise<Move[]> {
        const response = await this.connection.get("moves", {
            roundId: round.id,
        });
        return response.map(MoveEndpoint.toMove);
    }

    public async moveShips(
        fromSea: Sea,
        toSea: Sea,
        shipCount: number
    ): Promise<Fallible> {
        return await this.connection.put("moves", {
            fromSeaId: fromSea.id,
            toSeaId: toSea.id,
            shipCount,
        });
    }

    public async canMove(): Promise<boolean> {
        return await this.connection.get("moves/can-move");
    }
}
