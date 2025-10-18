import { IPoint } from './interfaces/';
import { Point } from './Shapes';
import { SVGUtil } from '../Util';
import { Bus } from './Bus';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces/ISchema';
import { IProperties } from './interfaces';
import { CanvasEngineAction, DrawingToolType } from '../Enums';
import { UtilityService } from '../Services';

interface IBoxedConnectShapeProperties extends IProperties {
    description: string;
    width: number;
    height: number;
    abroad: boolean;
    igmcStationId: number;
}

export class BoxedConnectShapeProperties implements IBoxedConnectShapeProperties {
    public name: string;
    public voltage: number;
    public description: string;
    public width: number;
    public height: number;
    public abroad: boolean;
    public igmcStationId: number;

    constructor(name: string, description: string) {
        this.name = name;
        this.voltage = 0;
        this.description = description;
        this.width = 200;
        this.height = 30;
        this.abroad = false;
        this.igmcStationId = 0;
    }
}


export class BoxedConnectShape extends Bus {
    public mode: string;
    protected properties: IBoxedConnectShapeProperties;

    constructor(name: string, x: number, y: number, mode: string) {
        super(name, x, y, 200, 30);
        this.$type = "BoxedConnectShape";
        this.mode = mode;

        this.properties = new BoxedConnectShapeProperties(name, "شرح");
        let location = new Point(x, y);

        let w: number = this.shape.width / 2;
        let h: number = this.shape.height / 2;

        var style = new TextStyle();
        style.textAlign = 'center'; // start, end, left, right, center
        style.textBaseline = 'middle'; //top hanging middle alphabetic ideographic bottom central
        style.textAnchor = 'middle';

        let offset = new Point(w, h);
        this.addText("Name", this.properties.name, location, offset, style);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(this.shape.angle * Math.PI / 2);
        ctx.translate(-x, -y);
        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;
        ctx.fillStyle = this.style.fillStyle;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.strokeRect(this.shape.x, this.shape.y, this.shape.width, this.shape.height);
        super.draw(ctx);
        ctx.restore();
    }

    public inResizeZone(point: IPoint): number {
        let ret: number = -1;

        let p1: IPoint = new Point(this.shape.x, this.shape.y + this.shape.height / 2);
        let p2: IPoint = new Point(this.shape.x + this.shape.width, this.shape.y + this.shape.height / 2);
        let p3: IPoint = new Point(this.shape.x + this.shape.width / 2, this.shape.y);
        let p4: IPoint = new Point(this.shape.x + this.shape.width / 2, this.shape.y + this.shape.height);


        if (p1.distance(point) < this.selectionZoneWidth)
            ret = 0;
        if (p2.distance(point) < this.selectionZoneWidth)
            ret = 1;
        if (p3.distance(point) < this.selectionZoneWidth)
            ret = 2;
        if (p4.distance(point) < this.selectionZoneWidth)
            ret = 3;

        return ret;
    }

    public resizeToLocation(to: IPoint, handle: number) {
        if (handle === 0) {
            let x2 = this.shape.x + this.shape.width;
            let x1 = to.x;
            let width = x2 - x1;
            this.shape.x = x1;
            this.shape.width = width;

            let t = this.findText("Name");
            if (t) {
                let w = this.shape.width / 2;
                let h = this.shape.height / 2;
                t.offset = new Point(w, h);
                t.location = new Point(this.shape.x, this.shape.y);
            }
        }
        else if (handle === 1) {
            this.shape.width = to.x - this.shape.x;

            let t = this.findText("Name");
            if (t) {
                let w = this.shape.width / 2;
                let h = this.shape.height / 2;
                t.offset = new Point(w, h);
            }
        }
        else if (handle === 2) {
            let y2 = this.shape.y + this.shape.height;
            let y1 = to.y;
            let height = y2 - y1;
            this.shape.y = y1;
            this.shape.height = height;

            let t = this.findText("Name");
            if (t) {
                let w = this.shape.width / 2;
                let h = this.shape.height / 2;
                t.offset = new Point(w, h);
                t.location = new Point(this.shape.x, this.shape.y);
            }
        }
        else if (handle === 3) {
            this.shape.height = to.y - this.shape.y;

            let t = this.findText("Name");
            if (t) {
                let w = this.shape.width / 2;
                let h = this.shape.height / 2;
                t.offset = new Point(w, h);
            }
        }
    }

    public getClickLocationAction(mousePoint: Point, tool: DrawingToolType): CanvasEngineAction {
        if (this.contains(mousePoint) && tool === DrawingToolType.Connect)
            return CanvasEngineAction.Connect;
        else if (this.contains(mousePoint) && tool === DrawingToolType.ConnectLine)
            return CanvasEngineAction.ConnectLine;
        else if (this.inResizeZone(mousePoint) !== -1)
            return CanvasEngineAction.Resize;
        else if (this.contains(mousePoint))
            return CanvasEngineAction.Move;
        else
            return CanvasEngineAction.Pan;

        // Changed
        //return CanvasEngineAction.None;
        //return CanvasEngineAction.Pan;
    }

    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
        if (this.contains(mousePoint) && (tool === DrawingToolType.Connect || tool === DrawingToolType.ConnectLine))
            return "pointer";
        else if (this.inResizeZone(mousePoint) === 0)
            return "w-resize";
        else if (this.inResizeZone(mousePoint) === 1)
            return "e-resize";
        else if (this.inResizeZone(mousePoint) === 2)
            return "n-resize";
        else if (this.inResizeZone(mousePoint) === 3)
            return "s-resize";

        else
            return "move";
    }

    public toSVG(): string {
        var markup: Array<string> = [];
        markup.push('\t<g ', this.getSvgId(), this.getSvgId(), '>\n');
        markup.push(
            '\t\t<rect ', this.getSvgId(),
            ' x="', this.shape.x + '"',
            ' y="', this.shape.y + '"',
            ' width="', this.shape.width + '"',
            ' height="', this.shape.height + '"',
            ' style="', SVGUtil.getSvgStyles(this.style) + '"',
            '/>\n');

        if (this.texts)
            for (let t of this.texts)
                markup.push(t.toSVG());

        markup.push('\t</g>\n');
        return markup.join('');
    }

    public get Schema(): ISchema {
        let modeText: string = "";
        switch (this.mode) {
            case "network":
                modeText = "برق منطقه ای";
                break;
            case "area":
                modeText = "ناحیه";
                break;
            case "zone":
                modeText = "ایستگاه";
                break;
            default:
                modeText = "نامعلوم";
                break;
        }

        let schema: ISchema;

        if (this.mode === "network") {
            schema = {
                title: `ویژگی ${modeText}`,
                type: "object",
                tabs: [
                    {
                        tabIndex: 1,
                        tabName: 'عمومی',
                        properties: {
                            "name": {
                                "type": "string",
                                "title": "نام"
                            },
                            "description": {
                                "type": "string",
                                "format": "textarea",
                                "title": "شرح",
                            },
                            "width": {
                                "type": "integer",
                                "minimum": 0,
                                "title": "عرض",
                                "disabled": true,
                            },
                            "height": {
                                "type": "integer",
                                "minimum": 0,
                                "title": "طول",
                                "disabled": true,
                            },
                            "abroad": {
                                "type": "boolean",
                                "title": "برون مرزی",
                            }
                        }
                    }
                ]
            };
        }
        else if (this.mode === "zone") {
            schema = {
                title: `ویژگی ${modeText}`,
                type: "object",
                tabs: [
                    {
                        tabIndex: 1,
                        tabName: 'عمومی',
                        properties: {
                            "name": {
                                "type": "string",
                                "title": "نام"
                            },
                            "description": {
                                "type": "string",
                                "format": "textarea",
                                "title": "شرح",
                            },
                            "igmcStationId": {
                                "type": "string",
                                "enum": UtilityService.IGMCSubstations,
                                "required": true,
                                "title": "شماره ایستگاه"
                            },
                            "width": {
                                "type": "integer",
                                "minimum": 0,
                                "title": "عرض",
                                "disabled": true,
                            },
                            "height": {
                                "type": "integer",
                                "minimum": 0,
                                "title": "طول",
                                "disabled": true,
                            }
                        }
                    }
                ]
            };
        }
        else {
            schema = {
                title: `ویژگی ${modeText}`,
                type: "object",
                tabs: [
                    {
                        tabIndex: 1,
                        tabName: 'عمومی',
                        properties: {
                            "name": {
                                "type": "string",
                                "title": "نام"
                            },
                            "description": {
                                "type": "string",
                                "format": "textarea",
                                "title": "شرح",
                            },
                            "width": {
                                "type": "integer",
                                "minimum": 0,
                                "title": "عرض",
                                "disabled": true,
                            },
                            "height": {
                                "type": "integer",
                                "minimum": 0,
                                "title": "طول",
                                "disabled": true,
                            },
                        }
                    }
                ]
            };
        }

        return schema;
    }

    public get Properties(): IBoxedConnectShapeProperties {
        this.properties.width = this.shape.width;
        this.properties.height = this.shape.height;
        return this.properties;
    }

    public set Properties(prop: IBoxedConnectShapeProperties) {
        this.properties = prop;
        this.name = prop.name
    }

    protected _drawHandle(ctx: CanvasRenderingContext2D) {
        let d = this.selectionZoneWidth;
        if (this.isSelected) {
            ctx.globalAlpha = this.style.opacity;
            ctx.strokeStyle = "#00FFFF";
            ctx.lineWidth = 1;

            let p: IPoint = new Point(this.shape.x, this.shape.y + this.shape.height / 2);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);

            p = new Point(this.shape.x + this.shape.width, this.shape.y + this.shape.height / 2);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);

            p = new Point(this.shape.x + this.shape.width / 2, this.shape.y);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);

            p = new Point(this.shape.x + this.shape.width / 2, this.shape.y + this.shape.height);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
        }
    }
}
