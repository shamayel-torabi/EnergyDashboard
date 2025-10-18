import { useState, useEffect, useLayoutEffect, useRef, useImperativeHandle, forwardRef, memo } from 'react'
import { PropertyDialog } from './PropertyDialog';
import {
    DrawingToolType,
    DrawingShapeType,
    DiagramTool,
    DiagramModel,
    DiagramEventHandler,
} from './DiagramLib';
import { useDiagram } from './DiagramContext'
import { CRUDService } from '../../services';
import { apiUrl } from '../../ApiConfig';

const DiagramEditor = memo(forwardRef((props, ref) => {
    const { diagramMode, diagramId, zoneId, areaId, width, height } = props;
    const canvasRef = useRef(null);
    const rAF = useRef(0);

    const [propertyModal, showPropertyModal] = useState(false);
    const [selected, setSelected] = useState(undefined);
    const [diagramHandler, setDiagramHandler] = useState(null);

    const digramContext = useDiagram();

    useLayoutEffect(() => {
        invalidate();
        canvasRef.current.focus();
    }, [width, height]);

    useEffect(() => {
        const loadDiagram = async () => {
            const url = `${apiUrl}/${diagramMode}s`;
            const service = new CRUDService(url);
            try {
                let diagram = await service.read(diagramId);
                if (diagram) {
                    let canvas = canvasRef.current;
                    let handler = new DiagramEventHandler(canvas, true);
                    handler.loadDiagram(diagram);
                    let state = handler.state;
                    handler.commandInvoker.saveState(state);
                    setDiagramHandler(handler);
                }
            } catch (e) {
                console.error(e);
            }
        };

        loadDiagram();
    }, [diagramMode, diagramId]);

    useImperativeHandle(ref, () => (
        {
            getDiagramHandler: () => { return diagramHandler },
            invalidate: () => { invalidate() },
        }),
        [diagramHandler]
    );

    useEffect(()=>{
        return () => {
            if (rAF.current)
                cancelAnimationFrame(rAF.current);
        }
    },[]);

    const invalidate = () => {
        rAF.current = requestAnimationFrame(() => diagramHandler?.draw());
    };

    const clearEngineState = () => {
        digramContext.changeDrawTool({
            diagramTool: DiagramTool.Select,
            drawingTool: DrawingToolType.Select,
            drawingShape: DrawingShapeType.Null
        });
    }

    const handleMouseDown = (e) => {
        if (canvasRef.current === null)
            return;

        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }

        const event = {
            clientX: e.clientX,
            clientY: e.clientY,
            pageX: e.pageX,
            pageY: e.pageY,
            shiftKey: e.shiftKey,
            diagramState: diagramState,
        }

        diagramHandler?.handleMouseDown(event);
        invalidate();
    }

    const handleMouseMove = (e) => {
        if (canvasRef.current === null)
            return;

        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }

        const event = {
            clientX: e.clientX,
            clientY: e.clientY,
            pageX: e.pageX,
            pageY: e.pageY,
            shiftKey: e.shiftKey,
            diagramState: diagramState,
        }

        diagramHandler?.handleMouseMove(event);
        digramContext.mousePosition(event.clientX, event.clientY);
        invalidate();
    }

    const handleMouseUp = (e) => {
        if (canvasRef.current === null)
            return;

        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }

        const event = {
            clientX: e.clientX,
            clientY: e.clientY,
            pageX: e.pageX,
            pageY: e.pageY,
            shiftKey: e.shiftKey,
            diagramState: diagramState,
        }

        diagramHandler?.handleMouseUp(event);
        if (!event.inAction) {
            clearEngineState();
            invalidate();
        }
    }

    const handleContextMenu = (e) => {
        e.preventDefault();

        if (canvasRef.current === null)
            return;
        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }

        const event = {
            clientX: e.clientX,
            clientY: e.clientY,
            pageX: e.pageX,
            pageY: e.pageY,
            shiftKey: e.shiftKey,
            diagramState: diagramState,
        }

        diagramHandler?.handleContextMenu(event);

        clearEngineState();
        invalidate();        
    }

    const handleDoubleClick = (e) => {
        e.preventDefault();

        if (canvasRef.current === null)
            return;

        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }

        const event = {
            clientX: e.clientX,
            clientY: e.clientY,
            pageX: e.pageX,
            pageY: e.pageY,
            shiftKey: e.shiftKey,
            diagramState: diagramState,
        }

        diagramHandler?.handleDoubleClick(event);

        if (event.inAction)
            return;

        if (event.diagramState.selected) {
            setSelected(event.diagramState.selected);
            showPropertyModal(true);
        }
        else if (diagramHandler) {
            setSelected(diagramHandler.diagram);
            showPropertyModal(true);
        }
        clearEngineState();
        invalidate();
    }

    const handleKeyDown = (e) => {
        if (canvasRef.current === null)
            return;

        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }

        const event = {
            keyCode: e.keyCode,
            diagramState: diagramState
        }

        diagramHandler?.handleKeyDown(event)

        if (event.diagramState.selected) {
            setSelected(event.diagramState.selected);
            showPropertyModal(true);
        }

        if (e.keyCode === 27)
            clearEngineState();
            
        invalidate();
    }

    const handleWheel = (e) => {
        if (canvasRef.current === null)
            return;
        const diagramState = {
            diagramMode: diagramMode,
            drawingToolType: digramContext.drawTool.drawingTool,
            drawingShapeType: digramContext.drawTool.drawingShape,
            areaId: areaId,
            zoneId: zoneId,
            selected: undefined
        }


        let event = {
            clientX: e.clientX,
            clientY: e.clientY,
            pageX: e.pageX,
            pageY: e.pageY,
            deltaX: e.deltaX,
            deltaY: e.deltaY,
            shiftKey: e.shiftKey,
            diagramState: diagramState
        }

        diagramHandler?.handleWheel(event);
        invalidate();
    }

    const onSaveProperty = (m) => {
        if (m && selected) {
            selected.Properties = m;
            if (selected instanceof DiagramModel) {
                const w = m.width;
                const h = m.height;
                diagramHandler?.setDiagramSize(w, h);
            }
            invalidate();
        }

        setSelected(undefined);
        showPropertyModal(false);
    }

    return (
        <div className="canvas-container">
            <canvas className='canvas'
                ref={canvasRef}
                tabIndex={0}
                width={width}
                height={height}
                onMouseDown={handleMouseDown}
                onMouseMove={handleMouseMove}
                onMouseUp={handleMouseUp}
                onContextMenu={handleContextMenu}
                onDoubleClick={handleDoubleClick}
                onKeyDown={handleKeyDown}
                onWheel={handleWheel}>
            </canvas>
            {propertyModal && <PropertyDialog
                isOpen={propertyModal}
                property={selected?.Properties}
                schema={selected?.Schema}
                substationId={diagramHandler?.diagram.Properties.igmcStationId}
                onSave={onSaveProperty} />}
        </div>
    );
}));

export { DiagramEditor }

