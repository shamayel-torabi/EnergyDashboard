import { IDrawingShape, IDiagramObject, ISchema, IShape, LineFeeder, LineConnectShape,
         BusBar, Transformer, BoxedConnectShape, ConnectShape } from '../Shapes';
import { DiagramMode } from '../Enums';
import { v4 as uuidv4 } from 'uuid';
import { UtilityService } from '../Services';
import { ObjectRebuilder } from '../Util';

export interface IDiagramModelProperties {
    title: string;
    width: number;
    height: number;
    backgroundColor: string;
    description: string;
    dispachingCode: string
    igmcCode: string;
    igmcStationId: number;
    mountDate: string;
    dismountDate: string;
}

export class DiagramModelProperties implements IDiagramModelProperties {
    public title: string;
    public width: number;
    public height: number;
    public backgroundColor: string;
    public description: string;
    public dispachingCode: string
    public igmcCode: string;
    public igmcStationId: number;
    public mountDate: string;
    public dismountDate: string;

    constructor(width: number, height: number) {
        const date = new Date();

        this.title = "دیاگرام";
        this.width = width;
        this.height = height;
        this.backgroundColor = "#FFFFFF";
        this.description = "شرح";
        this.dispachingCode = "";
        this.igmcCode = 'igmcCode';
        this.igmcStationId = 0;
        this.mountDate = new Date(date.getFullYear(), date.getMonth(), date.getDate()).toJSON();
        this.dismountDate = new Date(date.getFullYear() + 30, date.getMonth(), date.getDate()).toJSON();
    }
}

export interface IDiagramModel extends IDiagramObject {
    $type: string;
    id: string;
    diagramMode: DiagramMode;
    parentId: string;
    shapes: IDrawingShape<IShape>[];
    changed: boolean;
    Schema: ISchema;
    Properties: IDiagramModelProperties;

    addShape(shape: IDrawingShape<IShape>): void;
    removeShape(shape: IDrawingShape<IShape>): void;
    removeAll(): void;
    findShape(shapeId: string): IDrawingShape<IShape> | undefined;
    disconnect(shape: IDrawingShape<IShape>): void;
    bringToFront(shape: IDrawingShape<IShape>): void;
    updateProperty(obj: any): void;
    setShapeColor(shape: IDrawingShape<IShape>, color: string): void;
    setShapeVoltage(shape: IDrawingShape<IShape>, voltage: number): void;
    setBusColor(bus: BusBar, color: string): void;
    setBusVoltage(bus: BusBar, voltage: number): void;

    loadJson(json: any): void;

    toSVG(preAmble: boolean): string;
    toObject(): Object;
    toJSON(): string;
}

export class DiagramModel implements IDiagramModel {
    public $type: string;
    public id: string;
    public diagramMode: DiagramMode;
    public parentId: string;
    public shapes: IDrawingShape<IShape>[];
    public changed: boolean;
    private properties: IDiagramModelProperties;

    constructor(width = 1200, height = 800) {
        this.$type = "DiagramModel";
        this.id = uuidv4();
        this.diagramMode = "network";
        this.parentId = "";
        this.shapes = [];
        this.properties = new DiagramModelProperties(width, height);
        this.changed = false;

        this.updateProperty = this.updateProperty.bind(this);
    }

    public addShape(shape: IDrawingShape<IShape>) {
        this.shapes.push(shape);
    }
    public removeShape(shape: IDrawingShape<IShape>) {
        if (shape instanceof LineFeeder)
            return;

        if (shape instanceof LineConnectShape) {
            if (shape.srcShapeId) {
                let s = this.findShape(shape.srcShapeId) as IDrawingShape<IShape>;
                if (s && s.ports) {
                    for (let port of s.ports) {
                        if (port.connectShapeId === shape.id) {
                            if (s instanceof BusBar || s instanceof BoxedConnectShape) {
                                s.removePort(port);
                            }
                            else {
                                port.isConnected = false;
                                port.connectShapeId = null;
                            }
                        }
                    }
                }
                this.disconnect(s);
            }
            if (shape.desShapeId) {
                let s = this.findShape(shape.desShapeId) as IDrawingShape<IShape>;
                if (s && s.ports) {
                    for (let port of s.ports) {
                        if (port.connectShapeId === shape.id) {
                            if (s instanceof BusBar || s instanceof BoxedConnectShape) {
                                s.removePort(port);
                            }
                            else {
                                port.isConnected = false;
                                port.connectShapeId = null;
                            }
                        }
                    }
                }
                this.disconnect(s);
            }
        }
        if (shape && shape.ports) {
            for (let port of shape.ports) {
                if (port.isConnected && port.connectShapeId !== null) {
                    let s = this.findShape(port.connectShapeId);
                    port.connectShapeId = null;
                    port.isConnected = false;
                    if (s)
                        this.removeShape(s);
                }
            }
        }
        const index = this.shapes.indexOf(shape);
        if (index !== -1) {
            this.shapes.splice(index, 1);
        }
    }
    public removeAll(): void {
        const length = this.shapes.length;
        this.shapes.splice(0, length);
    }
    public findShape(shapeId: string): IDrawingShape<IShape> | undefined {
        let shape = this.shapes.find(x => x.id === shapeId);
        return shape;
    }
    public disconnect(shape: IDrawingShape<IShape>) {
        for (let shape of this.shapes) {
            if (shape instanceof BusBar)
                continue;
            shape.style.strokeStyle = "#000000";
            shape.voltage = 0;
            if (shape instanceof Transformer) {
                shape.strokeStylePrimary = "#000000";
                shape.Properties.primaryVoltage = 0;
                shape.strokeStyleSecondary = "#000000";
                shape.Properties.secondaryVoltage = 0;
            }
        }
        for (let shape of this.shapes) {
            if (shape instanceof BusBar) {
                this.setBusColor(shape, shape.style.strokeStyle);
                this.setBusVoltage(shape, shape.voltage);
            }
        }
    }
    public setShapeColor(shape: IDrawingShape<IShape>, color: string) {
        if (shape instanceof BusBar) {
            if (shape.ports && shape.ports.length) {
                for (let port of shape.ports) {
                    if (port.isConnected && port.connectShapeId) {
                        let s = this.findShape(port.connectShapeId);
                        if (s)
                            s.color = color;
                        //s.style.strokeStyle = color;
                    }
                }
            }
            return;
        }
        else if (shape instanceof ConnectShape) {
            //shape.style.strokeStyle = color;
            shape.color = color;

            if (shape.desShapeId) {
                let shp = this.findShape(shape.desShapeId);
                if (shp)
                    this.setShapeColor(shp, color);
            }
            if (shape.srcShapeId) {
                let shp = this.findShape(shape.srcShapeId);
                if (shp)
                    this.setShapeColor(shp, color);
            }
        }
        else if (shape instanceof Transformer) {
            if (shape.ports && shape.ports.length) {
                for (let port of shape.ports) {
                    if (port.isConnected && port.connectShapeId) {
                        let s = this.findShape(port.connectShapeId) as ConnectShape;
                        if (s.srcPortName === port.name || s.desPortName === port.name) {
                            if (port.name === "1" && shape.strokeStylePrimary === "#000000")
                                shape.strokeStylePrimary = color;
                            else if (port.name === "2" && shape.strokeStyleSecondary === "#000000")
                                shape.strokeStyleSecondary = color;
                        }
                    }
                }
            }
        }
        else {
            //shape.style.strokeStyle = color;
            shape.color = color;

            if (shape.ports && shape.ports.length) {
                for (let port of shape.ports) {
                    if (port.isConnected && port.connectShapeId) {
                        let s = this.findShape(port.connectShapeId) as ConnectShape;
                        if (s) {
                            //s.style.strokeStyle = color;
                            s.color = color;
                            if (s.srcShapeId === shape.id) {
                                if (s.desShapeId) {
                                    let shp = this.findShape(s.desShapeId);
                                    if (shp && shp.style.strokeStyle === "#000000")
                                        this.setShapeColor(shp, color);
                                }
                            }
                            else if (s.desShapeId === shape.id) {
                                if (s.srcShapeId) {
                                    let shp = this.findShape(s.srcShapeId);
                                    if (shp && shp.style.strokeStyle === "#000000")
                                        this.setShapeColor(shp, color);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public setShapeVoltage(shape: IDrawingShape<IShape>, voltage: number) {
        if (shape instanceof BusBar) {
            if (shape.ports && shape.ports.length) {
                for (let port of shape.ports) {
                    if (port.isConnected && port.connectShapeId) {
                        let s = this.findShape(port.connectShapeId);
                        if (s)
                            shape.voltage = voltage;
                    }
                }
            }
            return;
        }
        else if (shape instanceof ConnectShape) {
            shape.voltage = voltage;
            if (shape.desShapeId) {
                let shp = this.findShape(shape.desShapeId);
                if (shp)
                    this.setShapeVoltage(shp, voltage);
            }
            if (shape.srcShapeId) {
                let shp = this.findShape(shape.srcShapeId);
                if (shp)
                    this.setShapeVoltage(shp, voltage);
            }
        }
        else if (shape instanceof Transformer) {
            if (shape.ports && shape.ports.length) {
                for (let port of shape.ports) {
                    if (port.isConnected && port.connectShapeId) {
                        let s = this.findShape(port.connectShapeId) as ConnectShape;
                        if (s.srcPortName === port.name || s.desPortName === port.name) {
                            if (port.name === "1" && shape.Properties.primaryVoltage === 0)
                                shape.Properties.primaryVoltage = s.voltage;
                            else if (port.name === "2" && shape.Properties.secondaryVoltage === 0)
                                shape.Properties.secondaryVoltage = s.voltage;
                        }
                    }
                }
            }
        }
        else {
            shape.voltage = voltage;
            if (shape.ports && shape.ports.length) {
                for (let port of shape.ports) {
                    if (port.isConnected && port.connectShapeId) {
                        let s = this.findShape(port.connectShapeId) as ConnectShape;
                        if (s) {
                            s.voltage = voltage;
                            if (s.srcShapeId === shape.id) {
                                if (s.desShapeId) {
                                    let shp = this.findShape(s.desShapeId);
                                    if (shp && shp.voltage === 0)
                                        this.setShapeVoltage(shp, voltage);
                                }
                            }
                            else if (s.desShapeId === shape.id) {
                                if (s.srcShapeId) {
                                    let shp = this.findShape(s.srcShapeId);
                                    if (shp && shp.voltage === 0)
                                        this.setShapeVoltage(shp, voltage);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public setBusColor(bus: BusBar, color: string) {
        let branchEnd = false;

        bus.color = color;
        for (let port of bus.ports) {
            branchEnd = false;
            if (port.connectShapeId) {
                let s = this.findShape(port.connectShapeId);
                let bid = bus.id;
                while (!branchEnd && s) {
                    switch (s.$type) {
                        case "ConnectShape":
                        case "Breaker":
                        case "CT":
                            let shp = s;
                            if (shp)
                                shp.color = color;
                            //shp.style.strokeStyle = color;                                
                            s = this.findNextShape(shp, bid);
                            bid = shp.id;
                            break;
                        case "Transformer":
                            let transformer = s as Transformer;
                            let portName;
                            if (transformer.ports && transformer.ports.length) {
                                for (let port of transformer.ports) {
                                    if (port.isConnected && port.connectShapeId === bid) {
                                        portName = port.name;
                                        break;
                                    }
                                }
                            }
                            if (portName === "1")
                                transformer.strokeStylePrimary = color;
                            else if (portName === "2")
                                transformer.strokeStyleSecondary = color;
                            branchEnd = true;
                            break;
                        case "Generator":
                        case "LineFeeder":
                        case "LoadFeeder":
                            if (s)
                                s.color = color;
                            //s.style.strokeStyle = color;
                            branchEnd = true;
                            break;
                        default:
                            branchEnd = true;
                            //throw new Error('Shape in branch not defined')
                            break;
                    }
                }
            }
        }
    }
    public setBusVoltage(bus: BusBar, voltage: number) {
        let branchEnd = false;
        bus.voltage = voltage;

        //Start Modified
        bus.Properties.voltageMag = voltage;
        //End Modified

        for (let port of bus.ports) {
            branchEnd = false;
            if (port.connectShapeId) {
                let s = this.findShape(port.connectShapeId);
                let bid = bus.id;
                while (!branchEnd && s) {
                    switch (s.$type) {
                        case "ConnectShape":
                        case "Breaker":
                        case "CT":
                            let shp = s;
                            if (shp)
                                shp.voltage = voltage;
                            s = this.findNextShape(shp, bid);
                            bid = shp.id;
                            break;
                        case "Transformer":
                            let transformer = s as Transformer;
                            let portName;
                            if (transformer.ports && transformer.ports.length) {
                                for (let port of transformer.ports) {
                                    if (port.isConnected && port.connectShapeId === bid) {
                                        portName = port.name;
                                        break;
                                    }
                                }
                            }
                            if (portName === "1")
                                transformer.Properties.primaryVoltage = voltage;
                            else if (portName === "2")
                                transformer.Properties.secondaryVoltage = voltage;
                            branchEnd = true;
                            break;
                        case "Generator":
                        case "LineFeeder":
                        case "LoadFeeder":
                            if (s)
                                s.voltage = voltage;
                            branchEnd = true;
                            break;
                        default:
                            branchEnd = true;
                            //throw new Error('Shape in branch not defined')
                            break;
                    }
                }
            }
        }
    }
    private findNextShape(shape: IDrawingShape<IShape>, id: string): IDrawingShape<IShape> | undefined {
        if (shape instanceof ConnectShape) {
            let connectShape = shape;
            if (connectShape.srcShapeId === id) {
                if (connectShape.desShapeId)
                    return this.findShape(connectShape.desShapeId);
            }
            else if (shape.desShapeId === id) {
                if (connectShape.srcShapeId)
                    return this.findShape(connectShape.srcShapeId);
            }
            else
                return undefined;
        }
        else {
            let shapeId;
            for (let port of shape.ports) {
                if (port.connectShapeId !== id)
                    shapeId = port.connectShapeId;
            }
            if (shapeId)
                return this.findShape(shapeId);
        }
        return undefined;
    }
    public bringToFront(shape: IDrawingShape<IShape>) {
        let index = this.shapes.indexOf(shape);

        if (index > -1) {
            this.shapes.splice(index, 1);
            this.shapes.push(shape);
        }
    }
    public updateProperty(obj: any): void {
        let shape = this.findShape(obj.shapeId);
        if (shape && shape instanceof BusBar) {
            this.setBusColor(shape, obj.color);
            this.setBusVoltage(shape, obj.voltage);
        }
    }

    public loadJson(json: any) {
        let o = ObjectRebuilder.restoreObject(json);

        let shapes = o.shapes.map((s: any) => {
            s.notifyPropertyChange = this.updateProperty;
            return s;
        });

        this.$type = o.$type;
        this.id = o.id;
        this.diagramMode = o.diagramMode;
        this.parentId = o.parentId;
        this.shapes = shapes;
        this.Properties = o.properties;
        this.changed = o.changed;
    }

    public toSVG(preAmble: boolean = false): string {
        let svg: string[] = [];
        if (preAmble) {
            svg.push('<?xml version="1.0" encoding="UTF-8" standalone="no" ?>\n');
            svg.push('<!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" ');
            svg.push('"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd">\n');
        }

        svg.push('<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" ');
        svg.push(`width="${this.Properties.width}" height="${this.Properties.height}" `);
        svg.push(`style="background:${this.Properties.backgroundColor}" `);
        svg.push(`viewBox="0 0 ${this.Properties.width} ${this.Properties.height}" xml:space="preserve">\n`);
        svg.push('\t<desc>Created with EnergyDashboard App</desc>\n');
        if (this.shapes) {
            for (let i = 0; i < this.shapes.length; i++) {
                svg.push(this.shapes[i].toSVG());
            }
        }
        svg.push('</svg>');
        return svg.join('');
    }
    public toObject(): Object {
        let obj: Object = {
            $type: this.$type,
            id: this.id,
            diagramMode: this.diagramMode,
            parentId: this.parentId,
            shapes: this.shapes,
            properties: this.Properties,
            changed: this.changed,
        };
        return obj;
    }
    public toJSON(): string {
        let obj = this.toObject();
        return JSON.stringify(obj);
    }

    public get Schema(): ISchema {
        let modeText = "";
        switch (this.diagramMode) {
            case "network":
                modeText = "شبکه";
                break;
            case "area":
                modeText = "برق منطقه ای";
                break;
            case "zone":
                modeText = "ناحیه";
                break;
            case "substation":
                modeText = "ایستگاه";
                break;
            default:
                modeText = "نامعلوم";
                break;
        }

        let schema: ISchema =
        {
            title: `ویژگی دیاگرام ${modeText}`,
            type: "object",
            tabs: [
                {
                    tabIndex: 1,
                    tabName: 'عمومی',
                    properties: {
                        "title": {
                            "type": "string",
                            "required": true,
                            "title": "عنوان"
                        },
                        "description": {
                            "type": "string",
                            "format": "textarea",
                            "title": "شرح"
                        },
                        "width": {
                            "type": "integer",
                            "minimum": 0,
                            "title": "عرض"
                        },
                        "height": {
                            "type": "integer",
                            "minimum": 0,
                            "title": "طول"
                        },
                        "backgroundColor": {
                            "type": "string",
                            "format": "color",
                            "title": "رنگ زمینه"
                        }
                    }
                }
            ]
        };

        if (this.diagramMode === "substation") {
            schema.tabs.push(
                {
                    tabIndex: 2,
                    tabName: 'سنجش و پایش',
                    properties: {
                        "dispachingCode": {
                            "type": "string",
                            "title": "کد دیسپاچینگ"
                        },
                        "igmcCode": {
                            "type": "string",
                            "title": "کد مدیریت شبکه"
                        },
                        "igmcStationId": {
                            "type": "string",
                            "enum": UtilityService.IGMCSubstations,
                            "required": true,
                            "title": "شماره ایستگاه"
                        },
                        "mountDate": {
                            "type": "string",
                            "format": "date",
                            "title": "تاریخ شروع بهره برداری"
                        },
                        "dismountDate": {
                            "type": "string",
                            "format": "date",
                            "title": "تاریخ پایان بهره برداری"
                        },
                    }
                }
            )
        }

        return schema;
    }

    public get Properties(): IDiagramModelProperties {
        return this.properties;
    }

    public set Properties(prop: IDiagramModelProperties) {
        this.properties = prop;
    }

    public get backgroundColor(): string {
        return this.properties.backgroundColor;
    }

    public set backgroundColor(value: string) {
        this.properties.backgroundColor = value;
    }

    public get name(): string {
        return this.properties.title;
    }

    public set name(value: string) {
        this.properties.title = value;
    }

    public get width(): number {
        return this.properties.width;
    }

    public set width(value: number) {
        this.properties.width = value;
    }

    public get height(): number {
        return this.properties.height;
    }

    public set height(value: number) {
        this.properties.height = value;
    }
}
