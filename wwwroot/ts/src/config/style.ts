export class Style {
    public static teamColour(teamName: string): string {
        switch (teamName) {
            case "Team Drake":
                return "#ad847e";
            case "Team Blackbeard":
                return "#c38988";
            case "Team Kidd":
                return "#d19e8c";
            case "Team Read":
                return "#b09a80";
            case "Team O'Malley":
                return "#b3aa92";
            default:
                return "#000000";
        }
    }
}
