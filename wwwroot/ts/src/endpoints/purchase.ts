import { Connection, Fallible } from "./main";
import { Round, RoundEndpoint } from "./round";
import { Sea, SeaEndpoint } from "./sea";
import { Team, TeamEndpoint } from "./team";

export interface Purchase {
    id: number;
    team: Team;
    round: Round;
    sea: Sea;
    points: number;
    shipCount: number;
    creation: Date;
}

export class PurchaseEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toPurchase(object: any): Purchase {
        const purchase: Purchase = {
            id: object.id,
            team: TeamEndpoint.toTeam(object.team),
            round: RoundEndpoint.toRound(object.round),
            sea: SeaEndpoint.toSea(object.sea),
            points: object.points,
            shipCount: object.shipCount,
            creation: new Date(object.creation),
        };
        return purchase;
    }

    public async getPurchases(round: Round): Promise<Purchase[]> {
        const response = await this.connection.get("purchases", {
            roundId: round.id,
        });
        return response.map(PurchaseEndpoint.toPurchase);
    }

    public async getBalance(): Promise<number> {
        const response = await this.connection.get("purchases/balance");
        return parseInt(response);
    }

    public async purchaseShips(
        sea: Sea,
        pointsToSpend: number
    ): Promise<Fallible> {
        return await this.connection.put("purchases", {
            seaId: sea.id,
            points: pointsToSpend,
        });
    }
}
