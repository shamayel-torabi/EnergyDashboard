import { CanvasEngineAction } from '../Enums';
import { IPoint, Point, IDrawingShape, IShape } from '../Shapes';
import { MouseEventArgs, KeyboardEventArgs, MouseWheelEventArgs } from './EventArgs';
import { DiagramEventHandler, IDiagramEventHandler, Memento } from '../DiagramHandler';

interface ITool {
    mouseDown(e: MouseEventArgs): void;
    mouseMove(e: MouseEventArgs): void;
    mouseUp(e: MouseEventArgs): void;
    contextMenu(e: MouseEventArgs): void;
    doubleClick(e: MouseEventArgs): void;
    KeyDown(e: KeyboardEventArgs): void;
}

export abstract class ToolBase implements ITool {
    protected diagramEventHandler: IDiagramEventHandler;

    protected action: CanvasEngineAction;
    protected handle: number;
    protected snapMouseSize: number;
    protected inAction: boolean;
    protected selection: IDrawingShape<IShape> | undefined;
    protected state: Memento | undefined;

    constructor(diagramHandler: DiagramEventHandler) {
        this.diagramEventHandler = diagramHandler;
        this.inAction = false;
        this.action = CanvasEngineAction.None;
        this.selection = undefined;
        this.handle = -1;
        this.snapMouseSize = 5;
        this.inAction = false;
    }

    public startAction(): void {
        this.inAction = false;
        this.action = CanvasEngineAction.None;
        this.selection = undefined;
    }

    public mouseDown(e: MouseEventArgs): void {
        this.inAction = true;
        e.inAction = true;
    }

    public mouseMove(e: MouseEventArgs): void {
        e.inAction = this.inAction;
    }

    public mouseUp(e: MouseEventArgs): void {
        this.inAction = false;
        e.inAction = false;
    }

    public mouseWheel(e: MouseWheelEventArgs): void {

    }

    public doubleClick(e: MouseEventArgs): void {
        if (this.selection)
            e.diagramState.selected = this.selection;
        e.inAction = this.inAction;
    }
    public contextMenu(e: MouseEventArgs): void {
        if (this.selection)
            e.diagramState.selected = this.selection;
        e.inAction = this.inAction;
    }
    public KeyDown(e: KeyboardEventArgs): void {
        e.inAction = this.inAction;
    }

    public endAction(): void {
        this.inAction = false;
    }

    protected snapToGrid(point: IPoint) {
        var x = Math.round(point.x / this.snapMouseSize) * this.snapMouseSize;
        var y = Math.round(point.y / this.snapMouseSize) * this.snapMouseSize;
        return new Point(x, y);
    }

    protected setShapeAsSelected(shape: IDrawingShape<IShape>) {
        shape.isSelected = true;
        this.selection = shape;
    }
}

