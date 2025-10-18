import { IPoint } from './interfaces/IShapes';
import { DrawingShape } from './DrawingShape';
import { DrawingToolType } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { Line, Point } from './Shapes';

export class LineShape extends DrawingShape<Line> {
    constructor(name: string, x: number, y: number) {
        super(name);
        this.$type = "LineShape";
        this.shape = new Line(new Point(x, y), new Point(x+1, y+1));
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
        let dx: number = this.shape.p2.x - this.shape.p1.x + to.x;
        let dy: number = this.shape.p2.y - this.shape.p1.y + to.y;

        this.shape.p1 = to;
        this.shape.p2 = new Point(dx, dy);
    }
    public inResizeZone(point: IPoint): number {
        var ret: number = -1;
        if (this.shape.p1.distance(point) < this.selectionZoneWidth)
            ret = 0;
        if (this.shape.p2.distance(point) < this.selectionZoneWidth)
            ret = 1;
        return ret;
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

    protected _drawTexts(ctx: CanvasRenderingContext2D) {
        if (this.texts) {
            for (let text of this.texts) {
                if (text.isVisible)
                    text.draw(ctx);
            }
        }
    }
}
