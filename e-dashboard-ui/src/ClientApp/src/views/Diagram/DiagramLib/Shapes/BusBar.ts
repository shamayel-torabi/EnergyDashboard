import { IPoint } from './interfaces';
import { ISchema } from './interfaces';
import { IProperties } from './interfaces';
import { TextStyle } from './TextStyle';
import { SVGUtil } from '../Util/SVGUtil';
import { Bus } from './Bus';
import { Point } from './Shapes';

interface IBusBarProperties extends IProperties {
    voltageMag: number;
    voltageAng: number;
    powerBalance: number;
    color: string;
    dispachingCode: string;
    igmcCode: string;
}

export class BusBarProperties implements IBusBarProperties {
    public name: string;
    public voltage:number;
    public voltageMag: number;
    public voltageAng: number;
    public powerBalance: number;
    public color: string;
    public dispachingCode: string;
    public igmcCode: string;

    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.voltageMag = 0.0;
        this.voltageAng = 0.0;
        this.powerBalance = 0.0;
        this.color = "#000000";
        this.dispachingCode = "";
        this.igmcCode = "";
    }
}

export class BusBar extends Bus {
    protected properties: IBusBarProperties;

    constructor(name: string, x: number, y: number, width: number = 100, height: number = 5) {
        super(name, x, y, width, height);
        this.$type = "BusBar";

        this.properties = new BusBarProperties(name);

        this.style.strokeStyle = "#000000";
        this.style.fillStyle = "#000000";
        this.style.strokeWidth = 1;

        var location = new Point(this.shape.x, this.shape.y);

        let style = new TextStyle();
        style.textAlign = 'start'; // start, end, left, right, center
        style.textBaseline = 'middle'; //top hanging middle alphabetic ideographic bottom central
        style.textAnchor = 'start';

        let offset = new Point(-1 * this.selectionZoneWidth, -10);
        this.addText("Name", this.properties.name, location, offset, style);

        offset = new Point(-1 * this.selectionZoneWidth, 2);        
        this.addText("Voltage", `V=${this.properties.voltage} kV`, location, offset, style);

        offset = new Point(-1 * this.selectionZoneWidth, 12);
        this.addText("Loss", `Loss=${this.properties.powerBalance} Mw`, location, offset, style);
    }

    public get color(): string {
        return this.properties.color
    }

    public set color(value: string) {
        this.properties.color = value;
        this.style.strokeStyle = value;
        this.style.fillStyle = value;
    }

    public get loss(): number {
        return this.properties.powerBalance
    }

    public set loss(value: number) {
        this.properties.powerBalance = value;

        let t = this.findText("Loss");
        if (t)
            t.text = `Loss=${value} Mw`;
    }

    public draw(ctx: CanvasRenderingContext2D) {
        ctx.save();
        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;
        ctx.fillStyle = this.style.fillStyle;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.fillRect(this.shape.x, this.shape.y, this.shape.width, this.shape.height);
        super.draw(ctx)
        ctx.restore();
    }
    public resizeToLocation(to: IPoint, handle: number) {
        if (handle === 0) {
            let dx = to.x - this.shape.x;
            let x2 = this.shape.x + this.shape.width;
            let x1 = to.x;
            let width = x2 - x1
            this.shape.x = x1;
            this.shape.width = width;

            let t = new Point(to.x, this.shape.y);
            if (this.texts) {
                for (let text of this.texts)
                    text.move(t);
            }

            if(this.ports){
                for(let port of this.ports){
                    let of = new Point(port.offset.x - dx, port.offset.y);                    
                    port.offset = of;
                }
            }
        }
        else if (handle === 1) {
            this.shape.width = to.x - this.shape.x;
        }
    }

    public toSVG(): string {
        let markup:string[] = [];
        markup.push(`\t<g ${this.getSvgId()}>\n`);
        markup.push('\t<rect', ' x="', this.shape.x + '"', ' y="', this.shape.y + '"', ' width="', this.shape.width + '"', ' height="', this.shape.height + '"', ' style="', SVGUtil.getSvgStyles(this.style) + '"', '/>\n');
        if (this.texts)
            for (let t of this.texts)
                markup.push(t.toSVG());
        markup.push('\t</g>\n');
        return markup.join('');
    }

    public get Schema(): ISchema {
        let schema = {
            title: "ویژکی باسبار",
            type: "object",
            tabs: [
                {
                    tabIndex: 1,
                    tabName: 'عمومی',
                    properties: {
                        "name": {
                            "type": "string",
                            "required": true,
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
                        "voltageMag": {
                            "type": "number",
                            "title": "اندازه ولتاژ (Kv)",
                            "pattern": "[-+]?[0-9]*\.?[0-9]*"

                        },
                        "voltageAng": {
                            "type": "number",
                            "title": "زاویه ولتاژ",
                            "pattern": '[-+]?[0-9]*\.?[0-9]*'
                        },
                        "color": {
                            "type": "string",
                            "format": "vcolor",
                            "title": "رنگ"
                        },
                    }
                }
            ]
        };


        return schema;
    }

    get Properties(): IBusBarProperties {
        return this.properties;
    }

    set Properties(prop: IBusBarProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;

        if (this.notifyPropertyChange) {
           let p = {
               shapeId: this.id,
               color: prop.color,
               voltage: prop.voltageMag,
           };
           this.notifyPropertyChange(p);
        }
    }
}
