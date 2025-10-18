import { IPoint } from './interfaces/IShapes';
import { SVGUtil } from '../Util/SVGUtil';
import { BoxedShape } from './BoxedShape';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces/ISchema';
import { IProperties } from './interfaces';
import { Point } from './Shapes';
import { UtilityService } from '../Services';

interface IGeneratorProperties extends IProperties {
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    capacity: number;
    nominalCapacity: number;
    genSize: number;
    genType: number;
    operator: number;
}

export class GeneratorProperties implements IGeneratorProperties {
    name: string;
    voltage:number;
    dispachingCode: string;
    toolId: number;
    igmcCode: string;
    capacity: number;
    nominalCapacity: number;
    genSize: number;
    genType: number;
    operator: number;

    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.dispachingCode = "";
        this.igmcCode = "";
        this.toolId = 0;
        this.capacity = 0;
        this.nominalCapacity = 0;
        this.genSize = 1;
        this.genType = 1;
        this.operator = 1;
    }
}

export class Generator extends BoxedShape {
    protected properties: IGeneratorProperties;

    constructor(name: string, x: number, y: number) {
        super(name, x, y, 20 , 30);
        this.$type = "Generator";
        this.properties = new GeneratorProperties(name);

        var location = new Point(x, y);

        let w: number = this.shape.width / 2;
        let h: number = this.shape.height / 3;

        var offset: IPoint = new Point(w, 0);
        this.addPort("1", location, offset);

        var style = new TextStyle();
        style.textAlign = 'center'; // start, end, left, right, center
        style.textBaseline = 'hanging'; //top hanging middle alphabetic ideographic bottom central

        offset = new Point(w, this.shape.height + this.selectionZoneWidth);
        this.addText("Name", this.properties.name, location, offset, style);

        offset = new Point(w, this.shape.height + 12);
        this.addText("P", `P=${this.properties.voltage} Mw`, location, offset, style);

        offset = new Point(w, this.shape.height + 24);
        this.addText("Q", `Q=${this.properties.voltage} MVar`, location, offset, style);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let d = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(this.shape.angle * Math.PI / 2);
        ctx.translate(-x, -y);

        ctx.strokeStyle = this.style.strokeStyle;
        ctx.lineWidth = this.style.strokeWidth;

        ctx.beginPath();
        ctx.moveTo(this.shape.x + d, this.shape.y);
        ctx.lineTo(this.shape.x + d, this.shape.y + d);
        ctx.closePath();

        ctx.moveTo(this.shape.x + 2 * d, this.shape.y + 2 * d);
        ctx.arc(this.shape.x + d, this.shape.y + 2 * d, d, 0, 2 * Math.PI);
        ctx.stroke();
        ctx.closePath();

        this.drawAlt(ctx, this.shape.x + this.selectionZoneWidth, this.shape.y + 2 * d, this.shape.width - 2 * this.selectionZoneWidth, d);

        super.draw(ctx);
        ctx.restore();
    }

    public rotate(angle: number): void {
        let poffset: IPoint;
        let toffset: IPoint;
        this.shape.angle = angle;

        switch (this.shape.angle) {
            case 0:
                poffset = new Point(this.shape.width / 2, 0);
                toffset = new Point(this.shape.width / 2, this.shape.height + this.selectionZoneWidth);
                break;
            case 1:
                poffset = new Point(this.shape.width / 2 + this.shape.height / 2, this.shape.height / 2);
                toffset = new Point(this.shape.width / 2 - this.shape.height / 2 - 2 * this.selectionZoneWidth, this.shape.height / 2);
                break;
            case 2:
                poffset = new Point(this.shape.width / 2, this.shape.height);
                toffset = new Point(this.shape.width / 2, - 4 * this.selectionZoneWidth);
                break;
            case 3:
                poffset = new Point(this.shape.width / 2 - this.shape.height / 2, this.shape.height / 2);
                toffset = new Point(this.shape.width / 2 + this.shape.height / 2 + 2 * this.selectionZoneWidth, this.shape.height / 2);
                break;
            default:
                poffset = new Point(this.shape.width / 2, 0);
                toffset = new Point(this.shape.width / 2, this.shape.height + this.selectionZoneWidth);
                break;
        }
        this.ports[0].offset = poffset;
        this.texts[0].offset = toffset;
    }

    private drawAlt(ctx: CanvasRenderingContext2D, x: number, y: number, w: number, h: number) {
        ctx.beginPath();
        ctx.moveTo(x, y);
        ctx.quadraticCurveTo(x + w / 4, y + h, x + w / 2, y);
        ctx.moveTo(x + w / 2, y);
        ctx.quadraticCurveTo(x + 3 * w / 4, y - h, x + w, y);
        ctx.stroke();
        ctx.closePath();
    }

    public toSVG(): string {
        var markup: Array<string> = [];
        let dx = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        markup.push('\t<g ', this.getSvgId(), 'transform="rotate(' + this.shape.angle * 90 + ',' + x + ',' + y + ')"', this.getSvgId(), '>\n');
        var p1 = new Point(this.shape.x + dx, this.shape.y);
        var p2 = new Point(this.shape.x + dx, this.shape.y + dx);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );
        p1 = new Point(this.shape.x + dx, this.shape.y + 2 * dx);
        markup.push(
            '\t\t<circle ',
            'cx="' + p1.x + '" cy="' + p1.y + '" ',
            'r="', dx + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n');

        if (this.texts)
            for (let t of this.texts)
                markup.push(t.toSVG());

        markup.push('\t</g>\n');
        return markup.join('');
    }
    public get Schema(): ISchema {
        let schema = {
            title: "ویژگی ژنراتور",
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
                            "type": "integer",
                            "title": "شناسه دیسپاچینگ"
                        },
                        "igmcCode": {
                            "type": "integer",
                            "title": "شناسه مدیریت شبکه"
                        },
                        "toolId": {
                            "type": "integer",
                            "minimum": 0,
                            "title": "شناسه سنجش"
                        },
                    }
                },
                {
                    tabIndex: 2,
                    tabName: 'سنجش و پایش',
                    properties: {
                        "voltage": {
                            "type": "number",
                            "title": "اندازه ولتاژ (KV)",
                            "disabled": true,
                        },
                        "capacity": {
                            "type": "number",
                            "title": "ظرفیت (MW)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*"
                        },
                        "nominalCapacity": {
                            "type": "number",
                            "title": "ظرفیت نامی (MW)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*"
                        },
                        "genSize": {
                            "type": "string",
                            "enum": [
                                { value: 1, label: "معمولی" },
                                { value: 2, label: "تولید پراکنده" }
                            ],
                            "title": "نوع"
                        },
                        "genType": {
                            "type": "string",
                            "enum": UtilityService.PowerPlantTypes,
                            "title": "تکنولوژی تولید"
                        },
                        "operator": {
                            "type": "string",
                            "enum": UtilityService.PowerplantOperators,
                            "title": "شرکت بهره بردار"
                        },
                    }
                },
            ]
        };
        return schema;
    }

    public get Properties(): IGeneratorProperties {
        return this.properties;
    }

    public set Properties(prop: IGeneratorProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
