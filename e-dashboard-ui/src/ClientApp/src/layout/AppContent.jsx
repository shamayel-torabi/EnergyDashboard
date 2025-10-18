import React from 'react'
import { CContainer} from '@coreui/react'
import { Outlet } from 'react-router'

const AppContent = () => {
    return (
        <CContainer className="mt-2" fluid >
            <Outlet/>
        </CContainer >
    )
}

export default React.memo(AppContent)
