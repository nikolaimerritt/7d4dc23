import { Connection } from "./main";

export interface Sea {
    id: number;
    name: string;
    adjacentSeas: Sea[];
}

export class SeaEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toSea(object: any): Sea {
        const sea: Sea = {
            id: object.id,
            name: object.name,
            adjacentSeas: object.adjacentSeas?.map((inner) => {
                const adjacentSea: Sea = {
                    id: inner.id,
                    name: inner.name,
                    adjacentSeas: [],
                };
                return adjacentSea;
            }),
        };
        return sea;
    }

    public async getAllSeas(): Promise<Sea[]> {
        const response = await this.connection.get("seas");
        return response.map(SeaEndpoint.toSea);
    }

    public async getAccessibleSeas(): Promise<Sea[]> {
        const response = await this.connection.get("seas/accessible");
        return response.map(SeaEndpoint.toSea);
    }
}
