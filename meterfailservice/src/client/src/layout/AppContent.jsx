import React from 'react'
import { Outlet } from 'react-router-dom'
import { CContainer} from '@coreui/react'

const AppContent = () => {
  return (
    <CContainer fluid className="mt-1">
      <Outlet />
    </CContainer>
  )
}

export default React.memo(AppContent)
