import { IPoint } from './interfaces/IShapes';
import { SVGUtil } from '../Util/SVGUtil';
import { TextStyle } from './TextStyle';
import { ISchema } from './interfaces';
import { IProperties } from './interfaces';
import { BoxedBranch } from './BoxedBranch';
import { Point } from './Shapes';

interface IBreakerProperties extends IProperties {
    status: boolean;
    dispachingCode: string;
    igmcCode: string;
}

export class BreakerProperties implements IBreakerProperties {
    public name: string;
    public voltage:number;
    public status: boolean;
    public dispachingCode: string;
    public igmcCode: string;

    constructor(name: string, status: boolean) {
        this.name = name;
        this.voltage = 0;
        this.status = status;
        this.dispachingCode = "";
        this.igmcCode = "";
    }
}


export class Breaker extends BoxedBranch {
    protected properties: IBreakerProperties;

    constructor(name: string, x: number, y: number) {
        super(name, x, y);
        this.$type = "Breaker";

        this.properties = new BreakerProperties(name, true);

        let location = new Point(x, y);

        let w: number = this.shape.width / 2;
        let h: number = this.shape.height / 2;

        var offset: IPoint = new Point(w, 0);
        this.addPort("1", location, offset);

        offset = new Point(w, this.shape.height);
        this.addPort("2", location, offset);

        var style = new TextStyle();
        style.textAlign = 'start'; // start, end, left, right, center
        style.textBaseline = 'middle'; //top hanging middle alphabetic ideographic bottom central
        style.textAnchor = 'right';

        offset = new Point(-this.selectionZoneWidth, h);
        this.addText("Name", this.properties.name, location, offset, style);

        this.rotate(0);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let dx = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(this.shape.angle * Math.PI / 2);
        ctx.translate(-x, -y);

        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.fillStyle = this.style.fillStyle;

        if (this.properties.status) {
            ctx.beginPath();
            ctx.moveTo(this.shape.x + dx, this.shape.y);
            ctx.lineTo(this.shape.x + dx, this.shape.y + this.shape.height);
            ctx.stroke();
            ctx.closePath();

            ctx.strokeRect(this.shape.x, this.shape.y + dx, 2 * dx, 2 * dx);

        } else {
            ctx.beginPath();
            ctx.moveTo(this.shape.x + dx, this.shape.y);
            ctx.lineTo(this.shape.x + dx, this.shape.y + dx);
            ctx.stroke();
            ctx.closePath();

            ctx.strokeRect(this.shape.x, this.shape.y + dx, 2 * dx, 2 * dx);

            ctx.beginPath();
            ctx.moveTo(this.shape.x + dx, this.shape.y + 3 * dx);
            ctx.lineTo(this.shape.x + dx, this.shape.y + 4 * dx);
            ctx.stroke();
            ctx.closePath();
        }


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
        let markup: Array<string> = [];
        let dx = this.shape.width / 2;
        let x = this.shape.x + this.shape.width / 2;
        let y = this.shape.y + this.shape.height / 2;

        markup.push('\t<g ', this.getSvgId(), 'transform="rotate(' + this.shape.angle * 90 + ',' + x + ',' + y + ')"', '>\n');
        let p1 = new Point(this.shape.x + dx, this.shape.y);
        let p2 = new Point(this.shape.x + dx, this.shape.y + dx);
        markup.push(
            '\t\t<line ',
            'x1="', p1.x + '"',
            ' y1="', p1.y + '"',
            ' x2="', p2.x + '"',
            ' y2="', p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );

        p1 = new Point(this.shape.x, this.shape.y + dx);
        markup.push(
            '\t\t<rect',
            ' x="', p1.x + '"',
            ' y="', p1.y + '"',
            ' width="', 2 * dx + '"',
            ' height="', 2 * dx + '"',
            ' style="', SVGUtil.getSvgStyles(this.style) + '"',
            '/>\n');

        p1 = new Point(this.shape.x + dx, this.shape.y + 3 * dx);
        p2 = new Point(this.shape.x + dx, this.shape.y + 4 * dx);
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
            title: "ویژگی کلید قدرت",
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
                        "voltage": {
                            "type": "number",
                            "title": "اندازه ولتاژ (KV)",
                            "disabled": true,
                        },
                        "status": {
                            "type": "boolean",
                            "title": "وضعیت",
                        }
                    }
                }
            ]
        };


        return schema;
    }

    public get Properties(): IBreakerProperties {
        return this.properties;
    }

    public set Properties(prop: IBreakerProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
