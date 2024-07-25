import { Connection } from "./main";
import { Team, TeamEndpoint } from "./team";

export interface LeaderboardEntry {
    team: Team;
    rank: number;
    seasHeld: number;
}

export class LeaderboardEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public async getLeaderboard(): Promise<LeaderboardEntry[]> {
        const response = await this.connection.get("leaderboard");
        return response.map((item) => {
            const leaderboardEntry: LeaderboardEntry = {
                team: TeamEndpoint.toTeam(item.team),
                seasHeld: item.seasHeld,
                rank: item.rank,
            };
            return leaderboardEntry;
        });
    }
}
