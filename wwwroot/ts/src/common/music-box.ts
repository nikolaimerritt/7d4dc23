import introPath from "../../../audio/intro.mp3";
import fightStartPath from "../../../audio/fight-start.mp3";
import fightAmbientPath from "../../../audio/fight-ambient.mp3";
import fightEndPath from "../../../audio/fight-end.mp3";
import { TimeScale } from "chart.js";

interface Sound {
    name: "intro" | "fight-start" | "fight-end" | "fight-ambient";
    path: string;
}
export default class MusicBox {
    public static Singleton = new MusicBox();

    public static Intro: Sound = { name: "intro", path: introPath };
    public static FightStart: Sound = {
        name: "fight-start",
        path: fightStartPath,
    };
    public static FightAmbient: Sound = {
        name: "fight-ambient",
        path: fightAmbientPath,
    };
    public static FightEnd: Sound = { name: "fight-end", path: fightEndPath };

    private currentAudio?: HTMLAudioElement = undefined;
    private audioQueue: HTMLAudioElement[] = [];

    private constructor() {}

    public queue(sound: Sound, loop: boolean = false) {
        const audio = new Audio(sound.path);
        audio.loop = loop;
        audio.addEventListener("ended", () => {
            this.currentAudio = undefined;
            this.playNextInQueue();
        });
        this.audioQueue.push(audio);
        if (this.currentAudio?.loop) {
            this.stopCurrentAudio();
        }
        if (this.currentAudio === undefined) {
            this.playNextInQueue();
        }
    }

    public stopCurrentAudio() {
        if (this.currentAudio !== undefined) {
            if (!isNaN(this.currentAudio.duration)) {
                this.currentAudio.currentTime = this.currentAudio.duration;
            }
            this.currentAudio.pause();
            this.currentAudio = undefined;
        }
    }

    public isPlaying() {
        return this.currentAudio !== undefined;
    }

    private play(audio: HTMLAudioElement) {
        this.stopCurrentAudio();
        this.currentAudio = audio;
        audio.play();
    }

    private playNextInQueue() {
        if (this.audioQueue.length > 0) {
            const [topOfQueue] = this.audioQueue.splice(0, 1);
            this.play(topOfQueue);
        }
    }
}
