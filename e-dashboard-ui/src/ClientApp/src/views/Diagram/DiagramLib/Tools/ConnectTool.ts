import { CanvasEngineAction, DrawingToolType } from '../Enums';
import { Point, BusBar, IPoint, ConnectShape, BoxedBranch } from '../Shapes';
import { MouseEventArgs, KeyboardEventArgs } from './EventArgs';
import { ToolBase } from './ToolBase';
import { DiagramEventHandler } from '../DiagramHandler';

export class ConnectTool extends ToolBase {
    private startConnect: boolean;

    constructor(diagramEventHandler: DiagramEventHandler) {
        super(diagramEventHandler);

        this.startConnect = false;
    }

    public startAction(): void {
        super.startAction();
        this.startConnect = false;
    }


    public mouseDown(e: MouseEventArgs): void {
        if (this.inAction) {
            e.inAction = this.inAction;
            return;
        }

        let mouse = new Point(e.clientX, e.clientY);
        let areaId = e.diagramState.areaId as string;
        let zoneId = e.diagramState.zoneId as string;

        let drawingToolType: DrawingToolType = e.diagramState.drawingToolType;

        let shapes = this.diagramEventHandler.diagram.shapes;
        for (let shp of shapes) {
            let action = shp.getClickLocationAction(mouse, drawingToolType);

            if (action === CanvasEngineAction.Connect) {

                if (shp instanceof BusBar) {
                    let mp = shp.shape.edjePointNearestMouse(mouse) as IPoint;
                    mp = this.snapToGrid(mp);
                    let offset = new Point(mp.x - shp.shape.x, mp.y - shp.shape.y);
                    let loc = new Point(shp.shape.x, shp.shape.y);
                    let port = shp.addPort(shp.Properties.name, loc, offset);

                    if (port) {
                        if (port.isConnected)
                            return;

                        let connectShape = this.getNewShape(port.getLocation(), areaId, zoneId);
                        port.connectShapeId = connectShape.id;
                        port.isConnected = true;
                        port.handle = 0;

                        connectShape.connectShapeType = shp.$type;
                        connectShape.srcShapeId = shp.id;
                        connectShape.srcPortName = port.name;
                        connectShape.Properties.srcShapeName = shp.Properties.name;

                        this.diagramEventHandler.diagram.addShape(connectShape);
                        this.diagramEventHandler.diagram.setShapeColor(connectShape, shp.style.strokeStyle);
                        this.diagramEventHandler.diagram.setShapeVoltage(connectShape, shp.Properties.voltageMag)

                        this.setShapeAsSelected(connectShape);

                        this.startConnect = true;
                        super.mouseDown(e);
                    }
                }
                else {
                    let port = shp.getPort(mouse);
                    if (port) {
                        if (port.isConnected)
                            return;
                        let connectShape = this.getNewShape(port.getLocation(), areaId , zoneId) as ConnectShape;

                        port.connectShapeId = connectShape.id;
                        port.isConnected = true;
                        port.handle = 0;

                        connectShape.connectShapeType = shp.$type;
                        connectShape.srcShapeId = shp.id;
                        connectShape.srcPortName = port.name;
                        connectShape.Properties.srcShapeName = shp.Properties.name;

                        this.diagramEventHandler.diagram.addShape(connectShape);

                        if (shp.style.strokeStyle !== "#000000") {
                            this.diagramEventHandler.diagram.setShapeColor(connectShape, shp.style.strokeStyle);
                        }
                        if (connectShape.style.strokeStyle !== "#000000") {
                            this.diagramEventHandler.diagram.setShapeColor(shp, connectShape.style.strokeStyle);
                        }

                        if (shp.voltage !== 0)
                            this.diagramEventHandler.diagram.setShapeVoltage(connectShape, shp.voltage)

                        if (connectShape.voltage !== 0)
                            this.diagramEventHandler.diagram.setShapeVoltage(shp, connectShape.voltage);

                        this.setShapeAsSelected(connectShape);

                        this.startConnect = true;
                        super.mouseDown(e);
                    }
                }
            }
        }
    }

    public mouseMove(e: MouseEventArgs): void {
        let mouse = new Point(e.clientX, e.clientY);
        let selection = this.selection;

        if (selection && this.inAction) {
            let pt = this.snapToGrid(mouse);
            selection.resizeToLocation(pt, this.handle);
            super.mouseMove(e);
        }
    }

    public mouseUp(e: MouseEventArgs): void {
        e.inAction = this.inAction;
        let mouse = new Point(e.clientX, e.clientY);
        let drawingToolType: DrawingToolType = e.diagramState.drawingToolType;
        let shapes = this.diagramEventHandler.diagram.shapes;

        if (!this.startConnect && this.selection instanceof ConnectShape) {
            for (let shp of shapes) {
                let action = shp.getClickLocationAction(mouse, drawingToolType);
                if (action === CanvasEngineAction.Connect) {
                    let connectShape = this.selection;
                    if (connectShape.srcShapeId === shp.id)
                        break;
                    if (shp instanceof BusBar && connectShape.connectShapeType !== "BusBar") {
                        let mp = shp.shape.edjePointNearestMouse(mouse) as IPoint;
                        mp = this.snapToGrid(mp);
                        let offset = new Point(mp.x - shp.shape.x, mp.y - shp.shape.y);
                        let loc = new Point(shp.shape.x, shp.shape.y);
                        let port = shp.addPort(shp.Properties.name, loc, offset);
                        if (port) {
                            port.isConnected = true;
                            port.handle = 1;
                            port.connectShapeId = connectShape.id;

                            connectShape.shape.p2 = port.getLocation();
                            connectShape.desShapeId = shp.id;
                            connectShape.desPortName = port.name;
                            connectShape.Properties.desShapeName = shp.Properties.name;
                            connectShape.Properties.name = `${connectShape.Properties.srcShapeName}-${connectShape.Properties.desShapeName}`;

                            this.diagramEventHandler.diagram.setBusColor(shp, shp.style.strokeStyle);
                            this.diagramEventHandler.diagram.setBusVoltage(shp, shp.Properties.voltageMag);

                            this.startConnect = false;

                            this.state = this.diagramEventHandler.state;
                            this.diagramEventHandler.commandInvoker.saveState(this.state);
                            super.mouseUp(e);
                        }
                    }
                    else {
                        let port = shp.getPort(mouse);
                        if (port) {
                            port.isConnected = true;
                            port.handle = 1;
                            port.connectShapeId = connectShape.id;

                            connectShape.shape.p2 = port.getLocation();
                            connectShape.desShapeId = shp.id;
                            connectShape.desPortName = port.name;
                            connectShape.Properties.desShapeName = shp.Properties.name;
                            connectShape.Properties.name = `${connectShape.Properties.srcShapeName}-${connectShape.Properties.desShapeName}`

                            if (shp.style.strokeStyle !== "#000000") {
                                this.diagramEventHandler.diagram.setShapeColor(connectShape, shp.style.strokeStyle);
                            }
                            if (connectShape.style.strokeStyle !== "#000000") {
                                this.diagramEventHandler.diagram.setShapeColor(shp, connectShape.style.strokeStyle);
                            }

                            if (shp.voltage !== 0)
                                this.diagramEventHandler.diagram.setShapeVoltage(connectShape, shp.voltage)

                            if (connectShape.voltage !== 0)
                                this.diagramEventHandler.diagram.setShapeVoltage(shp, connectShape.voltage);


                            if (shp instanceof BoxedBranch)
                                shp.connectShapeType = connectShape.connectShapeType;

                            this.startConnect = false;

                            this.state = this.diagramEventHandler.state;
                            this.diagramEventHandler.commandInvoker.saveState(this.state);
                            super.mouseUp(e);
                        }
                    }
                }
            }
        }
        else {
            this.startConnect = false;
        }
    }

    public contextMenu(e: MouseEventArgs): void {
        if (this.inAction && this.selection instanceof ConnectShape) {
            this.inAction = false;
            this.startConnect = false;
            this.diagramEventHandler.diagram.removeShape(this.selection);
            this.selection = undefined;
        }
    }

    public KeyDown(e: KeyboardEventArgs): void {

        if (e.keyCode === 27 && this.inAction && this.selection instanceof ConnectShape) {
            this.inAction = false;
            this.startConnect = false;
            this.diagramEventHandler.diagram.removeShape(this.selection);
            this.selection = undefined;
        }
    }

    private getNewShape(location: IPoint, areaId: string, zoneId: string): ConnectShape {
        let shape = new ConnectShape("اتصال", location.x, location.y);

        window.document.body.style.cursor = "pointer";

        if (shape) {
            shape.areaId = areaId;
            shape.zoneId = zoneId;
            shape.substationId = this.diagramEventHandler.diagram.id;
            shape.notifyPropertyChange = this.diagramEventHandler.diagram.updateProperty;
        }
        return shape;
    }
}
