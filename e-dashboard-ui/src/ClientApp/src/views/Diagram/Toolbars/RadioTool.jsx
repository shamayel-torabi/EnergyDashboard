import React, { Children, cloneElement } from 'react';
import PropTypes from 'prop-types';

const RadioTool = ({ children, name }) => {
    return (<>{Children.map(children, (child) => cloneElement(child, { name }))}</>);
};

RadioTool.prototype = {
    children: PropTypes.element,
    name: PropTypes.string.isRequired,
};

export { RadioTool }
