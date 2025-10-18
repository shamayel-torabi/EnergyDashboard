import {IDiagramObject} from './IDiagramObject'

export interface IPoint extends IDiagramObject {
    x: number;
    y: number;
    distance(point: IPoint): number;
}

export interface IShape extends IDiagramObject {
    angle: number;
    contains(mousePoint: IPoint): boolean;
    intersect(rect: IRectangle): boolean;
    equal(other: IShape): boolean;
}
export interface IRectangle extends IShape {
    x: number;
    y: number;
    height: number;
    width: number;
    selectionZoneWidth: number;
    resize(height: number, width: number): void;
    edjePointNearestMouse(mousePoint: IPoint): IPoint | null;
}
export interface ICircle extends IShape {
    x: number;
    y: number;
    radius: number;
    resize(radius: number): void;
    area(): number;
}
export interface ILine extends IShape {
    selectionZoneWidth: number;
    p1: IPoint;
    p2: IPoint;
    length(): number;
    getHandle(mouse: IPoint): IPoint | null;
    linepointNearestMouse(mousePoint: IPoint): IPoint
}
export interface IPolyLine extends IShape {
    selectionZoneWidth: number;
    points: Array<IPoint>;
    length(): number;
    getHandle(mouse: IPoint): IPoint | null;
    addPoint(point: IPoint): void;
    removePoint(point: IPoint): void;
}
