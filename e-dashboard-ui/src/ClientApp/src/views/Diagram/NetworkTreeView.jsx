import { useEffect, useState, memo } from 'react';
import { useToast } from '../../components';
import { apiUrl } from '../../ApiConfig';

const NetworkTreeView = memo((props) => {
    const [loading, setLoading] = useState(true);
    const [network, setNetwork] = useState(null);

    const toast = useToast();

    useEffect(() => {
        const loadNetwork = async () => {
            const url = `${apiUrl}/Networks`;
            setLoading(true);

            try {
                let response = await fetch(url);
                if (response.ok) {
                    let network = await response.json();
                    let n = network[0] == undefined ? null : network[0]
                    setNetwork(n);
                }
                else {
                    toast.showToast('error', "خطا در دریافت شبکه!");
                    const problem = await response.json();
                    console.error(problem)
                }
            }
            catch (e) {
                toast.showToast('error', "خطای اتصال به سرور!");
                console.error(e)
            }
            setLoading(false);
        }

        loadNetwork();
    }, [])

    const toggle = (e) => {
        let target = e.target;
        let parent = target.parentElement;
        let aa = parent.querySelector(".nested");
        aa.classList.toggle("active");
        target.classList.toggle("caret-down");
    }

    const treeClick = (e) => {
        let target = e.target;
        let diagramMode = target.getAttribute("data-type");
        let zone = target.getAttribute("data-zone-id");
        let area = target.getAttribute("data-area-id");

        if (diagramMode && target.id && props.onDiagramClick)
            props.onDiagramClick(diagramMode, target.id, zone, area);
    }

    const renderTreeItem = () => {
        if (loading)
            return null;

        if (network == null || network.areas == null) {
            toast.showToast('error', "خطا در دریافت شبکه!");
            return null;
        }

        let areaHtml = network.areas.map((area) => {
            let zoneHtml = area.zones.map((zone) => {
                let diagramHtml = zone.substations.map((diagram) => {
                    return (
                        <li data-zone-id={zone.id} data-area-id={area.id} data-network-id={network.id} className="diagram-item" data-type="substation" id={diagram.id} key={diagram.id}>
                            {diagram.name}
                        </li>
                    )
                })
                return (
                    <li key={zone.id}>
                        <span className="caret" onClick={toggle}></span>
                        <span className="tree-item" data-type="zone" data-network-id={network.id} data-area-id={area.id} id={zone.id}>{zone.name}</span>
                        <ul className="nested" >
                            {diagramHtml}
                        </ul>
                    </li>
                )
            })
            return (
                <li key={area.id}>
                    <span className="caret" onClick={toggle}></span>
                    <span className="tree-item" data-type="area" data-network-id={network.id} id={area.id}>{area.name}</span>
                    <ul className="nested">
                        {zoneHtml}
                    </ul>
                </li>
            )
        })

        let networkHtml = (
            <li className="tree-content" key={network.id} onClick={treeClick}>
                <span className="caret" onClick={toggle} ></span>
                <span className="tree-item" data-type="network" id={network.id}>{network.name}</span>
                <ul className="nested">
                    {areaHtml}
                </ul>
            </li>
        )

        return networkHtml;
    }

    return (
        <ul className="tree-view">
            {renderTreeItem()}
        </ul>
    );
});

export { NetworkTreeView };
