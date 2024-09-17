export class Style {
    public static teamColour(teamName: string): string {
        switch (teamName) {
            case "Team Drake":
                return "#d8822e";
            case "Team Blackbeard":
                return "#d14f31";
            case "Team Kidd":
                return "#9b5c74";
            case "Team Read":
                return "#5a67a3";
            case "Team Morgan":
                return "#708c56";
            default:
                return "#000000";
        }
    }
}
