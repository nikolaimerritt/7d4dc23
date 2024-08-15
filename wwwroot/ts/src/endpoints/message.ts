import { Connection, Fallible } from "./main";
import { Team, TeamEndpoint } from "./team";

export interface Message {
    id: number;
    sender: Team;
    recipient: Team;
    content: string;
    read: boolean;
    creation: Date;
}

export class MessageEndpoint {
    private connection: Connection;

    public constructor() {
        this.connection = new Connection();
    }

    public static toMessage(object: any): Message {
        const message: Message = {
            id: object.id,
            sender: TeamEndpoint.toTeam(object.sender),
            recipient: TeamEndpoint.toTeam(object.recipient),
            content: object.content,
            read: object.read,
            creation: new Date(object.creation),
        };
        return message;
    }

    public async getMessagesBetween(team: Team): Promise<Message[]> {
        const response = await this.connection.get("messages", {
            withTeamId: team.id,
        });
        return response.map(MessageEndpoint.toMessage);
    }

    public async writeMessage(
        recipient: Team,
        content: string
    ): Promise<Fallible> {
        return await this.connection.postText(
            "messages",
            { toTeamId: recipient.id },
            content
        );
    }

    public async markMessagesAsRead(messages: Message[]): Promise<Fallible> {
        return await this.connection.post(
            "messages/read",
            messages.map((message) => message.id)
        );
    }
}
