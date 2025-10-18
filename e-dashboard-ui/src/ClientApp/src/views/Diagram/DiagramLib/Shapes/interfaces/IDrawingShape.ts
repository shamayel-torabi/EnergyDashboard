import { IProperties } from "./IProperties";
import { IShape, IPoint, IRectangle } from "./IShapes";
import { IStyle, ITextStyle } from "./IStyle";
import { IPortShape } from "./IPortShape";
import { ITextShape } from "./ITextShape";
import { ISchema } from "./ISchema";
import { DrawingToolType, CanvasEngineAction } from "../../Enums";
import { IDiagramObject } from "./IDiagramObject";

export interface IDrawingShape<S extends IShape> extends  IDiagramObject{
    id: string;
    substationId: string;
    zoneId: string;
    areaId: string;
    shape: S;
    style: IStyle;
    ports: IPortShape[];
    texts: ITextShape[];
    isSelected: boolean;
    selectionZoneWidth: number;
    notifyPropertyChange: Function;
    changed: boolean;
    Properties: IProperties;

    location: IPoint;
    name: string;
    voltage: number;
    color: string;
    Schema: ISchema;

    draw(ctx: CanvasRenderingContext2D): void;
    inResizeZone: (mouse: IPoint) => number;
    resizeToLocation: (to: IPoint, handle: number) => void;
    move: (to: IPoint) => void;
    contains: (mousePoint: IPoint) => boolean;
    intersect(rect: IRectangle): boolean;
    getMoveOffset(mousePos: IPoint): IPoint;

    addPort(name: string, location: IPoint, offset: IPoint): IPortShape | null;
    removePort(port: IPortShape): void;
    getPort(mouse: IPoint): IPortShape | null;
    inPortZone(mouse: IPoint): boolean;
    findPort(portId: string): IPortShape | undefined;

    inTextZone(mouse: IPoint): boolean;
    getText(mouse: IPoint): ITextShape | null;
    addText(id: string, text: string, location: IPoint, offset: IPoint, style?: ITextStyle, angle?: number): void;
    removeText(text: ITextShape): void;
    findText(textId: string): ITextShape | undefined;

    rotate(angle: number): void;
    getCursorType(mousePoint: IPoint, tool: DrawingToolType): string;
    getClickLocationAction(mouse: IPoint, tool: DrawingToolType): CanvasEngineAction;
    toSVG(): string;
}

