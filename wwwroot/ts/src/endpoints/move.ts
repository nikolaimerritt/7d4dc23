import { Connection } from "./main";
import { Sea } from "./sea";

export class MoveEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public async moveShips(fromSea: Sea, toSea: Sea, shipCount: number) {
        await this.connection.put("moves", {
            fromSeaId: fromSea.id,
            toSeaId: toSea.id,
            shipCount,
        });
    }
}
