import { Connection, Fallible } from "./main";
import { Sea } from "./sea";

export class MoveEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
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
