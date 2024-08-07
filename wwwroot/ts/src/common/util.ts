import * as moment from "moment";

export type VueThis<Data> = Data & {
    $refs: { [refName: string]: HTMLElement };
} & { [functionName: string]: Function };

export class Util {
    public static uniqueByKey<T>(
        items: T[],
        keySelector: (item: T) => any
    ): T[] {
        return [
            ...new Map(items.map((item) => [keySelector(item), item])).values(),
        ];
    }
    public static timeBetween(bigger: Date, smaller: Date): moment.Moment {
        if (bigger < smaller) {
            bigger = smaller;
        }
        const duration = moment.duration(moment(bigger).diff(moment(smaller)));
        return moment.utc(duration.asMilliseconds());
    }

    public static getHtmlObjectContent(htmlObject: HTMLElement): HTMLElement {
        return (htmlObject as any).contentDocument;
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

    public static async sleep(milliseconds: number): Promise<void> {
        await new Promise((resolve) =>
            window.setTimeout(resolve, milliseconds)
        );
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

    public static randomNumber(seed: number): number {
        seed |= 0;
        seed = (seed + 0x9e3779b9) | 0;
        let t = seed ^ (seed >>> 16);
        t = Math.imul(t, 0x21f0aaad);
        t = t ^ (t >>> 15);
        t = Math.imul(t, 0x735a2d97);
        return ((t = t ^ (t >>> 15)) >>> 0) / 4294967296;
    }
}
