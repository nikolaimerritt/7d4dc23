import * as moment from "moment";
import { Sea } from "../endpoints/sea";
import Cookies from "js-cookie";

export type VueComponentRef = { $el: HTMLElement };
export type VueRef = HTMLElement | VueComponentRef;

export type VueThis<Data> = Data & {
    $refs: { [refName: string]: VueRef };
} & { [functionName: string]: Function };

export class Util {
    private static CookiePrefix = "pirate-conquest" as const;

    public static isHtmlElementRef(ref: VueRef): ref is HTMLElement {
        return ref instanceof HTMLElement;
    }

    public static isComponentRef(ref: VueRef): ref is VueComponentRef {
        return "$el" in ref;
    }

    public static uniqueByKey<T>(
        items: T[],
        keySelector: (item: T) => any
    ): T[] {
        return [
            ...new Map(items.map((item) => [keySelector(item), item])).values(),
        ];
    }
    public static formatTimeBetween(bigger: Date, smaller: Date): string {
        const duration = moment.duration(moment(bigger).diff(moment(smaller)));
        return moment.utc(duration.asMilliseconds()).format("HH:mm:ss");
    }
    public static timeBetween(bigger: Date, smaller: Date): Date {
        if (bigger < smaller) {
            bigger = smaller;
        }
        const duration = moment.duration(moment(bigger).diff(moment(smaller)));
        return new Date(duration.asMilliseconds());
    }

    public static getHtmlObjectContent(htmlObject: HTMLElement): HTMLElement {
        return (htmlObject as any).contentDocument;
    }

    public static seaNameLowercase(sea: Sea): string {
        if (sea.name === "Indian" || sea.name === "Southern") {
            return `the ${sea.name} ocean`;
        } else {
            return `the ${sea.name}`;
        }
    }

    public static seaNameTitleCase(sea: Sea): string {
        if (sea.name === "Indian" || sea.name === "Southern") {
            return `The ${sea.name} ocean`;
        } else {
            return `The ${sea.name}`;
        }
    }

    public static maxBy<T>(
        list: T[],
        score: (element: T) => number
    ): T | undefined {
        if (list.length === 0) {
            return undefined;
        } else {
            let largest = list[0];
            let largestScore = score(largest);
            for (let i = 1; i < list.length; i++) {
                const currentScore = score(list[i]);
                if (currentScore > largestScore) {
                    largestScore = currentScore;
                    largest = list[i];
                }
            }
            return largest;
        }
    }

    public static minBy<T>(
        list: T[],
        score: (element: T) => number
    ): T | undefined {
        return Util.maxBy(list, (element) => -1 * score(element));
    }

    public static async sleep(milliseconds: number): Promise<void> {
        await new Promise((resolve) =>
            window.setTimeout(resolve, milliseconds)
        );
    }
    public static randomNumber(seed: number): number {
        seed |= 0;
        seed = (seed + 0x9e3779b9) | 0;
        let t = seed ^ (seed >>> 16);
        t = Math.imul(t, 0x21f0aaad);
        t = t ^ (t >>> 15);
        t = Math.imul(t, 0x735a2d97);
        return ((t = t ^ (t >>> 15)) >>> 0) / 4294967296;
    }

    public static range(count: number): number[] {
        return [...Array(count).keys()];
    }

    public static shuffleInPlace<T>(items: T[]): void {
        for (let i = items.length - 1; i > 0; i--) {
            let j = Math.floor(Math.random() * (i + 1));
            let temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }

    public static async doAndRepeat(
        toRepeat: () => Promise<void>,
        delayMs: number
    ): Promise<number> {
        await toRepeat();
        return window.setInterval(toRepeat, delayMs);
    }

    public static sortByInPlace<T>(items: T[], sortKey: (item: T) => any): T[] {
        return items.sort((first, second) => {
            const firstKey = sortKey(first);
            const secondKey = sortKey(second);

            if (firstKey < secondKey) {
                return -1;
            } else if (firstKey === secondKey) {
                return 0;
            } else {
                return 1;
            }
        });
    }

    public static formatTime(datetime: Date): string {
        return moment(datetime).format("HH:mm:ss");
    }

    public static setCookie(name: string, value: string) {
        Cookies.set(`${Util.CookiePrefix}${name}`, value, {
            expires: 7,
            sameSite: "strict",
        });
    }

    public static getCookie(name: string): string | undefined {
        return Cookies.get(`${Util.CookiePrefix}${name}`);
    }
}
