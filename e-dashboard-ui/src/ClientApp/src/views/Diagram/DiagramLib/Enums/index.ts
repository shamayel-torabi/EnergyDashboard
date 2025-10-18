import { IDrawingShape, IShape } from "../Shapes";

export enum DrawingToolType {
    Select,
    Connect,
    ConnectLine,
    DrawShape,
}

export enum DrawingShapeType {
    Null,
    Connect,
    RectangleShape,
    RubberBandShape,
    CircleShape,
    LineShape,
    PolyLineShape,
    BusBar,
    Transformer,
    CT,
    Breaker,
    LineFeeder,
    LoadFeeder,
    Generator,
    BoxedConnectShape,
    TransmisionLine,
}

export enum CanvasEngineAction {
    None,
    Move,
    Resize,
    Connect,
    ConnectLine,
    Pan,
}

export type DrawingTool = {
    drawingToolType: DrawingToolType;
    drawingShapeType: DrawingShapeType;
}

export enum DiagramTool {
    Select,
    Connect,
    Rectangle,
    RubberBand,
    Circle,
    Line,
    PolyLine,
    BusBar,
    Transformer,
    CT,
    Breaker,
    LineFeeder,
    LoadFeeder,
    Generator,
    BoxedConnect,
    TransmisionLine,
}

export enum LineType {
    Unknown = 0,
    Zone = 1,
    Area = 2,
    Network = 3,
    Abroad = 4,
}

export type DiagramMode = "network" | "area" | "zone" | "substation";

export type DiagramState = {
    diagramMode: DiagramMode
    drawingToolType: DrawingToolType;
    drawingShapeType: DrawingShapeType;
    areaId: string;
    zoneId: string;
    selected?: IDrawingShape<IShape>;
}
