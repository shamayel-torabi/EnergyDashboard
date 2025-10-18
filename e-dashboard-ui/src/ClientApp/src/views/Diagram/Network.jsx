import { useState, useRef, useCallback } from 'react';
import { NetworkTreeView } from './NetworkTreeView'
import { DiagramViewer } from './DiagramViewer';
import { useSizeDetector } from '../../components';

const Network = () => {
    const targetRef = useRef();
    const { width, height } = useSizeDetector({ targetRef })

    const [diagramState, setDiagramState] = useState({
        diagramMode: 'network',
        diagramId: '',
        zoneId: '',
        areaId: '',
    });

    const onDiagramClick = useCallback((diagramMode, diagramId, zoneId, areaId) => {
        setDiagramState({
            diagramMode: diagramMode,
            diagramId: diagramId,
            zoneId: zoneId,
            areaId: areaId,
        });
    }, [diagramState]);

    return (
        <div className="network-diagrams animated fadeIn">
            <div className="tree-control">
                <NetworkTreeView onDiagramClick={onDiagramClick} />
            </div>
            <div ref={targetRef} className="diagram-control">
                <DiagramViewer
                    diagramState={diagramState}
                    width={width}
                    height={height} />
            </div>
        </div>
    );
}

export default Network;

