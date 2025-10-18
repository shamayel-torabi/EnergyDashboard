import { IPoint } from './interfaces';
import { DrawingToolType, LineType } from '../Enums';
import { ISchema } from './interfaces';
import { IProperties } from './interfaces';
import { LineConnectShape } from './LineConnectShape';
import { SVGUtil } from '../Util/SVGUtil';
import { PolyLine, Point } from './Shapes';

interface ITransmisionLineProperties extends IProperties {
    voltage: number;
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    transferCapacity: number;
    lineFeederType: LineType;
    srcShapeName: string | null;
    desShapeName: string | null;
}


export class TransmisionLineProperties implements ITransmisionLineProperties {
    name: string;
    voltage: number;
    dispachingCode: string;
    igmcCode: string;
    toolId: number;
    transferCapacity: number;
    lineFeederType: LineType;
    srcShapeName: string | null;
    desShapeName: string | null;

    constructor(name: string) {
        this.name = name;
        this.voltage = 0;
        this.dispachingCode = "dispachingCode";
        this.igmcCode = "igmcCode";
        this.toolId = 0;
        this.transferCapacity = 0;
        this.lineFeederType = LineType.Unknown;
        this.srcShapeName = null;
        this.desShapeName = null;
    }
}


export class TransmisionLine extends LineConnectShape<PolyLine> {
    protected properties: ITransmisionLineProperties;
    public connectType: number;

    constructor(name: string, x: number, y: number) {
        super(name);
        this.$type = "TransmisionLine";

        this.shape = new PolyLine();
        this.shape.addPoint(new Point(x, y));
        this.shape.addPoint(new Point(x + 1, y + 1));

        this.properties = new TransmisionLineProperties(name);

        this.style.opacity = 1;
        this.style.strokeWidth = 1;
        this.style.strokeStyle = "#000000";
        this.style.fillStyle = "#00000000";

        this.connectType = 0;
    }

    public get location(): IPoint {
        return new Point(this.shape.points[0].x, this.shape.points[0].y);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        var i;
        ctx.globalAlpha = this.style.opacity;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.strokeStyle = this.style.strokeStyle;

        ctx.beginPath();
        ctx.moveTo(this.shape.points[0].x, this.shape.points[0].y);
        for (i = 1; i < this.shape.points.length; i++) {
            ctx.lineTo(this.shape.points[i].x, this.shape.points[i].y);
        }
        ctx.stroke();
        this._drawHandle(ctx);
    }
    public move(to: IPoint) {
    }
    public inResizeZone(point: IPoint): number {
        let i;
        let ret: number = -1;
        for (i = 1; i < this.shape.points.length - 1; i++) {
            if (this.shape.points[i].distance(point) < this.selectionZoneWidth)
                ret = i;
        }
        return ret;
    }
    public resizeToLocation(to: IPoint, handle: number) {
        var cursor = window.document.body.style.cursor;
        const index = this.shape.points.length - 1;
        if (cursor === "pointer" && handle === -1) {
            this.shape.points[index].x = to.x;
            this.shape.points[index].y = to.y;
        }
        else {
            this.shape.points[handle].x = to.x;
            this.shape.points[handle].y = to.y;
        }
    }
    public contains(mousePoint: IPoint): boolean {
        return this.shape.contains(mousePoint);
    }
    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.points[0].x, mousePosition.y - this.shape.points[0].y);
    }
    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
        if (this.inResizeZone(mousePoint) !== -1)
            return "se-resize";
        else
            return "auto";
    }

    public toSVG(): string {
        var i;
        var markup: Array<string> = [];

        markup.push('\t<g ', this.getSvgId(), '>\n');

        for (i = 0; i < this.shape.points.length - 1; i++) {
            markup.push(
                '\t\t<line ',
                'x1="', this.shape.points[i].x + '"',
                ' y1="', this.shape.points[i].y + '"',
                ' x2="', this.shape.points[i + 1].x + '"',
                ' y2="', this.shape.points[i + 1].y + '"',
                ' style="', SVGUtil.getSvgStyles(this.style),
                '"/>\n'
            );
        }

        markup.push('\t</g>\n');
        return markup.join('');
    }

    public get Schema(): ISchema {
        let lineSchema: ISchema;

        if (this.connectType === 1) {
            lineSchema = {
                title: "ویژگی خط انتقال",
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
                                "title": "ولتاژ (KV)",
                                "pattern": "[-+]?[0-9]*\.?[0-9]*",
                                "disabled": true,
                            },
                            "transferCapacity": {
                                "type": "number",
                                "title": "ظرفیت انتقال (MW)",
                                "pattern": "[-+]?[0-9]*\.?[0-9]*"
                            }
                        }

                    }
                ]
            };
        }
        else {
            lineSchema = {
                title: "ویژگی خط انتقال",
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
                                "pattern": "[-+]?[0-9]*\.?[0-9]*",
                                "disabled": true,
                            },
                            "transferCapacity": {
                                "type": "number",
                                "title": "ظرفیت انتقال (MW)",
                                "pattern": "[-+]?[0-9]*\.?[0-9]*",
                                "disabled": true,
                            }
                        }

                    }
                ]
            };
        }

        return lineSchema;
    }

    public get Properties(): ITransmisionLineProperties {
        return this.properties;
    }

    public set Properties(prop: ITransmisionLineProperties) {
        this.properties = prop;
        this.voltage = prop.voltage;
        this.name = prop.name;
    }

    protected _drawHandle(ctx: CanvasRenderingContext2D) {
        if (this.isSelected) {
            var i;
            var d = this.selectionZoneWidth;
            ctx.globalAlpha = this.style.opacity;
            ctx.strokeStyle = "#00FFFF";
            ctx.lineWidth = 1;
            for (i = 0; i < this.shape.points.length; i++) {
                ctx.strokeRect(this.shape.points[i].x - d, this.shape.points[i].y - 2, 2 * d, 2 * d);
            }
        }
    }
}
