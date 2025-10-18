import { CCol, CContainer, CRow } from '@coreui/react'
import { Outlet } from 'react-router';

export const AuthLayout = () => {
    return (
        <div className="bg-light min-vh-100 d-flex flex-row align-items-center">
            <CContainer>
                <CRow className="justify-content-center">
                    <CCol>
                        <Outlet/>
                    </CCol>
                </CRow>
            </CContainer>
        </div>
    )
}

export default AuthLayout;

