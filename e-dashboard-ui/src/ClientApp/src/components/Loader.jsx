import React from 'react';
import './Loader.scss';
import { CSpinner } from '@coreui/react';

export const Loader = () => {
    return (
        <div className="sb-loading">
            <CSpinner style={{ width: '3rem', height: '3rem' }} type="grow"/>
        </div>
    );
}

