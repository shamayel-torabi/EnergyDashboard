import { CanvasEngineAction, DrawingToolType, LineType } from '../Enums';
import { Point, IPoint, BoxedConnectShape, TransmisionLine, LineFeeder, IPolyLine } from '../Shapes';
import { MouseEventArgs, KeyboardEventArgs } from './EventArgs';
import { ToolBase } from './ToolBase';
import { DiagramEventHandler } from '../DiagramHandler';

export class ConnectLineTool extends ToolBase {
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
        let mouse = new Point(e.clientX, e.clientY);
        let areaId = e.diagramState.areaId as string;
        let zoneId = e.diagramState.zoneId as string;


        if (this.inAction) {
            if (this.selection instanceof TransmisionLine) {
                let pt = this.snapToGrid(mouse);
                this.selection.shape.addPoint(pt);
            }
            e.inAction = this.inAction;
            return;
        }

        let drawingToolType: DrawingToolType = e.diagramState.drawingToolType;
        let shapes = this.diagramEventHandler.diagram.shapes;

        for (let shp of shapes) {
            this.action = shp.getClickLocationAction(mouse, drawingToolType);
            if (this.action === CanvasEngineAction.ConnectLine) {
                //this.diagramEventHandler.commandInvoker.saveState();

                if (shp instanceof BoxedConnectShape) {

                    let mp = shp.shape.edjePointNearestMouse(mouse) as IPoint;
                    mp = this.snapToGrid(mp);
                    let offset = new Point(mp.x - shp.shape.x, mp.y - shp.shape.y);
                    let loc = new Point(shp.shape.x, shp.shape.y);
                    let port = shp.addPort(shp.Properties.name, loc, offset);
                    if (port && !port.isConnected) {

                        let connectShape = this.getNewTransmisionLineShape(port.getLocation(),  areaId, zoneId);
                        if (connectShape) {
                            port.isConnected = true;
                            port.handle = 0;
                            port.connectShapeId = connectShape.id;

                            connectShape.connectShapeType = shp.$type;
                            connectShape.srcShapeId = shp.id;
                            connectShape.srcPortName = port.name;
                            connectShape.Properties.srcShapeName = shp.Properties.name;

                            this.diagramEventHandler.diagram.addShape(connectShape);
                            //this.state = this.diagramEventHandler.getState();

                            this.setShapeAsSelected(connectShape);

                            this.startConnect = true;
                            super.mouseDown(e);   
                        }
                    }
                }
                else if (shp instanceof LineFeeder) {
                    let port = shp.getPort(mouse);
                    if (port) {
                        if (port.isConnected)
                            return;

                        let connectShape = this.getNewTransmisionLineShape(port.getLocation(), areaId, zoneId);
                        if (connectShape) {
                            if (shp.connectShapeId)
                                connectShape.id = shp.connectShapeId;

                            port.isConnected = true;
                            port.handle = 0;
                            port.connectShapeId = connectShape.id;

                            connectShape.connectShapeType = shp.$type;
                            connectShape.connectType = 2;
                            connectShape.srcShapeId = shp.id;
                            connectShape.srcPortName = port.name;

                            connectShape.Properties.dispachingCode = shp.Properties.dispachingCode;
                            connectShape.Properties.voltage = shp.Properties.voltage;
                            connectShape.Properties.igmcCode = shp.Properties.igmcCode;
                            connectShape.Properties.toolId = shp.Properties.toolId;
                            connectShape.Properties.transferCapacity = shp.Properties.transferCapacity;
                            connectShape.Properties.lineFeederType = shp.Properties.lineFeederType;
                            connectShape.Properties.srcShapeName = shp.Properties.name;

                            this.diagramEventHandler.diagram.addShape(connectShape);
                            //this.state = this.diagramEventHandler.getState();

                            // if (shp.style.strokeStyle !== "#000000") {
                            //     this.diagramEventHandler.setShapeColor(connectShape, shp.style.strokeStyle);
                            // }
                            // if (connectShape.style.strokeStyle !== "#000000") {
                            //     this.diagramEventHandler.setShapeColor(shp, connectShape.style.strokeStyle);
                            // }

                            // if (shp.voltage !== 0)
                            //     this.diagramEventHandler.setShapeVoltage(connectShape, shp.voltage)

                            // if (connectShape.voltage !== 0)
                            //     this.diagramEventHandler.setShapeVoltage(shp, connectShape.voltage);

                            this.setShapeAsSelected(connectShape);
                            this.startConnect = true;
                            super.mouseDown(e);   
                        }
                    }
                }
            }
        }
    }

    public mouseMove(e: MouseEventArgs): void {
        let mouse = new Point(e.clientX, e.clientY);
        let selection = this.selection as TransmisionLine;

        if (selection && this.inAction) {
            let shape = selection.shape as IPolyLine;
            this.handle = shape.points.length - 1
            let pt = this.snapToGrid(mouse);
            selection.resizeToLocation(pt, this.handle);
            super.mouseMove(e);
        }
    }

    public mouseUp(e: MouseEventArgs): void {
        e.inAction = this.inAction;

        let mouse = new Point(e.clientX, e.clientY);
        let drawingToolType: DrawingToolType = e.diagramState.drawingToolType;
        let connectShape = this.selection as TransmisionLine;
        let shapes = this.diagramEventHandler.diagram.shapes;

        if (!this.startConnect && this.inAction) {
            for (let shp of shapes) {
                let action = shp.getClickLocationAction(mouse, drawingToolType);
                if (action === CanvasEngineAction.ConnectLine) {
                    if (connectShape.srcShapeId === shp.id)
                        break;
                    if (shp instanceof BoxedConnectShape) {
                        let mp = shp.shape.edjePointNearestMouse(mouse) as IPoint;
                        mp = this.snapToGrid(mp);
                        let offset = new Point(mp.x - shp.shape.x, mp.y - shp.shape.y);
                        let loc = new Point(shp.shape.x, shp.shape.y);
                        let port = shp.addPort(shp.Properties.name, loc, offset);

                        if (port) {
                            if (connectShape) {
                                let shape = connectShape.shape as IPolyLine;
                                const index = shape.points.length - 1;
                                shape.points.splice(index, 1);

                                port.isConnected = true;
                                port.handle = shape.points.length - 1;
                                port.connectShapeId = connectShape.id;

                                connectShape.desShapeId = shp.id;
                                connectShape.desPortName = port.name;
                                connectShape.Properties.desShapeName = shp.Properties.name;
                                connectShape.Properties.name = `${connectShape.Properties.srcShapeName}-${connectShape.Properties.desShapeName}`

                                if (connectShape.connectShapeType === "BoxedConnectShape") {
                                    connectShape.connectType = 1;
                                    switch (e.diagramState.diagramMode) {
                                        case "network":
                                            connectShape.Properties.lineFeederType = LineType.Network;
                                            break;
                                        case "area":
                                            connectShape.Properties.lineFeederType = LineType.Area;
                                            break;
                                        case "zone":
                                            connectShape.Properties.lineFeederType = LineType.Zone;
                                            break;
                                        default:
                                            connectShape.Properties.lineFeederType = LineType.Unknown;
                                            break;
                                    }
                                }
                                else
                                    connectShape.connectType = 2;

                                connectShape.resizeToLocation(mp, shape.points.length - 1);
                                this.startConnect = false;

                                this.state = this.diagramEventHandler.state;
                                this.diagramEventHandler.commandInvoker.saveState(this.state);

                                super.mouseUp(e);
                            }
                        }
                    }
                    else if (shp instanceof LineFeeder) {
                        let port = shp.getPort(mouse);
                        if (port) {
                            if (shp.connectShapeId)
                                connectShape.id = shp.connectShapeId;

                            port.isConnected = true;
                            port.handle = connectShape.shape.points.length - 1;
                            port.connectShapeId = connectShape.id;

                            connectShape.desShapeId = shp.id;
                            connectShape.desPortName = port.name;
                            connectShape.connectType = 2;

                            connectShape.Properties.dispachingCode = shp.Properties.dispachingCode;
                            connectShape.Properties.voltage = shp.Properties.voltage;
                            connectShape.Properties.igmcCode = shp.Properties.igmcCode;
                            connectShape.Properties.toolId = shp.Properties.toolId;
                            connectShape.Properties.transferCapacity = shp.Properties.transferCapacity;
                            connectShape.Properties.lineFeederType = shp.Properties.lineFeederType;
                            connectShape.Properties.desShapeName = shp.Properties.name;
                            connectShape.Properties.name = `${connectShape.Properties.srcShapeName}-${connectShape.Properties.desShapeName}`

                            let mp = port.getLocation();
                            connectShape.resizeToLocation(mp, connectShape.shape.points.length - 1);
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
        let selection = this.selection;

        if (this.inAction && selection) {
            this.inAction = false;
            this.startConnect = false;
            e.inAction = false;
            this.diagramEventHandler.diagram.removeShape(selection);
        }
    }

    public KeyDown(e: KeyboardEventArgs): void {
        let selection = this.selection;

        if (this.inAction && e.keyCode === 27 && selection) {
            this.inAction = false;
            e.inAction = false;
            this.startConnect = false;
            this.diagramEventHandler.diagram.removeShape(selection);
        }
    }

    private getNewTransmisionLineShape(location: IPoint, areaId: string, zoneId: string): TransmisionLine {
        let newShape = new TransmisionLine("خط انتقال", location.x, location.y);

        if (newShape) {
            newShape.areaId = areaId;
            newShape.zoneId = zoneId;
            newShape.notifyPropertyChange = this.diagramEventHandler.diagram.updateProperty;
        }

        window.document.body.style.cursor = "pointer";

        return newShape;
    }
}
