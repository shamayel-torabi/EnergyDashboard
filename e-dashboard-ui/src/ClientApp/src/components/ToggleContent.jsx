import React, { useState } from 'react';
import { CCollapse } from '@coreui/react';

const ToggleContent = (props) => {

    const [collapse, setCollapse] = useState(false)

    const toggle = (e) => {
        let target = e.target;
        target.classList.toggle("caret-down");
        setCollapse(!collapse)
    }

    return (
        <li onClick={toggle}>
            <h3 className="caret">{props.title}</h3>
            <CCollapse visible={collapse}>
                {props.children}
            </CCollapse>
        </li>
    );
};


export { ToggleContent };
