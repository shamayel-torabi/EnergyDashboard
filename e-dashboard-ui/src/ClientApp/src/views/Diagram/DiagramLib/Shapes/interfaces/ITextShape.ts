import { IPoint } from "./IShapes";
import { ITextStyle } from "./IStyle";
import { IDiagramObject } from "./IDiagramObject";

export interface ITextShape extends IDiagramObject{
    id: string;
    text: string;
    offset: IPoint;
    location: IPoint;
    style: ITextStyle;
    isVisible: boolean;
    angle: number;

    draw(ctx: CanvasRenderingContext2D): void;
    rotate(angle: number): void;
    move(to: IPoint): void;
    inTextZone(mouse: IPoint): boolean;
    toSVG(): string;
    wrapText(ctx: CanvasRenderingContext2D, text: string, x: number, y: number, maxWidth: number, lineHeight: number): void;
}