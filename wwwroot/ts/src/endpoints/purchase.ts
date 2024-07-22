import { Connection } from "./main";

export class PurchaseEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public async getBalance(): Promise<number> {
        const response = await this.connection.get("purchases/balance");
        return parseInt(response);
    }
}
