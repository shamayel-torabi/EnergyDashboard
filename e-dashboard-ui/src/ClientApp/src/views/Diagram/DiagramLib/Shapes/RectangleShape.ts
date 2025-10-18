import { IPoint } from './interfaces/IShapes';
import { DrawingShape } from './DrawingShape';
import { IProperties } from './interfaces';
import { Style } from './Style';
import { DrawingToolType } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { ISchema } from './interfaces/ISchema';
import { Rectangle, Point } from './Shapes';

interface IRectangleShapeProperties extends IProperties {
    lineWidth: number;
    strokeStyle: string;
}

export class RectangleShapeProperties implements IRectangleShapeProperties {
    name: string;
    voltage:number;
    lineWidth: number;
    strokeStyle: string;

    constructor(name: string, lineWidth: number, strokeStyle: string) {
        this.name = name;
        this.voltage = 0;
        this.lineWidth = lineWidth;
        this.strokeStyle = strokeStyle;
    }
}


export class RectangleShape extends DrawingShape<Rectangle> {
    protected properties: IRectangleShapeProperties;

    constructor(name: string, x: number, y: number, width: number, height: number) {
        super(name);
        this.$type = "RectangleShape";
        this.shape = new Rectangle(x, y, width, height);
        this.style = new Style();
        this.properties = new RectangleShapeProperties(name, this.style.strokeWidth, this.style.strokeStyle);
    }

    public get location(): IPoint {
        return new Point(this.shape.x, this.shape.y);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;
        ctx.fillStyle = this.style.fillStyle;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.strokeRect(this.shape.x, this.shape.y, this.shape.width, this.shape.height);
        this._drawHandle(ctx);
    }

    public move(to: IPoint) {
        this.shape.x = to.x;
        this.shape.y = to.y;
        if (this.ports) {
            for (let port of this.ports)
                port.move(to);
        }

        if (this.texts) {
            for (let text of this.texts)
                text.move(to);
        }
    }
    public inResizeZone(point: IPoint): number {
        let ret: number = -1;

        let p1: IPoint = new Point(this.shape.x, this.shape.y + this.shape.height / 2);
        let p2: IPoint = new Point(this.shape.x + this.shape.width, this.shape.y + this.shape.height / 2);

        if (p1.distance(point) < this.selectionZoneWidth)
            ret = 0;
        if (p2.distance(point) < this.selectionZoneWidth)
            ret = 1;
        return ret;
    }
    public resizeToLocation(to: IPoint, handle: number) {
        if(handle === 0){
            let x2 = this.shape.x + this.shape.width;
            let x1 = to.x;
            let width = x2 - x1
            this.shape.x = x1;
            this.shape.width = width ;
        }
        else if(handle === 1){
            this.shape.width = to.x - this.shape.x;
        }

    }
    public contains(mousePoint: IPoint): boolean {
        return this.shape.contains(mousePoint);
    }
    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.x, mousePosition.y - this.shape.y);
    }
    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
        if (this.inResizeZone(mousePoint) === 0)
            return "w-resize";
        else if (this.inResizeZone(mousePoint) === 1)
            return "e-resize";
        else
            return "move";
    }
    public toSVG(): string {
        var markup: Array<string> = [];

        markup.push(
            '\t<rect ', this.getSvgId(),
            ' x="', this.shape.x + '"',
            ' y="', this.shape.y + '"',
            ' width="', this.shape.width + '"',
            ' height="', this.shape.height + '"',
            ' style="', SVGUtil.getSvgStyles(this.style) + '"',
            '/>\n');
        return markup.join('');
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
            // p = new Point(this.shape.x, this.shape.y + this.shape.height);
            // ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
            // p = new Point(this.shape.x + this.shape.width, this.shape.y + this.shape.height);
            // ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
        }
    }

    public get Schema(): ISchema {
        let schema = {
            title: "ویژگی مستطیل",
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
                        "lineWidth": {
                            "type": "integer",
                            "title": "ضخامت",
                        },
                        "strokeStyle": {
                            "type": "string",
                            "format": "color",
                            "title": "رنگ"
                        }
                    }
                }
            ]
        };

        return schema;
    }

    public get Properties(): IRectangleShapeProperties {
        return this.properties;
    }

    public set Properties(prop: IRectangleShapeProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }
}
