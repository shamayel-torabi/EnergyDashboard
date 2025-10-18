import { DrawingToolType, CanvasEngineAction } from '../Enums';
import { PortShape } from './PortShape';
import { TextShape } from './TextShape';
import { Point } from './Shapes';
import { Style } from './Style';
import { v4 as uuidv4 } from 'uuid';
import {
    IProperties, IDrawingShape, IStyle,
    ISchema, ITextStyle, IPoint, IShape,
    IRectangle, IPortShape, ITextShape
} from './interfaces';

export class Properties implements IProperties {
    public name: string;
    public voltage: number;

    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
    }
}

export abstract class DrawingShape<S extends IShape> extends EventTarget implements IDrawingShape<S>{
    public $type: string;
    public id: string;
    public substationId: string;
    public zoneId: string;
    public areaId: string;
    public shape: S;
    public ports: IPortShape[];
    public texts: ITextShape[];
    public isSelected: boolean;
    public selectionZoneWidth: number;
    public style: IStyle;
    public notifyPropertyChange: Function;
    public changed: boolean;
    protected properties: IProperties;

    constructor(name: string) {
        super();
        this.$type = "DrawingShapeBase"
        this.id = uuidv4();
        this.zoneId = "";
        this.areaId = "";
        this.shape = {} as S;
        this.substationId = "";
        this.ports = [];
        this.texts = [];
        this.isSelected = false
        this.selectionZoneWidth = 3;
        this.style = new Style();
        this.notifyPropertyChange = (obj: any): void => { };
        this.changed = false;
        this.properties = new Properties(name);
    }

    public get location(): IPoint {
        return new Point(0, 0);
    }

    public set P(value: number) {
        let t = this.findText("P");
        if (t)
            t.text = `P=${value} Mw`;
    }

    public set Q(value: number) {
        let t = this.findText("Q");
        if (t)
            t.text = `Q=${value} MVar`;
    }

    public get voltage(): number {
        return this.properties.voltage;
    }

    public set voltage(value: number) {
        this.properties.voltage = value;

        let t = this.findText("Voltage");
        if (t)
            t.text = `V=${value} kV`;
    }

    public get name(): string {
        return this.properties.name;
    }

    public set name(value: string) {
        this.properties.name = value;

        let t = this.findText("Name");
        if (t)
            t.text = value;
    }

    public get color(): string {
        return this.style.strokeStyle;
    }

    public set color(value: string) {
        this.style.strokeStyle = value;
    }

    public abstract draw(ctx: CanvasRenderingContext2D): void;

    public inResizeZone(mouse: IPoint): number {
        return -1;
    }
    public abstract move(to: IPoint): void;

    public rotate(angle: number): void {
        if (this.shape)
            this.shape.angle = angle;
    }
    public abstract resizeToLocation(to: IPoint, handle: number): void;

    public contains(mousePoint: IPoint): boolean {
        if (this.shape)
            return this.shape.contains(mousePoint);
        return false;
    }
    public intersect(rect: IRectangle): boolean {
        if (this.shape)
            return this.shape.intersect(rect);
        return false;
    }
    public abstract getMoveOffset(mousePos: IPoint): IPoint;

    public abstract getCursorType(mousePoint: IPoint, tool: DrawingToolType): string;

    public getClickLocationAction(mousePoint: Point, tool: DrawingToolType): CanvasEngineAction {
        if (this.inPortZone(mousePoint))
            return CanvasEngineAction.Connect;
        else if (this.inResizeZone(mousePoint) !== -1)
            return CanvasEngineAction.Resize;
        else if (this.contains(mousePoint))
            return CanvasEngineAction.Move;
        else
            return CanvasEngineAction.Pan

        //changed
        //return CanvasEngineAction.Pan;
    }

    public inPortZone(mouse: IPoint): boolean {
        var ret: boolean = false;
        if (this.ports) {
            for (let port of this.ports) {
                ret = ret || port.inPortZone(mouse);
            }
        }
        return ret;
    }
    public getPort(mouse: IPoint): IPortShape | null {
        var p: IPortShape | null = null;
        if (this.ports) {
            for (let port of this.ports) {
                if (port.inPortZone(mouse)) {
                    p = port;
                    break;
                }
            }
        }
        return p;
    }
    public addPort(name: string, location: IPoint, offset: IPoint): IPortShape | null {
        if (this.ports) {
            let port: IPortShape = new PortShape(name, offset);
            port.move(location);
            this.ports.push(port);
            return port;
        }
        return null;
    }
    public removePort(port: IPortShape) {
        if (this.ports) {
            const index: number = this.ports.indexOf(port);
            if (index !== -1) {
                this.ports.splice(index, 1);
            }
        }
    }
    public findPort(portId: string): IPortShape | undefined {
        let port;
        if (this.ports) {
            port = this.ports.find(x => x.id === portId);
        }
        return port;
    }

    public inTextZone(mouse: IPoint): boolean {
        var ret: boolean = false;
        if (this.texts) {
            for (let text of this.texts) {
                ret = ret || text.inTextZone(mouse);
            }
        }
        return ret;
    }
    public getText(mouse: IPoint): ITextShape | null {
        var t: ITextShape | null = null;
        if (this.texts) {
            for (let text of this.texts) {
                if (text.inTextZone(mouse)) {
                    t = text;
                    break;
                }
            }
        }
        return t;
    }
    public addText(id: string, text: string, location: IPoint, offset: IPoint, style?: ITextStyle, angle: number = 0) {
        if (this.texts) {
            var t: ITextShape = new TextShape(id, text, offset, angle);
            if (style !== undefined)
                t.style = style;
            t.move(location);
            this.texts.push(t);
        }
    }
    public removeText(text: ITextShape) {
        if (this.texts) {
            const index: number = this.texts.indexOf(text);
            if (index !== -1) {
                this.texts.splice(index, 1);
            }
        }
    }
    public findText(textId: string): ITextShape | undefined {
        let text;
        if (this.texts) {
            text = this.texts.find(x => x.id === textId);
        }
        return text;
    }

    public abstract toSVG(): string;

    public get Schema(): ISchema {
        let schema: ISchema = {
            title: "ویژگی",
            type: "object",
            tabs: [
                {
                    tabIndex: 1,
                    tabName: 'عمومی',
                    properties: {
                        "name": {
                            "type": "string",
                            "title": "نام"
                        }
                    }
                }
            ]
        };

        return schema;
    }

    public get Properties(): IProperties {
        return this.properties;
    }

    public set Properties(prop: IProperties) {
        this.properties = prop;
        this.name = prop.name;
        this.voltage = prop.voltage;
    }

    protected getSvgId(): string {
        var ret = '';
        if (this.shape)
            ret = this.id ? 'id="' + this.id + '" ' : '';
        return ret;
    }
}
