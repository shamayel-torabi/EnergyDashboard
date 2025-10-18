import { IPoint } from "./IShapes";
import { IStyle } from "./IStyle";
import { IDiagramObject } from "./IDiagramObject";

export interface IPortShape extends  IDiagramObject{
    id: string;
    name: string;
    offset: IPoint;
    location: IPoint;
    style: IStyle;
    isConnected: boolean;
    handle: number;
    selectionZoneWidth: number;
    connectShapeId: string | null;
    draw(ctx: CanvasRenderingContext2D): void;
    move: (to: IPoint) => void;
    inPortZone(point: IPoint): boolean;
    getLocation(): IPoint;
}
