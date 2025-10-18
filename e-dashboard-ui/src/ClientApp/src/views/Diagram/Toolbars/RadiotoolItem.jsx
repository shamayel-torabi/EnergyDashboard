import React from 'react';
import PropTypes from 'prop-types';
import { DrawingToolType, DrawingShapeType, DiagramTool } from '../DiagramLib/Enums';
import { useDiagram } from '../DiagramContext'

const RadioToolItem = (props) => {
    const { name, value, label, icon, children } = props;
    const diagramContext = useDiagram();

    const changeTool = (tool) => {
        let drawTool, shapeTool;
        switch (tool) {
            case DiagramTool.Select:
                drawTool = DrawingToolType.Select;
                shapeTool = DrawingShapeType.Null;
                break;
            case DiagramTool.Connect:
                drawTool = DrawingToolType.Connect;
                shapeTool = DrawingShapeType.Connect;
                break;
            case DiagramTool.Rectangle:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.RectangleShape;
                break;
            case DiagramTool.RubberBand:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.RubberBandShape;
                break;
            case DiagramTool.Circle:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.CircleShape;
                break;
            case DiagramTool.Line:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.LineShape;
                break;
            case DiagramTool.PolyLine:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.PolyLineShape;
                break;
            case DiagramTool.BusBar:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.BusBar;
                break;
            case DiagramTool.Generator:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.Generator;
                break;
            case DiagramTool.Transformer:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.Transformer;
                break;
            case DiagramTool.CT:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.CT;
                break;
            case DiagramTool.Breaker:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.Breaker;
                break;
            case DiagramTool.LineFeeder:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.LineFeeder;
                break;
            case DiagramTool.LoadFeeder:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.LoadFeeder;
                break;
            case DiagramTool.BoxedConnect:
                drawTool = DrawingToolType.DrawShape;
                shapeTool = DrawingShapeType.BoxedConnectShape;
                break;
            case DiagramTool.TransmisionLine:
                drawTool = DrawingToolType.ConnectLine;
                shapeTool = DrawingShapeType.TransmisionLine;
                break;
            default:
                drawTool = DrawingToolType.Select;
                shapeTool = DrawingShapeType.Null;
                break;
        }

        return {
            diagramTool: tool,
            drawingTool: drawTool,
            drawingShape: shapeTool,
        };
    };

    const handleChange = (event) => {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        let tool = parseInt(value);
        let drawTool = changeTool(tool);
        diagramContext.changeDrawTool(drawTool);
    }

    const drawingTool = diagramContext.drawTool.diagramTool;

    return (
        <div className="tool" title={label}>
            <input
                type="radio"
                id={`${name}-${value}`}
                value={value}
                name={name}
                checked={drawingTool === value}
                onChange={handleChange}  />
            <label className="btn btn-secondary btn-sm" htmlFor={`${name}-${value}`}>
                <span className={icon}>
                    {children}
                </span>
            </label>
        </div>
    );
};

RadioToolItem.propTypes = {
    children: PropTypes.element,
    name: PropTypes.string,
    value: PropTypes.number.isRequired,
    label: PropTypes.string.isRequired,
    icon: PropTypes.string.isRequired,
};

export { RadioToolItem }
