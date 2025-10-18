
export interface IStyle{
    opacity: number;
    strokeWidth: number;
    strokeStyle: string;
    fillStyle: string;
}

export interface ITextStyle extends IStyle {
    fontFamily: string;
    fontSize: number;
    fontStyle: string;
    textAlign: CanvasTextAlign;
    textBaseline: CanvasTextBaseline;
    textAnchor: string;
}
