import { DrawingShape } from './DrawingShape';
import { ILine, IPoint, IPolyLine } from './interfaces';
import { DrawingToolType } from '../Enums';
import { Point } from './Shapes';
import { ILineConnectShape } from './interfaces';

export abstract class LineConnectShape<S extends ILine | IPolyLine> extends DrawingShape<S> implements ILineConnectShape {
    public srcShapeId: string | undefined;
    public desShapeId: string | undefined;
    public srcPortName: string | undefined;
    public desPortName: string | undefined;
    public connectShapeType: string | undefined;

    constructor(name: string) {
        super(name);
        this.$type = "LineConnectShape";
        this.srcShapeId = undefined;
        this.desShapeId = undefined;
        this.srcPortName = undefined;
        this.desPortName = undefined;
        this.connectShapeType = undefined;
    }

    public draw(ctx: CanvasRenderingContext2D):void{
    }

    public move(to: IPoint): void{
    }

    public resizeToLocation(to: IPoint, handle: number): void{
    }
    public getCursorType(mousePoint: IPoint, tool: DrawingToolType): string{
        return "auto";
    }
    public getMoveOffset(mousePos: IPoint): IPoint{
        return new Point(0,0);
    }
    public toSVG(): string{
        return "";
    }
}
