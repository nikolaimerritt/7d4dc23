export class Style {
    public static teamColour(teamName: string): string {
        switch (teamName) {
            case "Team Drake":
                return "#c37b39";
            case "Team Blackbeard":
                return "#ba5737";
            case "Team Kidd":
                return "#9b5c74";
            case "Team Read":
                return "#5a67a3";
            case "Team O'Malley":
                return "#708c56";
            default:
                return "#000000";
        }
    }
}
