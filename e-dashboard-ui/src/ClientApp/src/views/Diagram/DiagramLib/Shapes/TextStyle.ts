
import { ITextStyle } from './interfaces';

type CanvasTextAlign = "left" | "right" | "center" | "start" | "end";
type CanvasTextBaseline= "top" | "hanging" | "middle" | "alphabetic" | "ideographic" | "bottom";

export class TextStyle implements ITextStyle {
    public opacity: number;
    public strokeWidth: number;
    public strokeStyle: string;
    public fillStyle: string;
    public fontFamily: string;
    public fontSize: number;
    public fontStyle: string;
    public textAlign: CanvasTextAlign;
    public textBaseline: CanvasTextBaseline;
    public textAnchor: string;

    constructor() {
        this.opacity = 1;
        this.strokeWidth = 1;
        this.strokeStyle = "#000000";
        this.fillStyle = "#101010";
        this.fontFamily = "Vazirmatn";
        this.fontSize = 8;
        this.fontStyle = "normal"; //oblique normal, italic, or bold
        this.textAlign = 'end';// start, end, left, right, center
        this.textBaseline = 'middle';//top hanging middle alphabetic ideographic bottom
        this.textAnchor = 'middle';
    }
}
