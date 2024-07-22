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

    public async getAllSeas(): Promise<Sea[]> {
        const response = await this.connection.get("seas");
        return response.map((item) => {
            const sea: Sea = {
                id: item.id,
                name: item.name,
                adjacentSeas: item.adjacentSeas.map((inner) => {
                    const adjacentSea: Sea = {
                        id: inner.id,
                        name: inner.name,
                        adjacentSeas: [],
                    };
                    return adjacentSea;
                }),
            };
            return sea;
        });
    }
}
