import { IPoint } from './interfaces/IShapes';
import { SVGUtil } from '../Util/SVGUtil';
import { BoxedShape } from './BoxedShape';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces/ISchema';
import { IProperties } from './interfaces';
import { Point } from './Shapes';
import { UtilityService } from '../Services';

interface ILoadFeederProperties extends IProperties {
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    maxDemand: number;
    feederType: string;
    loadFeederType: number;
    loadFeederSubType: number;
}



export class LoadFeederProperties implements ILoadFeederProperties {
    name: string;
    voltage:number;
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    maxDemand: number;
    feederType: string;
    loadFeederType: number;
    loadFeederSubType: number;

    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.dispachingCode = "";
        this.igmcCode = "";
        this.toolId = 0;
        this.maxDemand = 7.0;
        this.feederType = '1-1';
        this.loadFeederType = 1;
        this.loadFeederSubType = 1;
    }
}

export class LoadFeeder extends BoxedShape {
    protected properties: ILoadFeederProperties;

    constructor(name: string, x: number, y: number) {
        super(name, x, y, 10, 20);
        this.$type = "LoadFeeder";
        this.properties = new LoadFeederProperties(name);

        var location = new Point(x, y);

        this.ports = [];
        this.texts = [];

        let w: number = this.shape.width / 2;
        //let h: number = this.shape.height / 2;

        var offset: IPoint = new Point(w, 0);
        this.addPort("1", location, offset);

        var style = new TextStyle();
        style.textAlign = 'center'; // start, end, left, right, center
        style.textBaseline = 'hanging'; //top hanging middle alphabetic ideographic bottom central

        offset = new Point(w, this.shape.height);
        this.addText("Name", this.properties.name, location, offset, style);

        offset = new Point(w, this.shape.height + 12);
        this.addText("P", `P=${this.properties.voltage} Mw`, location, offset, style);

        offset = new Point(w, this.shape.height + 24);
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
        ctx.moveTo(this.shape.x + d, this.shape.y);
        ctx.lineTo(this.shape.x + d, this.shape.y + 3 * d);
        ctx.stroke();
        ctx.closePath();

        ctx.beginPath();
        ctx.moveTo(this.shape.x, this.shape.y + 2 * d);
        ctx.lineTo(this.shape.x + d, this.shape.y + 3 * d);
        ctx.lineTo(this.shape.x + 2 * d, this.shape.y + 2 * d);
        ctx.lineTo(this.shape.x, this.shape.y + 2 * d);
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
                offset = new Point(this.shape.width / 2, 0);
                break;
            case 1:
                offset = new Point(this.shape.width / 2 + this.shape.height / 2, this.shape.height / 2);
                break;
            case 2:
                offset = new Point(this.shape.width / 2, this.shape.height);
                break;
            case 3:
                offset = new Point(this.shape.width / 2 - this.shape.height / 2, this.shape.height / 2);
                break;
            default:
                offset = new Point(this.shape.width / 2, 0);
                break;
        }
        this.ports[0].offset = offset;
    }

    public toSVG(): string {
        let markup: Array<string> = [];
        let d = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        markup.push('\t<g ', this.getSvgId(), 'transform="rotate(' + this.shape.angle * 90 + ',' + x + ',' + y + ')"', this.getSvgId(), '>\n');
        let p1 = new Point(this.shape.x + d, this.shape.y);
        let p2 = new Point(this.shape.x + d, this.shape.y + 3 * d);
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
        p2 = new Point(this.shape.x + d, this.shape.y + 3 * d);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );

        p1 = new Point(this.shape.x + d, this.shape.y + 3 * d);
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

        p1 = new Point(this.shape.x + 2 * d, this.shape.y + 2 * d);
        p2 = new Point(this.shape.x, this.shape.y + 2 * d);
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
            title: "ویژگی فیدر بار",
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
                        "voltage": {
                            "type": "number",
                            "title": "اندازه ولتاژ (KV)",
                            "disabled": true,
                        },
                        "maxDemand": {
                            "type": "number",
                            "title": "ماگزیمم دیماند (MW)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*"
                        },
                        "feederType": {
                            "type": "string",
                            "title": "نوع فیدر",
                            "group": UtilityService.LoadFeederTypes,
                        },
                    }
                }
            ]
        };
        return schema;
    }

    public get Properties(): ILoadFeederProperties {
        let prop: ILoadFeederProperties = {
            name: this.properties.name,
            voltage: this.properties.voltage,
            dispachingCode: this.properties.dispachingCode,
            igmcCode: this.properties.igmcCode,
            toolId: this.properties.toolId,
            maxDemand: this.properties.maxDemand,
            feederType: this.properties.loadFeederType + '-' + this.properties.loadFeederSubType,
            loadFeederType: this.properties.loadFeederType,
            loadFeederSubType: this.properties.loadFeederSubType,
        };

        return prop;
    }

    public set Properties(prop: ILoadFeederProperties) {
        let feederType = prop.feederType;

        if (feederType) {
            let ft = feederType.split("-");
            prop.loadFeederType = parseInt(ft[0]);
            prop.loadFeederSubType = parseInt(ft[1]);
        }

        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
