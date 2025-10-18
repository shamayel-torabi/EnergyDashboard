import { useRouteError } from 'react-router-dom'
import { isRouteErrorResponse } from "@remix-run/router"
import { CCol, CContainer, CRow, CCard, CCardBody, CCardHeader, CCardFooter } from '@coreui/react'
import { Link } from 'react-router-dom';

export default function ErrorPage() {
    let error = useRouteError();
    let message = isRouteErrorResponse(error)
        ? `${error.status} ${error.statusText}`
        : error instanceof Error
            ? error.message
            : JSON.stringify(error);
    let stack = error instanceof Error ? error.stack : null;
    let lightgrey = "rgba(200,200,200, 1)";
    let red = "rgba(255,0,0, 0.5)";
    let preStyles = { padding: "0.5rem", backgroundColor: lightgrey };

    return (
        <div className="bg-light min-vh-100 d-flex flex-row align-items-center">
            <CContainer>
                <CRow className="justify-content-center">
                    <CCol >
                        <CCard>
                            <CCardHeader>
                                <h4 style={{ color: red }}>رخداد خطا !!</h4>
                            </CCardHeader>
                            <CCardBody dir='ltr'>
                                <h3 style={{ fontStyle: "italic" }}>{message}</h3>
                                {stack ? <pre style={preStyles}>{stack}</pre> : null}
                            </CCardBody>
                            <CCardFooter className='text-center'>
                                <Link className="text-dark" to="/">بازگشت به خانه</Link>
                            </CCardFooter>
                        </CCard>
                    </CCol>
                </CRow>
            </CContainer>
        </div>
    );
}