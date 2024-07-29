import * as moment from "moment";

export type VueThis<Data> = Data & {
    $refs: { [refName: string]: HTMLElement };
} & { [functionName: string]: Function };

export class Util {
    public static uniqueByKey(
        items: object[],
        keySelector: (item) => object
    ): object[] {
        return [
            ...new Map(items.map((item) => [keySelector(item), item])).values(),
        ];
    }
    public static timeBetween(bigger: Date, smaller: Date): string {
        if (bigger < smaller) {
            bigger = smaller;
        }
        let duration = moment.duration(moment(bigger).diff(moment(smaller)));
        return moment.utc(duration.asMilliseconds()).format("HH:mm:ss");
    }
    public static getHtmlObjectContent(htmlObject: HTMLElement): HTMLElement {
        return (htmlObject as any).contentDocument;
    }
}
