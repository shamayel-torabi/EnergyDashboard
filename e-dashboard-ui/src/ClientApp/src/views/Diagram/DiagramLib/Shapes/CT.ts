import { IPoint } from './interfaces/IShapes';
import { SVGUtil } from '../Util/SVGUtil';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces/ISchema';
import { IProperties } from './interfaces';
import { BoxedBranch } from './BoxedBranch';
import { Point } from './Shapes';

interface IMeter {
    meterId: number;
    name: string;
    serialNumber:string;
    mountDate: string;
    dismountDate: string;
    active: boolean;
}

interface ICTProperties extends IProperties {
    dispachingCode: string;
    igmcCode: string;
    ratioCT: string;
    ratioPT: string;
    reverse: boolean;
    meters: IMeter[];
}

export class CTProperties implements ICTProperties {
    public name: string;
    public voltage: number;
    public dispachingCode: string;
    public igmcCode: string;
    public ratioCT: string;
    public ratioPT: string;
    public reverse: boolean;
    public meters: IMeter[];


    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.dispachingCode = "";
        this.igmcCode = "";
        this.ratioCT = "100/5";
        this.ratioPT = "63000/110";
        this.reverse = false;
        this.meters = [];
    }
}

export class CT extends BoxedBranch {
    protected properties: ICTProperties;

    constructor(name: string, x: number, y: number) {
        super(name, x, y);
        this.$type = "CT";
        this.properties = new CTProperties(name);

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


        //style = new TextStyle();
        //style.textAlign = 'end'; // start, end, left, right, center
        //style.textBaseline = 'middle'; //top hanging middle alphabetic ideographic bottom central
        //style.textAnchor = 'end';

        //offset = new Point(this.shape.width + this.selectionZoneWidth, h - 4);
        //this.addText("P", `P=${this.properties.voltage} Mw`, location, offset, style);

        //offset = new Point(this.shape.width + this.selectionZoneWidth, h + 4);
        //this.addText("Q", `Q=${this.properties.voltage} MVar`, location, offset, style);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let dx = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(this.shape.angle * Math.PI / 2);
        ctx.translate(-x, -y);

        ctx.strokeStyle = this.style.strokeStyle;
        ctx.lineWidth = this.style.strokeWidth;

        ctx.beginPath();
        ctx.moveTo(this.shape.x + dx, this.shape.y);
        ctx.lineTo(this.shape.x + dx, this.shape.y + 4 * dx);
        ctx.closePath();

        ctx.moveTo(this.shape.x + 2 * dx, this.shape.y + 2 * dx);
        ctx.arc(this.shape.x + dx, this.shape.y + 2 * dx, dx, 0, 2 * Math.PI);
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
        var p2 = new Point(this.shape.x + dx, this.shape.y + 4 * dx);
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
            title: "ویژگی ترانس جریان",
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
                        }
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
                        "ratioCT": {
                            "type": "string",
                            "title": "نسبت تبدیل جریان"
                        },
                        "ratioPT": {
                            "type": "string",
                            "title": "نسبت تبدیل ولتاژ"
                        },
                        "reverse": {
                            "type": "boolean",
                            "title": "معکوس",
                        }
                    }
                },
                {
                    tabIndex: 3,
                    tabName: 'میتر',
                    properties: {
                        "meters": {
                            "type": "meterform",
                            "title": 'فهرست میتر',
                        },
                    }
                }
            ]
        };
        return schema;
    }

    public get Properties(): ICTProperties {
        return this.properties;
    }

    public set Properties(prop: ICTProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
