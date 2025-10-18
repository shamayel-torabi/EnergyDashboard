import React from 'react';

const DiagramContext = React.createContext({});
const useDiagram = () => React.useContext(DiagramContext);

export { DiagramContext, useDiagram }

