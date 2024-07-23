import { Connection } from "./main";
import { Sea } from "./sea";

export class PurchaseEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public async getBalance(): Promise<number> {
        const response = await this.connection.get("purchases/balance");
        return parseInt(response);
    }

    public async purchaseShips(sea: Sea, pointsToSpend: number) {
        await this.connection.put("purchases", {
            seaId: sea.id,
            points: pointsToSpend,
        });
    }
}
