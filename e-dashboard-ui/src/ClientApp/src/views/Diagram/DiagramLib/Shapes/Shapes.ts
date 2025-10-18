import {IPoint, IShape, IRectangle, ICircle, ILine, IPolyLine} from './interfaces'

export class Point implements IPoint {
    public $type: string;
    public x: number;
    public y: number;

    constructor(x: number, y: number) {
        this.$type = "Point";
        this.x = x;
        this.y = y;
    }

    public distance(point: IPoint): number {
        let x: number = point.x - this.x;
        let y: number = point.y - this.y;
        let s: number = Math.sqrt(x * x + y * y);
        return s;
    }
}

export class Shape implements IShape {
    public $type: string;
    public angle: number = 0;

    constructor() {
        this.$type = "Shape";
        this.angle = 0;
    }
    public contains(mousePoint: IPoint): boolean {
        return false;
    }
    public intersect(rect: IRectangle): boolean {
        return false;
    }

    public equal(other: IShape): boolean {
        return false;
    }
}

export class Rectangle extends Shape implements IRectangle {
    public $type: string;
    public angle: number;
    public selectionZoneWidth: number;

    constructor(public x: number, public y: number, public width: number, public height: number) {
        super();
        this.$type = "Rectangle";
        this.angle = 0;
        this.selectionZoneWidth = 2;
    }
    public resize(height: number, width: number) {
        this.height = height;
        this.width = width;
    }
    public contains(mousePoint: IPoint): boolean {
        let x, y, h, w;
        let xCenter = this.x + this.width / 2;
        let yCenter = this.y + this.height / 2;

        if (this.height < 0) {
            this.y = this.y + this.height;
            this.height = this.height * -1;
        }
        if (this.width < 0) {
            this.x = this.x + this.width;
            this.width = this.width * -1;
        }

        switch (this.angle) {
            case 0:
            case 2:
                x = this.x;
                y = this.y;
                h = this.height;
                w = this.width;
                break;
            case 1:
            case 3:
                x = xCenter - this.height / 2;
                y = yCenter - this.width / 2;
                h = this.width;
                w = this.height;
                break;
            default:
                x = this.x;
                y = this.y;
                h = this.height;
                w = this.width;
                break;
        }

        return (x <= mousePoint.x) &&
            (x + w >= mousePoint.x) &&
            (y <= mousePoint.y) &&
            (y + h >= mousePoint.y);
    }

    public intersect(rect: IRectangle): boolean {
        if (rect.width < 0) {
            rect.width = - rect.width;
            rect.x = rect.x - rect.width;
        }

        if (rect.height < 0) {
            rect.height = - rect.height;
            rect.y = rect.y - rect.height;
        }

        if (rect.x < this.x + this.width && this.x < rect.x + rect.width && rect.y < this.y + this.height)
            return this.y < rect.y + rect.height;
        else
            return false;
    }

    public equal(other: IRectangle): boolean {
        return false;
    }

    public edjePointNearestMouse(mousePoint: IPoint): IPoint | null {
        let point: IPoint | null = null;
        let minDistance: number = Infinity;
        let distTop: number;

        let p1: IPoint = new Point(this.x, this.y);
        let p2: IPoint = new Point(this.x + this.width, this.y);
        distTop = this._edjePointDistance(mousePoint, p1, p2);
        if (distTop < minDistance) {
            point = this._pointNearestMouse(mousePoint, p1, p2);
            minDistance = distTop;
        }

        p1 = new Point(this.x, this.y);
        p2 = new Point(this.x, this.y + this.height);
        distTop = this._edjePointDistance(mousePoint, p1, p2);
        if (distTop < minDistance) {
            point = this._pointNearestMouse(mousePoint, p1, p2);
            minDistance = distTop;
        }

        p1 = new Point(this.x + this.width, this.y);
        p2 = new Point(this.x + this.width, this.y + this.height);
        distTop = this._edjePointDistance(mousePoint, p1, p2);
        if (distTop < minDistance) {
            point = this._pointNearestMouse(mousePoint, p1, p2);
            minDistance = distTop;
        }

        p1 = new Point(this.x, this.y + this.height);
        p2 = new Point(this.x + this.width, this.y + this.height);
        distTop = this._edjePointDistance(mousePoint, p1, p2);
        if (distTop < minDistance) {
            point = this._pointNearestMouse(mousePoint, p1, p2);
            minDistance = distTop;
        }

        return point;
    }

    private _pointNearestMouse(mousePoint: IPoint, p1: IPoint, p2: IPoint): IPoint {
        let dx = p2.x - p1.x;
        let dy = p2.y - p1.y;
        let t = ((mousePoint.x - p1.x) * dx + (mousePoint.y - p1.y) * dy) / (dx * dx + dy * dy);
        let lineX = this._lerp(p1.x, p2.x, t);
        let lineY = this._lerp(p1.y, p2.y, t);
        return new Point(lineX, lineY);
    }

    private _lerp(a: number, b: number, x: number): number {
        return (a + x * (b - a));
    }

    private _edjePointDistance(mousePoint: IPoint, p1: IPoint, p2: IPoint): number {
        let distance: number;

        let linepoint: IPoint = this._pointNearestMouse(mousePoint, p1, p2);
        let dx: number = mousePoint.x - linepoint.x;
        let dy: number = mousePoint.y - linepoint.y;
        distance = Math.abs(Math.sqrt(dx * dx + dy * dy));

        return distance;
    }
}

export class Circle extends Shape implements ICircle {
    public $type: string;
    constructor(public x: number, public y: number, public radius: number) {
        super();
        this.$type = "Circle";
    }
    public resize(radius: number) {
        this.radius = radius;
    }
    public area(): number {
        return Math.PI * this.radius * this.radius;
    }
    public contains(mousePoint: IPoint): boolean {
        let x: number = mousePoint.x - this.x;
        let y: number = mousePoint.y - this.y;

        return Math.sqrt(x * x + y * y) < this.radius;
    }

    public intersect(rect: IRectangle): boolean {
        if (rect.width < 0) {
            rect.width = - rect.width;
            rect.x = rect.x - rect.width;
        }

        if (rect.height < 0) {
            rect.height = - rect.height;
            rect.y = rect.y - rect.height;
        }

        let that = new Rectangle(this.x - this.radius, this.y - this.radius, 2 * this.radius, 2 * this.radius);

        if (rect.x < that.x + that.width && that.x < rect.x + rect.width && rect.y < that.y + that.height)
            return that.y < rect.y + rect.height;
        else
            return false;
    }

    public equal(other: ICircle): boolean {
        return false;
    }
}

export class Line extends Shape implements ILine {
    public $type: string;
    public selectionZoneWidth: number;
    public p1:IPoint;
    public p2:IPoint;

    constructor(p1: IPoint, p2: IPoint) {
        super();
        this.$type = "Line";
        this.selectionZoneWidth = 3;
        this.p1 = p1;
        this.p2 = p2;
    }

    public length(): number {
        var a2 = (this.p2.x - this.p1.x, 2);
        var b2 = (this.p2.y - this.p1.y, 2);
        return Math.sqrt(a2 * a2 + b2 * b2);
    }
    public contains(mousePoint: IPoint): boolean {
        if ((this.p1.x < this.p2.x) && (mousePoint.x < this.p1.x || this.p2.x < mousePoint.x))
            return false;
        else if ((this.p2.x < this.p1.x) && (mousePoint.x < this.p2.x || this.p2.x > mousePoint.x))
            return false;
        else if ((this.p1.y < this.p2.y) && (mousePoint.y < this.p1.y || this.p2.y < mousePoint.y))
            return false;
        else if ((this.p1.y > this.p2.y) && (mousePoint.y < this.p2.y || this.p1.y < mousePoint.y))
            return false;
        else {
            let linepoint = this.linepointNearestMouse(mousePoint);
            let dx = mousePoint.x - linepoint.x;
            let dy = mousePoint.y - linepoint.y;
            let distance = Math.abs(Math.sqrt(dx * dx + dy * dy));
            return distance < this.selectionZoneWidth;
        }
    }

    public intersect(rect: IRectangle): boolean {
        if (rect.width < 0) {
            rect.width = - rect.width;
            rect.x = rect.x - rect.width;
        }

        if (rect.height < 0) {
            rect.height = - rect.height;
            rect.y = rect.y - rect.height;
        }

        let that = new Rectangle(this.p1.x, this.p1.y, this.p1.x - this.p2.x, this.p1.y - this.p2.y);

        if (rect.x < that.x + that.width && that.x < rect.x + rect.width && rect.y < that.y + that.height)
            return that.y < rect.y + rect.height;
        else
            return false;
    }

    public equal(other: ILine): boolean {
        return false;
    }
    public getHandle(mouse: IPoint): IPoint | null {
        var r: IPoint | null = null;
        if (this.p1.distance(mouse) < 4)
            r = this.p1;
        if (this.p2.distance(mouse) < 4)
            r = this.p2;

        return r;
    }

    public linepointNearestMouse(mousePoint: IPoint): IPoint {
        let dx = this.p2.x - this.p1.x;
        let dy = this.p2.y - this.p1.y;
        let t = ((mousePoint.x - this.p1.x) * dx + (mousePoint.y - this.p1.y) * dy) / (dx * dx + dy * dy);
        let lineX = this._lerp(this.p1.x, this.p2.x, t);
        let lineY = this._lerp(this.p1.y, this.p2.y, t);
        return new Point(lineX, lineY);
    }
    private _lerp(a: number, b: number, x: number): number {
        return (a + x * (b - a));
    }
}

export class PolyLine extends Shape implements IPolyLine {
    public $type: string;
    public selectionZoneWidth: number;
    public points: Array<IPoint>;

    constructor() {
        super();
        this.$type = "PolyLine";
        this.selectionZoneWidth = 4;
        this.points = [];
    }

    public length(): number {
        return 0;
    }
    public addPoint(point: IPoint) {
        this.points.push(point);
    }
    public removePoint(point: IPoint) {
        const index: number = this.points.indexOf(point);
        if (index !== -1) {
            this.points.splice(index, 1);
        }
    }
    public contains(mousePoint: IPoint): boolean {
        let i;
        for (i = 0; i < this.points.length - 1; i++) {
            let p1 = this.points[i];
            let p2 = this.points[i + 1];

            if (this._lineContains(p1, p2, mousePoint))
                return true;
        }
        return false;
    }
    public intersect(rect: IRectangle): boolean {
        if (rect.width < 0) {
            rect.width = - rect.width;
            rect.x = rect.x - rect.width;
        }

        if (rect.height < 0) {
            rect.height = - rect.height;
            rect.y = rect.y - rect.height;
        }

        let min_x = this.points[0].x, max_x = this.points[0].x;
        let min_y = this.points[0].y, max_y = this.points[0].y;

        for (let pt of this.points) {
            min_x = Math.min(min_x, pt.x);
            max_x = Math.max(max_x, pt.x);
            min_y = Math.min(min_y, pt.y);
            max_y = Math.max(max_y, pt.y);
        }

        let that = new Rectangle(min_x, min_y, max_x - min_x, max_y - min_y);

        if (rect.x < that.x + that.width && that.x < rect.x + rect.width && rect.y < that.y + that.height)
            return that.y < rect.y + rect.height;
        else
            return false;
    }

    public equal(other: IPolyLine): boolean {
        return false;
    }
    public getHandle(mouse: IPoint): IPoint | null {
        var r: IPoint | null = null;

        return r;
    }

    private _lineContains(p1: IPoint, p2: IPoint, mousePoint: IPoint) {
        if ((p1.x < p2.x) && (mousePoint.x < p1.x || p2.x < mousePoint.x))
            return false;
        else if ((p2.x < p1.x) && (mousePoint.x < p2.x || p2.x > mousePoint.x))
            return false;
        else if ((p1.y < p2.y) && (mousePoint.y < p1.y || p2.y < mousePoint.y))
            return false;
        else if ((p1.y > p2.y) && (mousePoint.y < p2.y || p1.y < mousePoint.y))
            return false;
        else {
            let linepoint = this._linepointNearestMouse(p1, p2, mousePoint);
            let dx = mousePoint.x - linepoint.x;
            let dy = mousePoint.y - linepoint.y;
            let distance = Math.abs(Math.sqrt(dx * dx + dy * dy));
            return distance < this.selectionZoneWidth;
        }
    }

    private _linepointNearestMouse(p1: IPoint, p2: IPoint, mousePoint: IPoint): IPoint {
        let dx = p2.x - p1.x;
        let dy = p2.y - p1.y;
        let t = ((mousePoint.x - p1.x) * dx + (mousePoint.y - p1.y) * dy) / (dx * dx + dy * dy);
        let lineX = this._lerp(p1.x, p2.x, t);
        let lineY = this._lerp(p1.y, p2.y, t);
        return new Point(lineX, lineY);
    }
    private _lerp(a: number, b: number, x: number): number {
        return (a + x * (b - a));
    }
}