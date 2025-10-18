import { IPoint } from './interfaces/IShapes';
import { SVGUtil } from '../Util/SVGUtil';
import { BoxedShape } from './BoxedShape';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces/ISchema';
import { IProperties } from './interfaces';
import { Point } from './Shapes';

interface ITransformerProperties extends IProperties {
    dispachingCode: string;
    igmcCode: string;
    primaryVoltage: number;
    secondaryVoltage: number;
    mva: number;
	transofmerType:number;
    toolId: number;
}

export class TransformerProperties implements ITransformerProperties {
    name: string;
    voltage:number;
    dispachingCode: string;
    igmcCode: string;
    primaryVoltage: number;
    secondaryVoltage: number;
    mva: number;
    transofmerType:number;
    toolId: number;

    constructor(name: string, primaryVoltage: number, secondaryVoltage: number, MVA: number) {
        this.name = name;
        this.voltage = 0;
        this.dispachingCode = "";
        this.igmcCode = "";
        this.primaryVoltage = primaryVoltage;
        this.secondaryVoltage = secondaryVoltage;
        this.mva = MVA;
		this.transofmerType = 2; // 1=توزیع , 2 = انتقال,
        this.toolId = 0;
    }
}

export class Transformer extends BoxedShape {
    public strokeStylePrimary: string;
    public strokeStyleSecondary: string;
    protected properties: ITransformerProperties;

    constructor(name: string, x: number, y: number) {
        super(name, x, y, 20, 50);
        this.$type = "Transformer";
        this.strokeStylePrimary = "#000000";
        this.strokeStyleSecondary = "#000000";
        this.properties = new TransformerProperties(name, 0, 0, 0);

        var location = new Point(x, y);

        let w: number = this.shape.width / 2;
        let h: number = this.shape.height / 2;

        var offset: IPoint = new Point(w, 0);
        this.addPort("1", location, offset);

        offset = new Point(w, this.shape.height);
        this.addPort("2", location, offset);

        var style = new TextStyle();
        style.textAlign = 'start'; // start, end, left, right, center
        style.textBaseline = 'middle'; //top hanging middle alphabetic ideographic bottom central
        style.textAnchor = 'start';

        offset = new Point(-this.selectionZoneWidth, h);
        this.addText("Name", this.properties.name, location, offset, style);

        style = new TextStyle();
        style.textAlign = 'end'; // start, end, left, right, center
        style.textBaseline = 'middle'; //top hanging middle alphabetic ideographic bottom central
        style.textAnchor = 'end';

        offset = new Point(this.shape.width + 2 * this.selectionZoneWidth + 12, 0);
        this.addText("P", `P=${0} Mw`, location, offset, style, 90);

        offset = new Point(this.shape.width + 2 * this.selectionZoneWidth, 0);
        this.addText("Q", `Q=${0} MVar`, location, offset, style, 90);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let dx = this.shape.width / 2;
        let dy = this.shape.height / 5;

        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(this.shape.angle * Math.PI / 2);
        ctx.translate(-x, -y);

        ctx.strokeStyle = this.strokeStylePrimary;
        ctx.lineWidth = 1;

        ctx.beginPath();
        ctx.moveTo(this.shape.x + dx, this.shape.y);
        ctx.lineTo(this.shape.x + dx, this.shape.y + dy);
        ctx.stroke();
        ctx.closePath();

        ctx.beginPath();
        ctx.arc(this.shape.x + dx, this.shape.y + 2 * dy, dx, 0, 2 * Math.PI);
        ctx.stroke();
        ctx.closePath();

        ctx.strokeStyle = this.strokeStyleSecondary;
        ctx.beginPath();
        ctx.arc(this.shape.x + dx, this.shape.y + 3 * dy, dx, 0, 2 * Math.PI);

        ctx.moveTo(this.shape.x + dx, this.shape.y + 4 * dy);
        ctx.lineTo(this.shape.x + dx, this.shape.y + 5 * dy);

        ctx.stroke();
        ctx.closePath();

        super.draw(ctx);
        ctx.restore();
    }
    public rotate(angle: number): void {
        let offset1: IPoint;
        let offset2: IPoint;

        this.shape.angle = angle;

        switch (this.shape.angle) {
            case 0:
                offset1 = new Point(this.shape.width / 2, 0);
                offset2 = new Point(this.shape.width / 2, this.shape.height);
                break;
            case 1:
                offset1 = new Point(this.shape.width / 2 + this.shape.height / 2, this.shape.height / 2);
                offset2 = new Point(this.shape.width / 2 - this.shape.height / 2, this.shape.height / 2);
                break;
            case 2:
                offset1 = new Point(this.shape.width / 2, this.shape.height);
                offset2 = new Point(this.shape.width / 2, 0);
                break;
            case 3:
                offset1 = new Point(this.shape.width / 2 - this.shape.height / 2, this.shape.height / 2);
                offset2 = new Point(this.shape.width / 2 + this.shape.height / 2, this.shape.height / 2);
                break;
            default:
                offset1 = new Point(this.shape.width / 2, 0);
                offset2 = new Point(this.shape.width / 2, this.shape.height);
                break;
        }

        this.ports[0].offset = offset1;
        this.ports[1].offset = offset2;
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
            ' style="', this._getPrimarySvgStyles(this.strokeStylePrimary),
            '"/>\n'
        );
        p1 = new Point(this.shape.x + dx, this.shape.y + 2 * dx);
        markup.push(
            '\t\t<circle ',
            'cx="' + p1.x + '" cy="' + p1.y + '" ',
            'r="', dx + '"',
            ' style="', this._getPrimarySvgStyles(this.strokeStylePrimary),
            '"/>\n');
        p1 = new Point(this.shape.x + dx, this.shape.y + 3 * dx);
        markup.push(
            '\t\t<circle ',
            'cx="' + p1.x + '" cy="' + p1.y + '" ',
            'r="', dx + '"',
            ' style="', this._getPrimarySvgStyles(this.strokeStyleSecondary),
            '"/>\n');

        p1 = new Point(this.shape.x + dx, this.shape.y + 4 * dx);
        p2 = new Point(this.shape.x + dx, this.shape.y + 5 * dx);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', this._getPrimarySvgStyles(this.strokeStyleSecondary),
            '"/>\n'
        );

        if (this.texts)
            for (let t of this.texts)
                markup.push(t.toSVG());


        markup.push('\t</g>\n');
        return markup.join('');
    }

    private _getPrimarySvgStyles(str: string) {
        var style: Array<string> = [];
        let stroke = SVGUtil.getSvgColorString('stroke', str);
        let strokeWidth = this.style.strokeWidth ? this.style.strokeWidth : '0';
        let s = 'stroke-width: ' + strokeWidth + '; ';
        style.push(stroke);
        style.push(s);

        if (this.style.fillStyle) {
            let fill = SVGUtil.getSvgColorString('fill', this.style.fillStyle);
            style.push(fill);
        }

        var opacity = typeof this.style.opacity !== 'undefined' ? this.style.opacity : '1';
        var o = 'opacity: ' + opacity + ';';
        style.push(o);
        return style.join('');
    }

    public get Schema(): ISchema {
		        let schema = {
            title: "ویژگی ترانسفورماتور",
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
                            "title": "شناسه دیسپاچینگ"
                        },
                        "igmcCode": {
                            "type": "string",
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
                        "transofmerType": {
                            "type": "string",
                            "enum": [
                                { value: 1, label: "انتقال" },
                                { value: 2, label: "فوق توزیع" },
                            ],
                            "title": "نوع ترانسفورماتور",
                        },
                        "primaryVoltage": {
                            "type": "number",
                            "title": "ولتاژ اولیه (KV)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*",
                            "disabled": true,
                        },
                        "secondaryVoltage": {
                            "type": "number",
                            "title": "ولتاژ ثانویه (KV)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*",
                            "disabled": true,
                        },
                        "mva": {
                            "type": "number",
                            "title": "توان ظاهری (MVA)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*"
                        },
                    }
                }
            ]
        };
        return schema;
    }

    public get Properties(): ITransformerProperties {
        return this.properties;
    }

    public set Properties(prop: ITransformerProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
