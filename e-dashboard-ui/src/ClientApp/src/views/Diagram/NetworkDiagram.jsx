import { useState, useRef, useCallback, memo } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { saveAs, encodeBase64 } from '@progress/kendo-file-saver';
import { DiagramEditor } from './DiagramEditor';
import { DiagramTool, DrawingToolType, DrawingShapeType } from './DiagramLib';
import { ToolbarPane, RadioTool, RadioToolItem, ButtonTool, Divider } from './Toolbars';
import { DiagramContext } from './DiagramContext';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { useToast } from '../../components';
import { apiUrl } from '../../ApiConfig';
import { useSizeDetector } from '../../components';

const MouseCoordinate = memo(({ x, y }) => {
    return (
        <table style={{ width: '100%' }}>
            <tbody>
                <tr>
                    <td className="text-left" style={{ width: '80%' }}>{Math.floor(x)}</td>
                    <td className="text-right" style={{ width: '20%' }}>:X</td>
                </tr>
                <tr>
                    <td className="text-left" style={{ width: '80%' }}>{Math.floor(y)}</td>
                    <td className="text-right" style={{ width: '20%' }}>:Y</td>
                </tr>
            </tbody>
        </table>
    )
});

const NetworkDiagram = (props) => {
    const diagramEditorRef = useRef(null);
    const targetRef = useRef();

    const { width, height } = useSizeDetector({ targetRef });
    const [mouse, setMouse] = useState({ mouseX: 0, mouseY: 0 });
    const [drawTool, setDrawTool] = useState({
        diagramTool: DiagramTool.Select,
        drawingTool: DrawingToolType.Select,
        drawingShape: DrawingShapeType.Null
    });


    const toast = useToast();
    const authService = useAuthorize();
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const params = useParams();

    const diagramId = params.diagramId;
    const diagramMode = searchParams.get('diagramMode').toLowerCase();
    const zoneId = searchParams.get('zoneId') ?? '';
    const areaId = searchParams.get('areaId') ?? '';

    const onDiagramUpdate = async () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();

        if (diagramHandler === null && diagramMode === null)
            return;

        const diagram = diagramHandler.diagram.toObject();
        const url = `${apiUrl}/${diagramMode}s/${diagram.id}`;
        const accessToken = authService.accessToken;

        try {
            const req = new Request(url, {
                method: 'PUT',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${accessToken}`
                }),
                body: JSON.stringify(diagram)
            })

            const response = await fetch(req);

            if (response.ok) {
                toast.showToast("success", "دیاگرام با موفقیت به روز شد.");
                navigate('/network');
            }
            else if (response.status === 401) {
                const problem = await response.json();
                const errStr = `برای به روز رسانی به سایت وارد شوید`;
                toast.showToast("error", errStr);
                console.error(problem);
            }
            else {
                const problem = await response.json();
                toast.showToast("error", problem.title);
                console.error(problem);
            }

        }
        catch (e) {
            toast.showToast('error', 'خطا اتصال به سرور !');
            console.error('خطا اتصال به سرور !', e);
        }
    }

    const onSaveSvg = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();
        const diagram = diagramHandler.diagram;

        let svg = diagram.toSVG();
        let title = diagram.Properties.title;
        let width = diagram.Properties.width;
        let height = diagram.Properties.height;


        width = Math.floor(width * window.devicePixelRatio / 96);
        height = Math.floor(height * window.devicePixelRatio / 96);

        let win = window.open();
        if (win) {

            let style = `
                <style>
                    html{
                        direction:rtl;
                        display:flex;
                        justify-content: center;    
                        align-items:center;
                    }
                    @media print {
                        @page {
                            size: ${width}in ${height}in;
                            margin: 0.5in;
                        }
                    }
                </style>`;

            win.document.title = title;
            win.document.write(style);
            win.document.write(svg);
        }
    }

    const onSaveLocal = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();

        const json = diagramHandler.diagram.toJSON();
        const title = diagramHandler.diagram.Properties.title;
        const dataURI = "data:application/json;base64," + encodeBase64(json);
        const fileName = `${title}.json`;
        saveAs(dataURI, fileName);
    }

    const onLoadLocal = (event) => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();

        let input = event.target;

        let reader = new FileReader();
        reader.addEventListener('load', (event) => {
            const result = event.target?.result;
            const diagram = JSON.parse(result);
            diagramHandler.diagram.removeAll();
            diagramHandler.diagram.loadJson(diagram);
            diagramEditor.invalidate();
        });

        if (input && input.files) {
            reader.readAsText(input.files[0]);
        }
    }

    const onClear = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();

        diagramHandler.diagram.removeAll();
        diagramEditor.invalidate();
    }

    const onRedo = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();

        diagramHandler.redo();
        diagramEditor.invalidate();
    }

    const onUndo = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();

        diagramHandler.undo();
        diagramEditor.invalidate();
    }

    const renderToolBar = () => {
        let label = '';

        switch (diagramMode) {
            case "network":
                label = 'برق منظقه ای';
                break;
            case "area":
                label = 'ناحیه';
                break;
            case "zone":
                label = 'ایستگاه';
                break;
            default:
                label = 'برق منظقه ای';
                break;
        }

        if (diagramMode === "substation") {
            return (
                <div className="toolbar-container">
                    <ToolbarPane>
                        <RadioTool name='powerTools'>
                            <RadioToolItem value={DiagramTool.Select} label='انتخاب' icon="sh-select" />
                            <RadioToolItem value={DiagramTool.Connect} label='اتصال' icon="sh-connect" />
                            <RadioToolItem value={DiagramTool.BusBar} label='باس بار' icon="sh-busbar" />
                            <RadioToolItem value={DiagramTool.Generator} label='زنراتور' icon="sh-generator" />
                            <RadioToolItem value={DiagramTool.Transformer} label='ترانسفورمر' icon="sh-transformer" />
                            <RadioToolItem value={DiagramTool.CT} label='ترانس جریان' icon="sh-ct" />
                            <RadioToolItem value={DiagramTool.Breaker} label='بریکر' icon="sh-breaker" />
                            <RadioToolItem value={DiagramTool.LoadFeeder} label='فیدر بار' icon="sh-loadfeeder" />
                        </RadioTool>
                    </ToolbarPane>
                    <Divider />
                    <ToolbarPane>
                        <ButtonTool label="بزرگنمایی" onClick={onZoomIn} icon="sh-zoomin"></ButtonTool>
                        <ButtonTool label="کوچکنمایی" onClick={onZoomOut} icon="sh-zoomout"></ButtonTool>
                        <ButtonTool label="انجام" onClick={onRedo} icon="sh-rotateright"></ButtonTool>
                        <ButtonTool label="پاد انجام" onClick={onUndo} icon="sh-rotateleft"></ButtonTool>
                        <ButtonTool label="ذخیره محلی" icon="sh-export" onClick={onSaveLocal} />
                        <ButtonTool label="بازیابی محلی" icon="sh-import">
                            <input type='file' accept='application/json' style={{ display: 'none' }} onChange={onLoadLocal} />
                        </ButtonTool>
                        <ButtonTool label="پاک کردن" icon="sh-clear" onClick={onClear} />
                        <ButtonTool label="به روز رسانی" icon="sh-save" onClick={onDiagramUpdate} />
                        <ButtonTool label="نمایش SVG" icon="sh-svg" onClick={onSaveSvg} />
                    </ToolbarPane>
                    <Divider />
                    <MouseCoordinate x={mouse.mouseX} y={mouse.mouseY} />
                </div>
            )
        }
        else {

            return (
                <div className="toolbar-container">
                    <ToolbarPane>
                        <RadioTool name='powerTools'>
                            <RadioToolItem value={DiagramTool.Select} label='انتخاب' icon="sh-select" />
                            <RadioToolItem value={DiagramTool.TransmisionLine} label='خط انتقال' icon="sh-connect" />
                            <RadioToolItem value={DiagramTool.BoxedConnect} label={label} icon="sh-rectangle" />
                        </RadioTool>
                    </ToolbarPane>
                    <Divider />
                    <ToolbarPane>
                        <ButtonTool label="بزرگنمایی" onClick={onZoomIn} icon="sh-zoomin"></ButtonTool>
                        <ButtonTool label="کوچکنمایی" onClick={onZoomOut} icon="sh-zoomout"></ButtonTool>
                        <ButtonTool label="انجام" onClick={onRedo} icon="sh-rotateright"></ButtonTool>
                        <ButtonTool label="پاد انجام" onClick={onUndo} icon="sh-rotateleft"></ButtonTool>
                        <ButtonTool label="ذخیره محلی" icon="sh-export" onClick={onSaveLocal} />
                        <ButtonTool label="بازیابی محلی" icon="sh-import">
                            <input type='file' accept='application/json' style={{ display: 'none' }} onChange={onLoadLocal} />
                        </ButtonTool>
                        <ButtonTool label="پاک کردن" icon="sh-clear" onClick={onClear} />
                        <ButtonTool label="به روز رسانی" icon="sh-save" onClick={onDiagramUpdate} />
                        <ButtonTool label="نمایش SVG" icon="sh-svg" onClick={onSaveSvg} />
                        <Divider />
                        <MouseCoordinate x={mouse.mouseX} y={mouse.mouseY} />
                    </ToolbarPane>
                </div>
            )
        }
    }

    const onZoomIn = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();
        const diagram = diagramHandler.diagram;
        const p = diagramHandler.getMousePosition(diagram.Properties.width / 2, diagram.Properties.height / 2);
        diagramHandler.zoom(1.1, p);
        diagramEditor.invalidate();
    }

    const onZoomOut = () => {
        const diagramEditor = diagramEditorRef.current;
        const diagramHandler = diagramEditor.getDiagramHandler();
        const diagram = diagramHandler.diagram;
        const p = diagramHandler.getMousePosition(diagram.Properties.width / 2, diagram.Properties.height / 2);
        diagramHandler.zoom(0.9, p);
        diagramEditor.invalidate();
    }

    const changeDrawTool = useCallback((drawTool) => {
        setDrawTool(drawTool)
    }, [drawTool]);

    const mousePosition = useCallback((x, y) => {
        setMouse({ mouseX: x, mouseY: y })
    }, [mouse]);

    const diagramContext = {
        drawTool,
        changeDrawTool,
        mousePosition,
    };

    return (
        <DiagramContext.Provider value={{ ...diagramContext }}>
            <div className="animated fadeIn diagrameditor">
                {renderToolBar()}
                <div ref={targetRef} className="diagram-container">
                    <DiagramEditor
                        ref={diagramEditorRef}
                        diagramMode={diagramMode}
                        diagramId={diagramId}
                        zoneId={zoneId}
                        areaId={areaId}
                        width={width}
                        height={height} />
                </div>
            </div>
        </DiagramContext.Provider>
    );
}

export default NetworkDiagram;
