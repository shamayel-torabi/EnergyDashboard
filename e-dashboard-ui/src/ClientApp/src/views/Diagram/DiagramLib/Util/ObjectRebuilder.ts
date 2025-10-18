import {
    Point, Rectangle, Circle, Line, PolyLine,
    CircleShape, LineShape, PolyLineShape, RectangleShape,
    BusBar,ConnectShape, CT, Breaker, BoxedConnectShape,
    LoadFeeder, LineFeeder,  Generator, TransmisionLine, Transformer,
    PortShape, TextShape,    
    //RubberBandShape
} from '../Shapes';
import {
    DiagramModel,
} from '../DiagramModel';



export class ObjectRebuilder {
    // We need this so we can instantiate objects from class name strings
    static classList: { [key: string]: any } = {
        DiagramModel: DiagramModel,
        Point: Point,
        Rectangle: Rectangle,
        Circle: Circle,
        Line: Line,
        PolyLine: PolyLine,
        ConnectShape: ConnectShape,
        RectangleShape: RectangleShape,
        CircleShape: CircleShape,
        LineShape: LineShape,
        PolyLineShape: PolyLineShape,
        BusBar: BusBar,
        Transformer: Transformer,
        CT: CT,
        Breaker: Breaker,
        LineFeeder: LineFeeder,
        LoadFeeder: LoadFeeder,
        Generator: Generator,
        TransmisionLine: TransmisionLine,
        BoxedConnectShape: BoxedConnectShape,
        //RubberBandShape:RubberBandShape,
        PortShape: PortShape,
        TextShape: TextShape,
    }

    // Checks if passed variable is object.
    // Returns true for arrays as well, as intended
    static isObject(varOrObj: any) {
        return varOrObj !== null && typeof varOrObj === 'object';
    }

    static restoreObject(obj: any) {
        let newObj = obj;

        // At this point we have regular javascript object
        // which we got from JSON.parse. First, check if it
        // has "__className" property which we defined in the
        // constructor of each class
        if (obj.hasOwnProperty("$type")) {
            let objClass = ObjectRebuilder.classList[obj['$type']];
            if (objClass) {
                newObj = new (objClass)();
                newObj = Object.assign(newObj, obj);
            }
            else{
                newObj = Object.assign(newObj, obj);
                let err = `خطا در بازیابی نوع شکل:${obj['$type']}`;
                console.error(err);
            }

            // Instantiate object of the correct class

            // Copy all of current object's properties
            // to the newly instantiated object
        }
        else {
            newObj = Object.assign(newObj, obj);
        }

        // Iterate over all of the properties of the new
        // object, and if some of them are objects (or arrays!) 
        // constructed by JSON.parse, run them through ObjectRebuilder
        for (let prop of Object.keys(newObj)) {
            if (ObjectRebuilder.isObject(newObj[prop])) {
                newObj[prop] = ObjectRebuilder.restoreObject(newObj[prop]);
            }
        }

        return newObj;
    }

    static clone(obj: any):any{
        let newObj = Object.assign({}, obj);

        for (let prop of Object.keys(newObj)) {
            if (ObjectRebuilder.isObject(newObj[prop])) {
                newObj[prop] = ObjectRebuilder.clone(newObj[prop]);
            }
        }

        return newObj;
    }

    static shallowEqual(object1: any, object2: any) {
        const keys1 = Object.keys(object1);
        const keys2 = Object.keys(object2);
    
        if (keys1.length !== keys2.length) {
            return false;
        }
    
        for (let key of keys1) {
            if (object1[key] !== object2[key]) {
                return false;
            }
        }
    
        return true;
    }

    static deepEqual(object1: any, object2: any) {
        const keys1 = Object.keys(object1);
        const keys2 = Object.keys(object2);
    
        if (keys1.length !== keys2.length) {
            return false;
        }
    
        for (const key of keys1) {
            const val1 = object1[key];
            const val2 = object2[key];
            const areObjects = ObjectRebuilder.isObject(val1) && ObjectRebuilder.isObject(val2);
            if (
                areObjects && !ObjectRebuilder.deepEqual(val1, val2) ||
                !areObjects && val1 !== val2
            ) {
                return false;
            }
        }
    
        return true;
    }
}
