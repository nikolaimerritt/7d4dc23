import introAudio from "../../../audio/intro.mp3";
import fightStartAudio from "../../../audio/fight-start.mp3";
import fightAmbientAudio from "../../../audio/fight-ambient.mp3";
import fightEndAudio from "../../../audio/fight-end.mp3";

export default class MusicBox {
    private playingAudio?: HTMLAudioElement = undefined;

    public async playIntro() {
        await this.playAudio(introAudio);
    }

    public async playFightStart() {
        await this.playAudio(fightStartAudio);
    }

    public async playFightEnd() {
        await this.playAudio(fightEndAudio);
    }

    public loopAmbientFightingMusic() {
        this.stopCurrentAudio();
        this.playingAudio = new Audio(fightAmbientAudio);
        this.playingAudio.loop = true;
        this.playingAudio.play();
    }

    public stopCurrentAudio() {
        if (this.playingAudio !== undefined) {
            this.playingAudio.currentTime = this.playingAudio.duration;
            this.playingAudio.pause();
            this.playingAudio = undefined;
        }
    }

    private playAudio(audio: string): Promise<void> {
        this.stopCurrentAudio();
        this.playingAudio = new Audio(audio);
        this.playingAudio.play();
        return new Promise((resolve) =>
            this.playingAudio.addEventListener("ended", () => resolve(), {
                once: true,
            })
        );
    }
}
