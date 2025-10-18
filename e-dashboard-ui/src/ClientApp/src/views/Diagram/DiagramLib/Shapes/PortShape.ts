import { IPoint } from './interfaces';
import { Style } from './Style';
import { IStyle, IPortShape } from './interfaces';
import { Point } from './Shapes';
import { v4 as uuidv4 } from 'uuid';

export class PortShape implements IPortShape {
    public $type: string;
    public id: string;
    public name: string;
    public offset: IPoint;
    public location: IPoint;
    public isConnected: boolean;
    public handle: number ;
    public selectionZoneWidth: number ;
    public connectShapeId: string | null ;
    public style: IStyle ;

    constructor(name: string, offset: IPoint) {
        this.$type = "PortShape";
        this.id = uuidv4();
        this.name = name;
        this.offset = offset;
        this.location = new Point(0,0);
        this.isConnected = false;
        this.handle = 0;
        this.selectionZoneWidth = 3;
        this.connectShapeId = null;
        this.style = new Style();
        
        this.style.opacity = 1;
        this.style.fillStyle = "#FFFFFF00";
        this.style.strokeStyle = "#FF0000";
        this.style.strokeWidth = 1;
    }

    public draw(ctx: CanvasRenderingContext2D) {
        if (!this.isConnected) {
            var d = this.selectionZoneWidth;
            ctx.globalAlpha = this.style.opacity;
            ctx.lineWidth = this.style.strokeWidth;
            ctx.strokeStyle = this.style.strokeStyle;
            ctx.fillStyle = this.style.fillStyle;

            ctx.strokeRect(this.location.x + this.offset.x - d, this.location.y + this.offset.y - d, 2 * d, 2 * d);
        }
    }
    public move(to: IPoint) {
        this.location = to;
    }

    public getCursorType(mousePoint: IPoint): string {
        return "pointer";
    }
    public inPortZone(point: IPoint): boolean {
        let x = this.location.x + this.offset.x;
        let y = this.location.y + this.offset.y;
        let loc = new Point(x,y)
        return loc.distance(point) < this.selectionZoneWidth;
    }

    getLocation(): IPoint {
        let x = this.location.x + this.offset.x;
        let y = this.location.y + this.offset.y;
        return new Point(x, y)
    }
}
