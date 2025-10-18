import { IPoint } from './interfaces/IShapes';
import { DrawingShape } from './DrawingShape';
import { Style } from './Style';
import { DrawingToolType } from '../Enums';
import { SVGUtil } from '../Util';
import { Rectangle, Point } from './Shapes';

export class RubberBandShape extends DrawingShape<Rectangle> {
    constructor(name: string, x: number, y: number, width: number, height: number) {
        super(name);
        this.$type = "RubberBandShape";
        this.shape = new Rectangle(x, y, width, height);
        this.style = new Style();
    }

    public get location(): IPoint {
        return new Point(this.shape.x, this.shape.y);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;
        ctx.fillStyle = this.style.fillStyle;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.setLineDash([4,4]);
        ctx.strokeRect(this.shape.x, this.shape.y, this.shape.width, this.shape.height);
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

    public resizeToLocation(to: IPoint, handle: number) {
        this.shape.width = to.x - this.shape.x;
        this.shape.height = to.y - this.shape.y
    }
    public contains(mousePoint: IPoint): boolean {
        return this.shape.contains(mousePoint);
    }
    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.x, mousePosition.y - this.shape.y);
    }
    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
            return "auto";
    }
}
