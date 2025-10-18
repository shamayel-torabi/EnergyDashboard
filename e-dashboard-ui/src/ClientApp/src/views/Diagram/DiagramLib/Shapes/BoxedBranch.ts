import { BoxedShape } from './BoxedShape';

export abstract class BoxedBranch extends BoxedShape {
    public connectShapeType: string | undefined;

    constructor(name: string, x: number, y: number, width: number = 10, height: number = 20) {
        super(name, x, y, width, height);
        this.$type = "BoxedLink";
        this.connectShapeType = undefined;
    }
}
