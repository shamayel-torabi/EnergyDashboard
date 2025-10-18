
export class Memento {
    private _state: string;

    constructor(state: any) {
        this._state = JSON.stringify(state);
    }

    get state() {
        return JSON.parse(this._state);
    }
}