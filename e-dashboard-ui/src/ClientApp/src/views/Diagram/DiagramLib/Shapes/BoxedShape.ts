import { IPoint } from './interfaces';
import { DrawingShape } from './DrawingShape';
import { DrawingToolType } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { Point, Rectangle } from './Shapes';

export abstract class BoxedShape extends DrawingShape<Rectangle> {

    constructor(name: string, x: number, y: number, width: number = 10, height: number = 20) {
        super(name);
        this.$type = "BoxedShape";
        this.shape = new Rectangle(x, y, width, height);
    }

    public get location(): IPoint {
        return new Point(this.shape.x, this.shape.y);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        this._drawHandle(ctx);
        this._drawPorts(ctx);
        this._drawTexts(ctx);
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

    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.x, mousePosition.y - this.shape.y);
    }
    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
        if (this.inPortZone(mousePoint) && (tool === DrawingToolType.Connect || tool === DrawingToolType.ConnectLine))
            return "pointer";
        else if (this.contains(mousePoint) && tool === DrawingToolType.Select)
            return "move";
        else
            return "auto";
    }
    public toSVG(): string {
        var markup: Array<string> = [];
        markup.push(
            '\t<rect', this.getSvgId(),
            ' x="', this.shape.x + '"',
            ' y="', this.shape.y + '"',
            ' width="', this.shape.width + '"',
            ' height="', this.shape.height + '"',
            ' style="', SVGUtil.getSvgStyles(this.style) + '"',
            '/>\n');
        return markup.join('');
    }

    public resizeToLocation(to: IPoint, handle: number): void{
    }

    protected _drawHandle(ctx: CanvasRenderingContext2D) {
        var d = this.selectionZoneWidth;
        if (this.isSelected) {
            ctx.globalAlpha = this.style.opacity;
            ctx.strokeStyle = "#00FFFF";
            ctx.lineWidth = 1;
            let p: IPoint = new Point(this.shape.x, this.shape.y);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
            p = new Point(this.shape.x + this.shape.width, this.shape.y);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
            p = new Point(this.shape.x, this.shape.y + this.shape.height);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
            p = new Point(this.shape.x + this.shape.width, this.shape.y + this.shape.height);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
        }
    }
    protected _drawPorts(ctx: CanvasRenderingContext2D) {
        if (this.ports) {
            for (let port of this.ports) {
                if (!port.isConnected)
                    port.draw(ctx);
            }
        }
    }
    protected _drawTexts(ctx: CanvasRenderingContext2D) {
        if (this.texts) {
            for (let text of this.texts) {
                if (text.isVisible)
                    text.draw(ctx);
            }
        }
    }
}
