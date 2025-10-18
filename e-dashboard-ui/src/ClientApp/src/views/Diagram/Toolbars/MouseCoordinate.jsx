import React from 'react';

const MouseCoordinate = React.memo(({ x, y }) => {
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
    );
});

export { MouseCoordinate }
