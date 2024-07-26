import * as moment from "moment";

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

        // // get total seconds between the times
        // var delta = Math.abs(bigger.getTime() - smaller.getTime()) / 1000;

        // // calculate (and subtract) whole days
        // var days = Math.floor(delta / 86400);
        // delta -= days * 86400;

        // // calculate (and subtract) whole hours
        // var hours = Math.floor(delta / 3600) % 24;
        // delta -= hours * 3600;

        // // calculate (and subtract) whole minutes
        // var minutes = Math.floor(delta / 60) % 60;
        // delta -= minutes * 60;

        // // what's left is seconds
        // var seconds = delta % 60;
        // return Util.formatTimeDifference(days, hours, minutes, seconds);
    }

    // private static formatTimeDifference(days: number, hours: number, minutes: number, seconds: number): string {
    //     let daysString = "";
    //     if (days > 0) {
    //         daysString += `${days} days, `;
    //     }
    //     return daysString + `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    // }
}
