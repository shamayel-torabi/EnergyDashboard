import { CanvasEngineAction, DrawingShapeType, DiagramMode } from '../Enums';
import {
    IPoint, Point, IDrawingShape,
    RectangleShape, RubberBandShape, CircleShape,
    LineShape, PolyLineShape, BoxedConnectShape,
    BusBar, Transformer, CT, Breaker, LoadFeeder, LineFeeder, Generator, IShape
} from '../Shapes';
import { MouseEventArgs, KeyboardEventArgs } from './EventArgs';
import { ToolBase } from './ToolBase';
import { DiagramEventHandler } from '../DiagramHandler';

export class DrawShapeTool extends ToolBase {
    private dragOffsetX: number;
    private dragOffsetY: number;
    private drag: boolean;

    constructor(diagramEventHandler: DiagramEventHandler) {
        super(diagramEventHandler);
        this.dragOffsetX = 0;
        this.dragOffsetY = 0;
        this.drag = false;
    }

    public startAction(): void {
        this.dragOffsetX = 0;
        this.dragOffsetY = 0;
        super.startAction();
    }

    public mouseDown(e: MouseEventArgs): void {

        let mouse = new Point(e.clientX, e.clientY);
        let areaId = e.diagramState.areaId as string;
        let zoneId = e.diagramState.zoneId as string;

        let drawingShapeType: DrawingShapeType = e.diagramState.drawingShapeType ;
        let diagramMode: DiagramMode = e.diagramState.diagramMode;

        if (this.drag) {
            let selection = this.selection;
            if (selection instanceof PolyLineShape) {
                let pt = this.snapToGrid(mouse);
                selection.shape.addPoint(pt);
            }
            return;
        }

        let pt = this.snapToGrid(mouse);

        let newShape = this.getNewShape(pt, drawingShapeType, areaId, zoneId, diagramMode);

        if (newShape) {
            this.diagramEventHandler.diagram.addShape(newShape);
            this.state = this.diagramEventHandler.state;
            this.setShapeAsSelected(newShape);
        }
        if (newShape instanceof LineShape || newShape instanceof PolyLineShape) {
            this.action = CanvasEngineAction.Resize;
            this.drag = true;
        }
        else if (newShape instanceof RubberBandShape) {
            this.action = CanvasEngineAction.Resize;
        }
        else
            this.action = CanvasEngineAction.Move;

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
                }
                break;
            case CanvasEngineAction.Resize:
                if (selection) {
                    selection.resizeToLocation(pt, this.handle);
                }
                break;
            default:
                break;
        }
    }

    public mouseUp(e: MouseEventArgs): void {
        let selection = this.selection;

        if (this.state)
            this.diagramEventHandler.commandInvoker.saveState(this.state);


        switch (this.action) {
            case CanvasEngineAction.Move:
                this.action = CanvasEngineAction.None;
                if (selection && this.state) {
                    selection.isSelected = false;
                }
                break;
            case CanvasEngineAction.Resize:
                if (this.drag) {
                    if (selection instanceof LineShape) {
                        this.drag = false;
                        if (selection) {
                            selection.isSelected = false;
                        }
                    }
                }
                else {
                    if (selection && this.state) {
                        selection.isSelected = false;
                    }
                }
                break;
            default:
                break;
        }

        this.dragOffsetX = 0;
        this.dragOffsetY = 0;
    }

    public KeyDown(e: KeyboardEventArgs): void {

    }


    private getNewShape(location: IPoint, drawingShape: DrawingShapeType, areaId: string, zoneId: string, diagramMode: DiagramMode): IDrawingShape<IShape> | null {
        let newShape: IDrawingShape<IShape> | null = null;
        let cursor: string = "auto";
        let name: string = "نامعلوم";
        let substationId : string  = '';

        let diagram = this.diagramEventHandler.diagram;

        switch (diagramMode) {
            case "network":
                name = "برق منطقه ای جدید";
                break;
            case "area":
                name = "ناحیه جدید";
                break;
            case "zone":
                name = "ایستگاه جدید";
                break;
            case "substation":
                substationId = diagram.id;
                break;
            default:
                break;
        }

        switch (drawingShape) {
            case DrawingShapeType.RectangleShape:
                newShape = new RectangleShape("R1", location.x, location.y, 60, 10);
                cursor = "move";
                break;
            case DrawingShapeType.RubberBandShape:
                newShape = new RubberBandShape("R1", location.x, location.y, 10, 10);
                cursor = "pointer";
                break;
            case DrawingShapeType.CircleShape:
                newShape = new CircleShape("C1", location.x, location.y, 30);
                cursor = "move";
                break;
            case DrawingShapeType.Transformer:
                newShape = new Transformer("T1", location.x, location.y);
                cursor = "move";
                break;
            case DrawingShapeType.CT:
                newShape = new CT("CT1", location.x, location.y);
                cursor = "move";
                break;
            case DrawingShapeType.Breaker:
                newShape = new Breaker("B1", location.x, location.y);
                cursor = "move";
                break;
            case DrawingShapeType.BusBar:
                newShape = new BusBar("Bus1", location.x, location.y);
                cursor = "pointer";
                break;
            case DrawingShapeType.LineShape:
                newShape = new LineShape("L1", location.x, location.y);
                cursor = "pointer";
                break;
            case DrawingShapeType.LineFeeder:
                newShape = new LineFeeder("Line To", location.x, location.y);
                cursor = "move";
                break;
            case DrawingShapeType.LoadFeeder:
                newShape = new LoadFeeder("LF1", location.x, location.y);
                cursor = "move";
                break;
            case DrawingShapeType.Generator:
                newShape = new Generator("G1", location.x, location.y);
                cursor = "move";
                break;
            case DrawingShapeType.PolyLineShape:
                newShape = new PolyLineShape("ML1", location.x, location.y);
                cursor = "pointer";
                break;
            case DrawingShapeType.BoxedConnectShape:
                newShape = new BoxedConnectShape(name, location.x, location.y, diagramMode);
                cursor = "move";
                break;
            default:
                break;
        }
        window.document.body.style.cursor = cursor;


        if (newShape) {
            newShape.areaId = areaId;
            newShape.zoneId = zoneId;
            newShape.substationId = substationId;
            newShape.notifyPropertyChange = this.diagramEventHandler.diagram.updateProperty;
        }
        return newShape;
    }
}