export class Style {
    public static teamColour(teamName: string): string {
        switch (teamName) {
            case "drake":
                return "#d8822e";
            case "blackbeard":
                return "#d14f31";
            case "kidd":
                return "#9b5c74";
            case "read":
                return "#5a67a3";
            case "morgan":
                return "#708c56";
            default:
                return "#000000";
        }
    }
}
