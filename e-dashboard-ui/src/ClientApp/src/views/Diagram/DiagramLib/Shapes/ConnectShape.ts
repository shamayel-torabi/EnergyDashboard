import { IPoint } from './interfaces/IShapes';
import { LineConnectShape } from './LineConnectShape';
import { DrawingToolType } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { IProperties } from './interfaces';
import { ISchema } from './interfaces/ISchema';
import { Line, Point } from './Shapes';

interface IConnectShapeProperties extends IProperties {
    srcShapeName: string | undefined;
    desShapeName: string | undefined;
}

export class ConnectShapeProperties implements IConnectShapeProperties {
    public name: string;
    public voltage:number;
    public srcShapeName: string | undefined;
    public desShapeName: string | undefined;


    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.srcShapeName = undefined;
        this.desShapeName = undefined;
    }
}

export class ConnectShape extends LineConnectShape<Line> {
    protected properties : IConnectShapeProperties;
    
    constructor(name: string, x: number, y: number) {
        super(name);
        this.$type = "ConnectShape";
        this.shape = new Line(new Point(x, y), new Point(x+1, y+1));
        this.properties = new ConnectShapeProperties(name);
    }

    public get location(): IPoint {
        return new Point(this.shape.p1.x, this.shape.p1.y);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        ctx.lineWidth = this.style.strokeWidth;
        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;

        ctx.beginPath();
        ctx.moveTo(this.shape.p1.x, this.shape.p1.y);
        ctx.lineTo(this.shape.p2.x, this.shape.p2.y);
        ctx.stroke();
        this._drawHandle(ctx);
    }
    public move(to: IPoint) {
    }
    public inResizeZone(point: IPoint): number {
        return -1;
    }
    public resizeToLocation(to: IPoint, handle: number) {
        if (handle === 0) {
            this.shape.p1.x = to.x;
            this.shape.p1.y = to.y;
        }
        else {
            this.shape.p2.x = to.x;
            this.shape.p2.y = to.y;
        }
    }

    public contains(mousePoint: IPoint): boolean {
        return this.shape.contains(mousePoint);
    }
    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.p1.x, mousePosition.y - this.shape.p1.y);
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
            '\t<line ', this.getSvgId(),
            'x1="', this.shape.p1.x + '"',
            ' y1="', this.shape.p1.y + '"',
            ' x2="', this.shape.p2.x + '"',
            ' y2="', this.shape.p2.y + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n'
        );
        return markup.join('');
    }

    get Schema(): ISchema {
        let schema: ISchema = {
            title: "ویژگی پیوند",
            type: "object",
            tabs: [
                {
                    tabIndex: 1,
                    tabName: 'عمومی',
                    properties: {
                        "name": {
                            "type": "string",
                            "title": "نام",
                            "disabled": true,
                        },
                        "srcShapeName": {
                            "type": "string",
                            "title": "مبدا",
                            "disabled": true,
                        },
                        "desShapeName": {
                            "type": "string",
                            "title": "مقصد",
                            "disabled": true,
                        },
                    }
                }
            ]
        };

        return schema;
    }

    public get Properties(): IConnectShapeProperties {
        return this.properties;
    }

    public set Properties(prop: IConnectShapeProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }

    protected _drawHandle(ctx: CanvasRenderingContext2D) {
        if (this.isSelected) {
            var d = this.selectionZoneWidth;
            ctx.globalAlpha = this.style.opacity;
            ctx.strokeStyle = "#00FFFF";
            ctx.lineWidth = 1;
            ctx.strokeRect(this.shape.p1.x - d, this.shape.p1.y - 2, 2 * d, 2 * d);
            ctx.strokeRect(this.shape.p2.x - d, this.shape.p2.y - 2, 2 * d, 2 * d);
        }
    }
}
