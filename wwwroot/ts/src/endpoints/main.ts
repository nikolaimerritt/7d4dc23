export type QueryParams = Record<
    string,
    string | number | null | undefined | boolean
>;

export type Ok = {};
export type Error = { error: string };
export type Result<T> = T | Error;
export type Fallible = Ok | Error;

export class Connection {
    public constructor() {}

    public static isError(response: Fallible): response is Error {
        return "error" in response;
    }

    public async get(
        endpoint: string,
        queryParams: QueryParams = {}
    ): Promise<any> {
        let url = this.apiUrl(endpoint);
        const searchParams = this.searchParams(queryParams).toString();
        if (searchParams.length > 0) {
            url += `?${searchParams}`;
        }
        const response = await fetch(url, {
            method: "GET",
            credentials: "same-origin",
            headers: this.jsonRequestHeaders(),
        });
        return response.json();
    }

    public async post(endpoint: string, body: object): Promise<any> {
        let url = this.apiUrl(endpoint);
        const response = await fetch(url, {
            method: "POST",
            credentials: "same-origin",
            headers: this.jsonRequestHeaders(),
            body: JSON.stringify(body),
        });
        return response.json();
    }

    public async put(endpoint: string, queryParams: QueryParams) {
        let url = this.apiUrl(endpoint);
        const searchParams = this.searchParams(queryParams).toString();
        if (searchParams.length > 0) {
            url += `?${searchParams}`;
        }
        const response = await fetch(url, {
            method: "PUT",
            credentials: "same-origin",
            headers: this.jsonRequestHeaders(),
        });
        return response.json();
    }

    private apiUrl(endpoint: string): string {
        return `/api/${endpoint}`;
    }
    private jsonRequestHeaders(): HeadersInit {
        return {
            "Content-Type": "application/json",
            // "X-CSRF-Token": this.getCsrfToken(),
        };
    }

    private searchParams(queryParams: QueryParams): URLSearchParams {
        const keysValues: string[][] = [];
        for (let [key, value] of Object.entries(queryParams)) {
            if (value !== null && value !== undefined) {
                keysValues.push([key, value.toString()]);
            }
        }
        return new URLSearchParams(keysValues);
    }

    private getCsrfToken(): string {
        return document
            .querySelector("meta[name=csrf-token]")
            .getAttribute("content");
    }
}
