import { IStyle } from "./interfaces";

export class Style implements IStyle {
  public opacity: number;
  public strokeWidth: number;
  public strokeStyle: string;
  public fillStyle: string;

  constructor() {
    this.opacity = 1;
    this.strokeWidth = 1;
    this.strokeStyle = "#000000";
    this.fillStyle = "#00000000";
  }
}
