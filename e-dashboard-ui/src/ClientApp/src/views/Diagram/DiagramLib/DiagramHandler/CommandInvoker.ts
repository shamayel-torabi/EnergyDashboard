
import { Memento } from "./Memento";
import { IDiagramEventHandler } from "../DiagramHandler";

export interface ICommandInvoker {
    saveState(state: Memento): void;
    undo(): void;
    redo(): void;
}

export class CommandInvoker implements ICommandInvoker {
    private diagramEventHandler: IDiagramEventHandler
    private history: Memento[];
    private current: number;

    constructor(diagramEventHandler: IDiagramEventHandler) {
        this.diagramEventHandler = diagramEventHandler;
        this.history = [];
        this.current = -1;
    }

    public saveState(state: Memento): void {
        if (this.history.length - 1 > this.current) {
            var next = this.current + 1
            this.history.splice(next, this.history.length - next);
        }
        this.history.push(state);
        this.current = this.history.length - 1;
    }

    public undo() {        
        if (this.current > 0) {
            this.current -= 1;
            let momento = this.history[this.current];

            if (momento) {
                this.diagramEventHandler.state = momento;
            }
        }
    }

    public redo() {
        if (this.current < this.history.length -1) {
            this.current += 1;

            let momento = this.history[this.current];
            if (momento) {
                this.diagramEventHandler.state = momento;
            }
        }
    }
}