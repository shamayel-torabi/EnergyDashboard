import { DrawingToolType } from "../Enums";
import { IDiagramModel, DiagramModel } from "../DiagramModel";
import { Point, IPoint, IDrawingShape, IShape } from "../Shapes";
import {
    MouseEventArgs, KeyboardEventArgs, MouseWheelEventArgs,
    SelectTool, DrawShapeTool, ConnectTool, ConnectLineTool
} from '../Tools';
import { CommandInvoker, ICommandInvoker } from "./CommandInvoker";
import { Memento } from "./Memento";

export interface IDiagramEventHandler {
    diagram: IDiagramModel;
    commandInvoker: ICommandInvoker;
    state: Memento;
    canvasSize: IPoint;
    firstRun: boolean;

    handleMouseDown(event: MouseEventArgs): void;
    handleMouseMove(event: MouseEventArgs): void;
    handleMouseUp(event: MouseEventArgs): void;
    handleContextMenu(event: MouseEventArgs): void;
    handleDoubleClick(event: MouseEventArgs): void;
    handleWheel(event: MouseWheelEventArgs): void;
    handleKeyDown(event: KeyboardEventArgs): void;

    draw(): void;
    save(): void;
    load(): void;
    undo(): void;
    redo(): void;
    move(shape: IDrawingShape<IShape>, to: IPoint): void;
    zoom(factor: number, currentPosition: IPoint): void;
    translate(dx: number, dy: number): void;
    setDiagramSize(w: number, h: number): void;
    getMousePosition(clientX: number, clientY: number): IPoint;

    loadDiagram(diagram: any): void;
}

export class DiagramEventHandler implements IDiagramEventHandler {
    private canvas: HTMLCanvasElement;
    private context: CanvasRenderingContext2D;
    private diagramModel: IDiagramModel;
    private _commandInvoker: ICommandInvoker;

    private selectTool: SelectTool;
    private drawShapeTool: DrawShapeTool;
    private connectTool: ConnectTool;
    private connectLineTool: ConnectLineTool;

    private inAction: boolean;
    private snapSize: number;
    private grid: boolean;
    private _firstRun: boolean;

    constructor(canvas: HTMLCanvasElement, grid: boolean = true) {
        this.canvas = canvas;
        this.grid = grid;
        this.diagramModel = new DiagramModel();
        this._commandInvoker = new CommandInvoker(this);

        this.context = canvas.getContext('2d') as CanvasRenderingContext2D;
        this.snapSize = 10;

        this.selectTool = new SelectTool(this);
        this.connectTool = new ConnectTool(this);
        this.connectLineTool = new ConnectLineTool(this);
        this.drawShapeTool = new DrawShapeTool(this);
        this.inAction = false;
        this._firstRun = true;
    }
    public get firstRun(): boolean {
        return this._firstRun;
    }

    public set firstRun(v: boolean) {
        this._firstRun = v;
    }

    public get canvasSize(): IPoint {
        return new Point(this.canvas.width, this.canvas.height);
    }

    public get diagram(): IDiagramModel {
        return this.diagramModel;
    }
    public get commandInvoker(): ICommandInvoker {
        return this._commandInvoker;
    }

    public get state(): Memento {
        return new Memento(this.diagramModel.toObject());
    }
    public set state(memento: Memento) {
        this.diagramModel.loadJson(memento.state);
    }

    public handleMouseDown(event: MouseEventArgs): void {
        let mouse = this.getMousePosition(event.clientX, event.clientY);
        event.clientX = mouse.x;
        event.clientY = mouse.y;

        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.startAction();
                this.selectTool.mouseDown(event);
                break;
            case DrawingToolType.Connect:
                if (!this.inAction)
                    this.connectTool.startAction();
                this.connectTool.mouseDown(event);
                this.inAction = event.inAction as boolean;
                break;
            case DrawingToolType.ConnectLine:
                if (!this.inAction)
                    this.connectLineTool.startAction();
                this.connectLineTool.mouseDown(event);
                this.inAction = event.inAction as boolean;
                break;
            case DrawingToolType.DrawShape:
                this.selectTool.startAction();
                this.drawShapeTool.mouseDown(event);
                break;
            default:
                break;
        }
    }
    public handleMouseMove(event: MouseEventArgs): void {
        let mouse = this.getMousePosition(event.clientX, event.clientY);

        event.clientX = mouse.x;
        event.clientY = mouse.y;

        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType;

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.mouseMove(event);
                break;
            case DrawingToolType.Connect:
                this.connectTool.mouseMove(event);
                this.inAction = event.inAction as boolean;
                break;
            case DrawingToolType.ConnectLine:
                this.connectLineTool.mouseMove(event);
                this.inAction = event.inAction as boolean;
                break;
            case DrawingToolType.DrawShape:
                this.drawShapeTool.mouseMove(event);
                break;
            default:
                break;
        }

        let shapes = this.diagramModel.shapes;
        let mousePointer = "auto";

        for (let shp of shapes) {
            if ((shp.inResizeZone(mouse) !== -1) ||
                shp.inPortZone(mouse) ||
                shp.contains(mouse)) {
                mousePointer = shp.getCursorType(mouse, drawingToolType);
                break;
            }
        }

        window.document.body.style.cursor = mousePointer;
    }
    public handleMouseUp(event: MouseEventArgs): void {
        let mouse = this.getMousePosition(event.clientX, event.clientY);
        event.clientX = mouse.x;
        event.clientY = mouse.y;
        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType;

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.mouseUp(event);
                break;
            case DrawingToolType.Connect:
                this.connectTool.mouseUp(event);
                if (!this.inAction) {
                    this.connectTool.endAction();
                    event.inAction = false;
                }
                break;
            case DrawingToolType.ConnectLine:
                this.connectLineTool.mouseUp(event);
                if (!this.inAction) {
                    this.connectLineTool.endAction();
                    event.inAction = false;
                }
                break;
            case DrawingToolType.DrawShape:
                this.drawShapeTool.mouseUp(event);
                break;
            default:
                break;
        }
    }
    public handleContextMenu(event: MouseEventArgs): void {
        let mouse = this.getMousePosition(event.clientX, event.clientY);
        event.clientX = mouse.x;
        event.clientY = mouse.y;
        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType;

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.doubleClick(event);
                break;
            case DrawingToolType.Connect:
                this.connectTool.contextMenu(event);
                break;
            case DrawingToolType.ConnectLine:
                this.connectLineTool.contextMenu(event);
                break;
            case DrawingToolType.DrawShape:
                break;
            default:
                break;
        }
    }
    public handleDoubleClick(event: MouseEventArgs): void {
        let mouse = this.getMousePosition(event.clientX, event.clientY);
        event.clientX = mouse.x;
        event.clientY = mouse.y;
        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType;

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.contextMenu(event);
                break;
            case DrawingToolType.Connect:
                this.connectTool.contextMenu(event);
                break;
            case DrawingToolType.ConnectLine:
                this.connectLineTool.contextMenu(event);
                break;
            case DrawingToolType.DrawShape:
                break;
            default:
                break;
        }
    }
    public handleWheel(event: MouseWheelEventArgs): void {
        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType;

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.mouseWheel(event);
                break;
            case DrawingToolType.Connect:
                break;
            case DrawingToolType.ConnectLine:
                break;
            case DrawingToolType.DrawShape:
                break;
            default:
                break;
        }
    }
    public handleKeyDown(event: KeyboardEventArgs): void {
        const drawingToolType: DrawingToolType = event.diagramState.drawingToolType;

        switch (drawingToolType) {
            case DrawingToolType.Select:
                this.selectTool.KeyDown(event);
                break;
            case DrawingToolType.Connect:
                this.connectTool.KeyDown(event);
                break;
            case DrawingToolType.ConnectLine:
                this.connectLineTool.KeyDown(event);
                break;
            case DrawingToolType.DrawShape:
                break;
            default:
                break;
        }

        let mouse = this.getMousePosition(this.canvas.width / 2, this.canvas.height / 2);

        switch (event.keyCode) {
            case 33:
                this.zoom(1.1, mouse);
                break;
            case 34:
                this.zoom(0.9, mouse);
                break;
            default:
                break;
        }
    }
    public draw(): void {
        let shapes = this.diagramModel.shapes;
        let context = this.context as CanvasRenderingContext2D;

        if (this._firstRun) {
            let xx = (this.canvas.width - this.diagram.Properties.width) / 2;
            let yy = (this.canvas.height - this.diagram.Properties.height) / 2;
            this.context.translate(xx, yy);
            this._firstRun = false;
        }

        this.clearCanvas();

        if (context) {
            this.drawGrid();

            for (let shp of shapes) {
                this.context.save();
                shp.draw(context);
                this.context.restore();
            }
        }
    }
    public load() {
        let data;
        if (this.diagramModel.diagramMode === "substation")
            data = this.getLocal('Substation-Diagram');
        else
            data = this.getLocal('Network-Diagram');
        this.diagramModel.loadJson(data);
    }
    public save() {
        if (this.diagramModel.diagramMode === "substation")
            this.setLocal('Substation-Diagram', this.diagramModel.toJSON());
        else
            this.setLocal('Network-Diagram', this.diagramModel.toJSON());
    }
    public undo(): void {
        this.commandInvoker.undo();
    }
    public redo(): void {
        this.commandInvoker.redo();
    }
    public move(shape: IDrawingShape<IShape>, to: IPoint): void {
        shape.move(to);
        if (shape.ports && shape.ports.length) {
            for (let port of shape.ports) {
                if (port.isConnected && port.connectShapeId) {
                    let s = this.diagramModel.findShape(port.connectShapeId);
                    if (s)
                        s.resizeToLocation(port.getLocation(), port.handle);
                }
            }
        }
    }
    public zoom(factor: number, currentPosition: IPoint): void {
        this.context.translate(currentPosition.x, currentPosition.y);
        this.context.scale(factor, factor);
        this.context.translate(-currentPosition.x, -currentPosition.y);
    }
    public translate(x: number, y: number): void {
        this.context.translate(x, y);
    }
    public setDiagramSize(w: number, h: number) {
        this.diagramModel.Properties.width = w;
        this.diagramModel.Properties.height = h;
    }
    public getMousePosition(clientX: number, clientY: number): IPoint {
        let rect = this.canvas.getBoundingClientRect();
        let mouseX = clientX - rect.left;
        let mouseY = clientY - rect.top;
        return this.transformedPoint(mouseX, mouseY);
    }

    public loadDiagram(diagram: any): void {
        this.diagramModel.loadJson(diagram);
    }

    private drawGrid(): void {
        let context = this.context as CanvasRenderingContext2D;
        let width = this.diagramModel.Properties.width;
        let height = this.diagramModel.Properties.height;
        let i, j, x, y;
        let dx = Math.round(width / this.snapSize);
        let dy = Math.round(height / this.snapSize);

        this.context.save();
        context.fillStyle = this.diagramModel.Properties.backgroundColor;
        context.fillRect(0, 0, this.diagramModel.Properties.width, this.diagramModel.Properties.height);
        this.context.restore();

        if (this.grid) {
            this.context.save();
            var p = this.context.getTransform();
            const pixl = 1 / p.a > 2 ? 1 : 1 / p.a;

            context.globalAlpha = 0.3;
            for (i = 1; i < dx; i++) {
                x = i * this.snapSize;
                for (j = 1; j < dy; j++) {
                    y = j * this.snapSize;
                    context.beginPath();
                    context.arc(x, y, pixl, 0, 2 * Math.PI, true);
                    context.fill();
                }
            }
            this.context.restore();
        }
    }
    private clearCanvas(): void {
        let context = this.context as CanvasRenderingContext2D;
        let width = this.canvas.width;
        let height = this.canvas.height;

        context.save();
        context.setTransform(1, 0, 0, 1, 0, 0);
        context.clearRect(0, 0, width, height);
        context.restore();
    }
    private transformedPoint(x: number, y: number): IPoint {
        const originalPoint = new DOMPoint(x, y);
        var aa = this.context.getTransform().invertSelf().transformPoint(originalPoint);
        return new Point(aa.x, aa.y);
    }
    private getLocal(key: string): any {
        const data = window.localStorage.getItem(key);
        let ret = null;
        if (data)
            ret = JSON.parse(data);
        return ret;
    }
    private setLocal(key: string, value: any): void {
        const data = value === undefined ? null : value;
        if (data)
            window.localStorage.setItem(key, data);
    }
}