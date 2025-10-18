import { CanvasEngineAction, DrawingToolType } from '../Enums';
import { Point, IDrawingShape, IShape, IPoint } from '../Shapes';
import { MouseEventArgs, KeyboardEventArgs, MouseWheelEventArgs } from './EventArgs';
import { ToolBase } from './ToolBase';
import { DiagramEventHandler } from '../DiagramHandler';

export class SelectTool extends ToolBase {
    private dragOffsetX: number;
    private dragOffsetY: number;
    private lastMouseX: number;
    private lastMouseY: number;

    private isDragging: boolean;
    private dragStartPosition: IPoint;

    constructor(diagramEventHandler: DiagramEventHandler) {
        super(diagramEventHandler);
        this.dragOffsetX = 0;
        this.dragOffsetY = 0;
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.isDragging = false;
        this.dragStartPosition = new Point(0, 0);
    }

    public startAction(): void {
        this.dragOffsetX = 0;
        this.dragOffsetY = 0;
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.isDragging = false;
        //this.dragStartPosition = new Point(0, 0);

        super.startAction();
    }

    public mouseDown(e: MouseEventArgs): void {
        let mouse = new Point(e.clientX, e.clientY);
        let drawingToolType: DrawingToolType = e.diagramState.drawingToolType;
        let shapes = this.diagramEventHandler.diagram.shapes;

        for (let shp of shapes) {
            this.action = shp.getClickLocationAction(mouse, drawingToolType);
            switch (this.action) {
                case CanvasEngineAction.Resize:
                case CanvasEngineAction.Move:
                    this.handle = shp.inResizeZone(mouse);
                    let moveOffsetPoint = shp.getMoveOffset(mouse);
                    let pt = this.snapToGrid(moveOffsetPoint);
                    this.lastMouseX = shp.location.x;
                    this.lastMouseY = shp.location.y;

                    this.dragOffsetX = pt.x;
                    this.dragOffsetY = pt.y;
                    this.setShapeAsSelected(shp);
                    this.diagramEventHandler.diagram.bringToFront(shp);
                    break;
                default:
                    shp.isSelected = false;
                    break;
            }
        }

        if (this.action === CanvasEngineAction.Pan) {
            this.isDragging = true;
            this.dragStartPosition = new Point(e.clientX, e.clientY);
        }

        super.mouseDown(e);
    }

    public mouseMove(e: MouseEventArgs): void {
        let mouse = new Point(e.clientX, e.clientY);
        let pt = this.snapToGrid(mouse);
        let selection = this.selection;

        switch (this.action) {
            case CanvasEngineAction.Move:
                let newLocationX = pt.x - this.dragOffsetX;
                let newLocationY = pt.y - this.dragOffsetY;
                let newLocation = new Point(newLocationX, newLocationY);

                if (selection) {
                    this.diagramEventHandler.move(selection, newLocation);
                    this.state = this.diagramEventHandler.state;
                }
                break;
            case CanvasEngineAction.Resize:
                if (selection) {
                    selection.resizeToLocation(pt, this.handle);
                    this.state = this.diagramEventHandler.state;
                }
                break;
            default:
                break;
        }

        if (this.action === CanvasEngineAction.Pan) {
            let current = new Point(e.clientX, e.clientY);

            if (this.isDragging) {
                this.diagramEventHandler.translate(
                    current.x - this.dragStartPosition.x,
                    current.y - this.dragStartPosition.y);
            }
        }

        super.mouseMove(e);
    }

    public mouseUp(e: MouseEventArgs): void {
        let selection = this.selection;

        switch (this.action) {
            case CanvasEngineAction.Move:
                if (selection && this.state) {
                    let location = selection.location;
                    if (location.x !== this.lastMouseX || location.y !== this.lastMouseY) {
                        this.diagramEventHandler.commandInvoker.saveState(this.state);
                    }
                }
                break;
            case CanvasEngineAction.Resize:
                if (selection && this.state) {
                    this.diagramEventHandler.commandInvoker.saveState(this.state);
                }
                break;
            default:
                break;
        }

        if (this.action === CanvasEngineAction.Pan) {
            this.isDragging = false;
        }

        this.action = CanvasEngineAction.None;
        this.dragOffsetX = 0;
        this.dragOffsetY = 0;
        super.mouseUp(e);
    }

    public mouseWheel(e: MouseWheelEventArgs): void {
        let position = new Point(e.clientX, e.clientY);
        const factor = e.deltaY < 0 ? 1.1 : 0.9;
        this.diagramEventHandler?.zoom(factor, position);
    }

    public doubleClick(e: MouseEventArgs): void {

        let mouse = new Point(e.clientX, e.clientY);
        let drawingToolType: DrawingToolType = e.diagramState.drawingToolType;
        let shapes = this.diagramEventHandler.diagram.shapes;

        for (let shp of shapes) {
            this.action = shp.getClickLocationAction(mouse, drawingToolType);
            if (this.action === CanvasEngineAction.Move) {
                this.setShapeAsSelected(shp);
                this.diagramEventHandler.diagram.bringToFront(shp);
                e.diagramState.selected = shp
            }
        }
        e.inAction = this.inAction;
    }

    public KeyDown(e: KeyboardEventArgs): void {
        let selection = this.selection as IDrawingShape<IShape>;
        const diagram = this.diagramEventHandler.diagram;
        const p = this.diagramEventHandler.getMousePosition(diagram.Properties.width / 2, diagram.Properties.height / 2);

        switch (e.keyCode) {
            case 46:
                this.diagramEventHandler.diagram.removeShape(selection);
                this.state = this.diagramEventHandler.state;
                this.diagramEventHandler.commandInvoker.saveState(this.state);
                break;
            case 33:
                this.diagramEventHandler.zoom(1.1, p);
                break;
            case 34:
                this.diagramEventHandler.zoom(0.9, p);
                break;

            default:
                e.diagramState.selected = selection;
                break;
        }
    }
}
