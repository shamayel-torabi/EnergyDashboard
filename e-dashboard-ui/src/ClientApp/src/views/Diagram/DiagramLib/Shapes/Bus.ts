import { IPoint, } from './interfaces';
import { DrawingToolType } from '../Enums';
import { CanvasEngineAction } from '../Enums';
import { DrawingShape } from './DrawingShape';
import { SVGUtil } from '../Util';
import { Rectangle, Point } from './Shapes';

export abstract class Bus extends DrawingShape<Rectangle> {

    constructor(name: string, x: number, y: number, width: number, height: number) {
        super(name);
        this.$type = "Bus";
        this.shape = new Rectangle(x, y, width, height);
    }

    public get location(): IPoint {
        return new Point(this.shape.x, this.shape.y);
    }


    public draw(ctx: CanvasRenderingContext2D) {
        this._drawPorts(ctx);
        this._drawHandle(ctx);
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
    public resizeToLocation(to: IPoint, handle: number): void {
    }

    public contains(mousePoint: IPoint): boolean {
        return this.shape.contains(mousePoint);
    }
    public getMoveOffset(mousePosition: IPoint): IPoint {
        return new Point(mousePosition.x - this.shape.x, mousePosition.y - this.shape.y);
    }
    public inPortZone(mouse: IPoint): boolean {
        return false;
    }
    public getClickLocationAction(mousePoint: Point, tool: DrawingToolType): CanvasEngineAction {
        if (this.contains(mousePoint) && tool === DrawingToolType.Connect)
            return CanvasEngineAction.Connect;
        else if (this.contains(mousePoint) && tool === DrawingToolType.ConnectLine)
            return CanvasEngineAction.ConnectLine;
        else if (this.inResizeZone(mousePoint) !== -1)
            return CanvasEngineAction.Resize;
        else if (this.contains(mousePoint))
            return CanvasEngineAction.Move;
        else
            return CanvasEngineAction.Pan;
    }

    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string {
        let cursur: string = "auto";
        if (tool === DrawingToolType.Select) {
            if (this.inResizeZone(mousePoint) === 0)
                cursur = "w-resize";
            else if (this.inResizeZone(mousePoint) === 1)
                cursur = "e-resize";
            else if (this.contains(mousePoint))
                cursur = "move";
        }
        else if (tool === DrawingToolType.Connect || tool === DrawingToolType.ConnectLine) {
            if (this.contains(mousePoint))
                cursur = "pointer";
        }
        return cursur;
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
