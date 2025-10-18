import { DiagramState } from '../Enums';
import { IDrawingShape, IShape } from '../Shapes';

export interface MouseEventArgs {
    clientX:number;
    clientY:number;
    pageX:number;
    pageY:number;    
    shiftKey: boolean;
    diagramState: DiagramState
    inAction?: boolean;
    selected?: IDrawingShape<IShape>;
}

export interface KeyboardEventArgs {
    keyCode: number;
    diagramState: DiagramState
    inAction?: boolean;
    selected?: IDrawingShape<IShape>;
}

export interface MouseWheelEventArgs {
    clientX:number;
    clientY:number;
    pageX:number;
    pageY:number;    
    deltaX: number;
    deltaY: number;
    shiftKey:boolean;
    diagramState: DiagramState
    inAction?: boolean;
}
