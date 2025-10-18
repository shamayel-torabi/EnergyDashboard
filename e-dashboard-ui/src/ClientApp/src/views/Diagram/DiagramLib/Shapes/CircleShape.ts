import { IPoint } from './interfaces/IShapes';
import { DrawingShape } from './DrawingShape';
import { DrawingToolType, CanvasEngineAction } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { Circle, Point } from './Shapes';

export class CircleShape extends DrawingShape<Circle> {

    constructor(name: string, x: number, y: number, radius: number) {
        super(name);
        this.$type = "CircleShape";
        this.shape = new Circle(x, y, radius);
    }

    public get location(): IPoint {
        return new Point(this.shape.x, this.shape.y);
    }

    public draw(ctx: CanvasRenderingContext2D) {
        ctx.lineWidth = this.style.strokeWidth;
        ctx.globalAlpha = this.style.opacity;
        ctx.strokeStyle = this.style.strokeStyle;

        ctx.beginPath();
        ctx.arc(this.shape.x, this.shape.y, this.shape.radius, 0, 2 * Math.PI);
        ctx.stroke();

        this._drawHandle(ctx);
    }
    public move(to: IPoint) {
        this.shape.x = to.x;
        this.shape.y = to.y;
        if (this.ports) {
            for (let port of this.ports)
                port.move(to);
        }
    }
    public resizeToLocation(to: IPoint, handle: number): void {
    }
    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.x, mousePosition.y - this.shape.y);
    }
    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
        if (this.inResizeZone(mousePoint) !== -1)
            return "se-resize";
        else
            return "move";
    }
    public getClickLocationAction(mousePoint: Point, tool: DrawingToolType): CanvasEngineAction {
        if (this.contains(mousePoint))
            return CanvasEngineAction.Move;
        else
            return CanvasEngineAction.Pan;

        //Changed
        //return CanvasEngineAction.None;
        //return CanvasEngineAction.Pan;
    }

    public toSVG(): string {
        var markup: Array<string> = [];

        markup.push(
            '\t<circle ', this.getSvgId(),
            'cx="' + this.shape.x + '" cy="' + this.shape.y + '" ',
            'r="', this.shape.radius + '"',
            ' style="', SVGUtil.getSvgStyles(this.style),
            '"/>\n');
        return markup.join('');
    }

    private _drawHandle(ctx: CanvasRenderingContext2D) {
        var d = this.selectionZoneWidth;
        if (this.isSelected) {
            ctx.globalAlpha = this.style.opacity;
            ctx.strokeStyle = "#00FFFF";
            ctx.lineWidth = 1;
            let p: IPoint = new Point(this.shape.x, this.shape.y);
            ctx.strokeRect(p.x - d, p.y - d, 2 * d, 2 * d);
        }
    }
}
