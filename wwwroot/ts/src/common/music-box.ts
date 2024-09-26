import introAudio from "../../../audio/intro.mp3";
import fightStartAudio from "../../../audio/fight-start.mp3";
import fightAmbientAudio from "../../../audio/fight-ambient.mp3";
import fightEndAudio from "../../../audio/fight-end.mp3";

export default class MusicBox {
    public static playIntro() {
        const intro = new Audio(introAudio);
        intro.play();
    }
}
