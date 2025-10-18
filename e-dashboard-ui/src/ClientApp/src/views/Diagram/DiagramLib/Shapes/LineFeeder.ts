import { IPoint } from './interfaces/IShapes';
import { DrawingToolType, CanvasEngineAction, LineType } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { BoxedShape } from './BoxedShape';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces/ISchema';
import { IProperties } from './interfaces';
import { Point } from './Shapes';

interface ILineFeederProperties extends IProperties {
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    transferCapacity: number;
    lineFeederType: LineType;
}

export class LineFeederProperties implements ILineFeederProperties {
    name: string;
    voltage: number;
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    transferCapacity: 0.0;
    lineFeederType: LineType;

    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.dispachingCode = "";
        this.igmcCode = "";
        this.toolId = 0;
        this.transferCapacity = 0;
        this.lineFeederType = LineType.Unknown;
    }
}

export class LineFeeder extends BoxedShape {
    protected properties: ILineFeederProperties;
    public connectPortId: string | null;
    public connectShapeId: string | null;

    constructor(name: string, x: number, y: number) {
        super(name, x, y, 10, 20);
        this.$type = "LineFeeder";
        this.properties = new LineFeederProperties(name);

        this.connectPortId = null;
        this.connectShapeId = null;

        let location = new Point(x, y);

        let w: number = this.shape.width / 2;
        let h: number = this.shape.height;

        let offset: IPoint = new Point(w, h);
        this.addPort("1", location, offset);

        let style = new TextStyle();
        style.textAlign = 'center'; // start, end, left, right, center
        style.textBaseline = 'bottom'; //top hanging middle alphabetic ideographic bottom central

        offset = new Point(w, - this.selectionZoneWidth);

        this.addText("Name", this.properties.name, location, offset, style);

        offset = new Point(w, this.shape.height - 26);
        this.addText("P", `P=${this.properties.voltage} Mw`, location, offset, style);

        offset = new Point(w, this.shape.height - 14);
        this.addText("Q", `Q=${this.properties.voltage} MVar`, location, offset, style);

        //this.rotate(0);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let d = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(this.shape.angle * Math.PI / 2);
        ctx.translate(-x, -y);

        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;
        ctx.lineWidth = this.style.strokeWidth;

        ctx.beginPath();
        ctx.moveTo(this.shape.x + d, this.shape.y + d);
        ctx.lineTo(this.shape.x + d, this.shape.y + 4 * d);
        ctx.stroke();
        ctx.closePath();

        ctx.beginPath();
        ctx.moveTo(this.shape.x, this.shape.y + 2 * d);
        ctx.lineTo(this.shape.x + d, this.shape.y + d);
        ctx.lineTo(this.shape.x + 2 * d, this.shape.y + 2 * d);
        ctx.stroke();
        ctx.closePath();

        ctx.beginPath();
        ctx.moveTo(this.shape.x, this.shape.y + d);
        ctx.lineTo(this.shape.x + d, this.shape.y);
        ctx.lineTo(this.shape.x + 2 * d, this.shape.y + d);
        ctx.stroke();
        ctx.closePath();

        super.draw(ctx);
        ctx.restore();

    }

    public rotate(angle: number): void {
        let offset: IPoint;

        this.shape.angle = angle;

        switch (this.shape.angle) {
            case 0:
                offset = new Point(this.shape.width / 2, this.shape.height);
                break;
            case 1:
                offset = new Point(this.shape.width / 2 - this.shape.height / 2, this.shape.height / 2);
                break;
            case 2:
                offset = new Point(this.shape.width / 2, 0);
                break;
            case 3:
                offset = new Point(this.shape.width / 2 + this.shape.height / 2, this.shape.height / 2);
                break;
            default:
                offset = new Point(this.shape.width / 2, 0);
                break;
        }
        this.ports[0].offset = offset;
    }

    public getClickLocationAction(mousePoint: Point, tool: DrawingToolType): CanvasEngineAction {
        if (this.inPortZone(mousePoint) && tool === DrawingToolType.Connect)
            return CanvasEngineAction.Connect;
        else if (this.inPortZone(mousePoint) && tool === DrawingToolType.ConnectLine)
            return CanvasEngineAction.ConnectLine;
        else if (this.contains(mousePoint))
            return CanvasEngineAction.Move;
        else
            return CanvasEngineAction.Pan;

        //Changed
        //return CanvasEngineAction.None;
        //return CanvasEngineAction.Pan;
    }

    public toSVG(): string {
        var markup: Array<string> = [];
        let d = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        markup.push('\t<g ', this.getSvgId(), 'transform="rotate(' + this.shape.angle * 90 + ',' + x + ',' + y + ')"', this.getSvgId(), '>\n');
        var p1 = new Point(this.shape.x + d, this.shape.y + d);
        var p2 = new Point(this.shape.x + d, this.shape.y + 4 * d);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );

        p1 = new Point(this.shape.x, this.shape.y + 2 * d);
        p2 = new Point(this.shape.x + d, this.shape.y + d);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );

        p1 = new Point(this.shape.x + d, this.shape.y + d);
        p2 = new Point(this.shape.x + 2 * d, this.shape.y + 2 * d);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );


        p1 = new Point(this.shape.x, this.shape.y + d);
        p2 = new Point(this.shape.x + d, this.shape.y);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );

        p1 = new Point(this.shape.x + d, this.shape.y);
        p2 = new Point(this.shape.x + 2 * d, this.shape.y + d);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );


        if (this.texts)
            for (let t of this.texts)
                markup.push(t.toSVG());

        markup.push('\t</g>\n');
        return markup.join('');
    }

    public get Schema(): ISchema {
        let schema = {
            title: "ویژگی فیدر خط",
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
                        "dispachingCode": {
                            "type": "string",
                            "title": "شناسه دیسپاچینگ",
                            "disabled": true,
                        },
                        "igmcCode": {
                            "type": "string",
                            "title": "شناسه مدیریت شبکه",
                            "disabled": true,
                        },
                        "toolId": {
                            "type": "integer",
                            "minimum": 0,
                            "title": "شناسه سنجش",
                            "disabled": true,
                        },
                    }
                },
                {
                    tabIndex: 2,
                    tabName: 'سنجش و پایش',
                    properties: {
                        "voltage": {
                            "type": "number",
                            "title": "ولتاژ (KV)",
                            "disabled": true,
                        },
                        "transferCapacity": {
                            "type": "number",
                            "title": "ظرفیت انتقال (MW)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*",
                            "disabled": true,
                        },
                        "lineFeederType": {
                            "type": "string",
                            "enum": [
                                { value: 0, label: "نا معلوم" },
                                { value: 1, label: "بین ایستکاه" },
                                { value: 2, label: "بین ناحیه" },
                                { value: 3, label: "بین منطقه" },
                                { value: 4, label: "برون مرزی" },
                            ],
                            "title": "نوع فیدر خط",
                            "disabled": true,
                        },
                    }
                }
            ]
        };
        return schema;
    }

    public get Properties(): ILineFeederProperties {
        return this.properties;
    }

    public set Properties(prop: ILineFeederProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
