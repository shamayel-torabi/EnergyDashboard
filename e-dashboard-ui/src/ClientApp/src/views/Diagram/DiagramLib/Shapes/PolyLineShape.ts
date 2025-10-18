import { IPoint } from './interfaces/IShapes';
import { DrawingShape } from './DrawingShape';
import { DrawingToolType } from '../Enums';
import { SVGUtil } from '../Util/SVGUtil';
import { PolyLine, Point } from './Shapes';

export class PolyLineShape extends DrawingShape<PolyLine> {

  constructor(name: string, x: number, y: number) {
    super(name);
    this.$type = "PolyLineShape";
    this.shape = new PolyLine();

    this.shape.addPoint(new Point(x, y));
    this.shape.addPoint(new Point(x + 1, y + 1));

    this.style.opacity = 1;
    this.style.strokeWidth = 1;
    this.style.strokeStyle = "#000000";
    this.style.fillStyle = "#00000000";
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
    let i;
    let dx: number = to.x - this.shape.points[0].x;
    let dy: number = to.y - this.shape.points[0].y;

    for (i = 0; i < this.shape.points.length; i++) {
      this.shape.points[i] = new Point(this.shape.points[i].x + dx, this.shape.points[i].y + dy);
    }
  }
  public inResizeZone(point: IPoint): number {
    let i;
    let ret: number = -1;
    for (i = 0; i < this.shape.points.length; i++) {
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
      return "move";
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
