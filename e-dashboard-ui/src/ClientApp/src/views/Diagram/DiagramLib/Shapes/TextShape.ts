import { IPoint } from './interfaces';
import { TextStyle } from './TextStyle';
import { SVGUtil } from '../Util';
import { ITextShape, ITextStyle } from './interfaces';
import { Point } from './Shapes';

export class TextShape implements ITextShape {
    public $type: string ;
    public id:string;
    public text: string;
    public offset: IPoint;
    public location: IPoint;
    public isVisible: boolean;
    public style: ITextStyle;
    public angle: number = 0;

    constructor(id: string, text: string, offset: IPoint, angle: number = 0) {
        this.$type = "TextShape";
        this.id = id;
        this.text = text;
        this.offset = offset;
        this.location = new Point(0, 0);
        this.angle = angle;
        this.isVisible = true;
        this.style = new TextStyle();
    }

    public draw(ctx: CanvasRenderingContext2D) {
        let font: string = this.style.fontStyle + ' ' + this.style.fontSize + 'pt ' + this.style.fontFamily;
        ctx.save();
        ctx.globalAlpha = this.style.opacity;
        ctx.lineWidth = this.style.strokeWidth;
        ctx.fillStyle = this.style.fillStyle;
        ctx.font = font;
        ctx.textAlign = this.style.textAlign;
        ctx.textBaseline = this.style.textBaseline;
        ctx.translate(this.location.x + this.offset.x, this.location.y + this.offset.y);
        ctx.rotate(this.angle * Math.PI / 180);
        ctx.fillText(this.text, 0, 0);
        ctx.restore();
    }

    public move(to: IPoint) {
        this.location = to;
    }

    public rotate(angle: number): void {
        this.angle = angle;
    }

    public inTextZone(mouse: IPoint): boolean {
        var ret: boolean = false;
        return ret;
    }

    public toSVG(): string {
        let x = this.location.x + this.offset.x;
        let y = this.location.y + this.offset.y;

        var markup: Array<string> = [];
        markup.push(
            '\t\t<text ', this.getSvgId(),
            'x="' + x + '" ',
            'y="' + y + '" ',
            'font-family="' + this.style.fontFamily.replace(/"/g, '\'') + '" ',
            'font-size="' + this.style.fontSize + '" ',
            'font-style="' + this.style.fontStyle + '" ',
            'alignment-baseline="' + this.style.textBaseline + '" ',
            'text-anchor="' + this.style.textAnchor + '" ',
            'style="', SVGUtil.getSvgStyles(this.style), '"', ' >\n',
            '\t\t\t', this.text, '\n',
            '\t\t</text>\n'
        );

        return markup.join('');
    }

    public wrapText(ctx: CanvasRenderingContext2D, text: string, x: number, y: number, maxWidth: number, lineHeight: number) {
        var words = text.split(' ');
        var line = '';

        for (var n = 0; n < words.length; n++) {
            var testLine = line + words[n] + ' ';
            var metrics = ctx.measureText(testLine);
            var testWidth = metrics.width;
            if (testWidth > maxWidth && n > 0) {
                ctx.fillText(line, x, y);
                line = words[n] + ' ';
                y += lineHeight;
            }
            else {
                line = testLine;
            }
        }
        ctx.fillText(line, x, y);
    }

    protected getSvgId(): string {
        var ret = this.id ? 'id="' + this.id + '" ' : ' ';
        return ret;
    }
}
