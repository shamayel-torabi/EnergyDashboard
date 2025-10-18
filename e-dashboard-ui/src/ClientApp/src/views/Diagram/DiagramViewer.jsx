import { useRef, useEffect, useCallback, useLayoutEffect } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useNavigate, createSearchParams } from 'react-router-dom';
import { DiagramEventHandler, Point } from './DiagramLib';
import { CRUDService } from '../../services';
import { ContextMenu } from '../../components/ContextMenu';
import { apiUrl, notificationUrl } from '../../ApiConfig';
import { useAuthorize, Roles } from '../../api-authorization'

const DiagramViewer = ({ diagramState, width, height }) => {
    const canvasRef = useRef(null);
    const hubConnection = useRef(null);
    const rAF = useRef(0);
    const diagramEventHandler = useRef(null);
    const isDragging = useRef(false);
    const dragStartPosition = useRef(new Point(0, 0));

    const navigate = useNavigate();
    const authService = useAuthorize();

    const role = authService.user?.profile.role;
    const isAdmin = [Roles.Admins].indexOf(role) === -1 ? false : true;

    const menuItems = [
        { id: "Edit", text: "ویرایش", isAdmin: isAdmin },
        { id: "EnergyReport", text: "گزارش انرژی", isAdmin: true },
        { id: "Properties", text: "مشخصات", isAdmin: true },
    ];

    const { diagramMode, diagramId } = diagramState;

    const loadDiagram = useCallback(async () => {
        if (!diagramId)
            return;

        const url = `${apiUrl}/${diagramMode}s`;
        const service = new CRUDService(url);

        try {
            let diagram = await service.read(diagramId);
            if (diagram) {
                diagramEventHandler.current = new DiagramEventHandler(canvasRef.current, false);
                diagramEventHandler.current.loadDiagram(diagram);
                invalidate();
            }
        } catch (e) {
            console.error(e);
        }
    }, [diagramMode, diagramId]);

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Error)
            .build();

        hubConnection.current.on('refreshMeterInstant', async () => {
            await loadDiagram();
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection'));


        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        };
    }, []);

    useLayoutEffect(() => {
        if (diagramEventHandler.current)
            diagramEventHandler.current.firstRun = true;

        invalidate();
    }, [width, height]);

    useEffect(() => {
        loadDiagram();
    }, [loadDiagram]);

    useEffect(()=>{
        return () => {
            if (rAF.current)
                cancelAnimationFrame(rAF.current);
        }
    },[]);

    const invalidate = () => {
        rAF.current = requestAnimationFrame(() => diagramEventHandler.current?.draw());
    };

    const handleMouseDown = (e) => {
        if (canvasRef.current === null)
            return;

        isDragging.current = true;
        dragStartPosition.current = diagramEventHandler.current?.getMousePosition(e.clientX, e.clientY);;
        invalidate();
    };

    const handleMouseMove = (e) => {
        if (canvasRef.current === null)
            return;
        let current = diagramEventHandler.current?.getMousePosition(e.clientX, e.clientY);
        if (isDragging.current) {
            diagramEventHandler.current?.translate(current.x - dragStartPosition.current.x, current.y - dragStartPosition.current.y);
        }
        invalidate();
    };

    const handleMouseUp = (e) => {
        if (canvasRef.current === null)
            return;

        isDragging.current = false;
    };

    const handleWheel = (e) => {
        if (canvasRef.current === null)
            return;

        let current = diagramEventHandler.current?.getMousePosition(e.clientX, e.clientY);
        const factor = e.deltaY < 0 ? 1.1 : 0.9;
        diagramEventHandler.current?.zoom(factor, current);
        invalidate();
    };

    const handleContexMenuaction = (item) => {
        switch (item.id) {
            case "Edit":
                editDiagram();
                break;
            case "EnergyReport":
                break;
            case "Properties":
                break;
            default:
                break;
        }
    }

    const handleKeyDown = (e) => {
        const diagram = diagramEventHandler.current?.diagram;
        const p = diagramEventHandler.current?.getMousePosition(diagram.Properties.width / 2, diagram.Properties.height / 2);

        switch (e.keyCode) {
            case 33:
                diagramEventHandler.current?.zoom(1.1, p);
                break;
            case 34:
                diagramEventHandler.current?.zoom(0.9, p);
                break;
            default:
                break;
        }
        invalidate();
    }

    const editDiagram = () => {
        const { diagramId, diagramMode, areaId, zoneId } = diagramState;
        const url = `/network/${diagramId}`;
        let search;

        if (diagramMode === "substation") {
            search = createSearchParams({
                diagramMode: diagramMode,
                areaId: areaId,
                zoneId: zoneId
            })
        }
        else {
            search = createSearchParams({
                diagramMode: diagramMode
            })
        }

        navigate({
            pathname: url,
            search: search.toString()
        });
    }

    return (
        <div className="canvas-container">
            <canvas className="canvas"
                ref={canvasRef}
                tabIndex={0}
                width={width}
                height={height}
                onMouseDown={handleMouseDown}
                onMouseMove={handleMouseMove}
                onMouseUp={handleMouseUp}
                onWheel={handleWheel}
                onKeyDown={handleKeyDown}>
            </canvas>
            <ContextMenu target="canvas" menuItems={menuItems} onAction={handleContexMenuaction} />
        </div>
    );
}

export { DiagramViewer };