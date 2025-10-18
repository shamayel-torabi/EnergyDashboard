import React from 'react';

const ToolbarPane = (props) => {
    return (
        <div className="toolbar">
            {props.children}
        </div>
    );
};

export { ToolbarPane }