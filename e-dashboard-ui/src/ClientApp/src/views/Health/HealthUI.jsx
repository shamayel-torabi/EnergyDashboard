import { useEffect, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol, CSpinner
} from '@coreui/react';


const HealthUI = (props) => {
    const [laoded, setLaoded] = useState(false);
    const [data, setData] = useState(null);

    useEffect(() => {
        const getHealthData = async () => {
            const url = `/healthz`;
            setLaoded(false);

            try {
                let response = await fetch(url);

                if (response.ok) {
                    let data = await response.json();
                    setData(data);
                }
                else {
                    let err = await response.json();
                    setData(null);
                    console.error(err)
                }
            }
            catch (e) {
                console.error('Error:', e);
                setData(null)
            }
            setLaoded(true);
        };

        getHealthData();
    }, []);

    const renderHealthCheck = () => {
        if (laoded && data) {
            let status = '';
            let style = {};

            if (data.status === 'Healthy') {
                status = 'سالم';
                style = {
                    color: '#00FF00'
                };
            }
            else if (data.status === 'Unhealthy') {
                status = 'ناسالم';
                style = {
                    color: '#FF0000'
                };
            }
            else {
                status = 'کاهش کیفیت ';
                style = {
                    color: '#FFFF00'
                };
            }

            let results = Object.entries(data.results).map((v, i) => {
                let subject = v[0];
                let details = v[1];
                let status = '';
                let style = {};

                if (details.status === 'Healthy') {
                    status = 'سالم';
                    style = {
                        color: '#00FF00'
                    };
                }
                else if (details.status === 'Unhealthy') {
                    status = 'ناسالم';
                    style = {
                        color: '#FF0000'
                    };
                }
                else {
                    status = 'کاهش کیفیت ';
                    style = {
                        color: '#FFFF00'
                    };
                }

                const description = details.description ? details.description : '';

                let dataList = null;

                if (details.data) {
                    dataList = Object.entries(details.data).map((d, i) => {
                        let title = d[0];
                        let status = d[1];

                        let style = {};

                        if (status) {
                            style = {
                                color: '#00FF00'
                            };
                        }
                        else {
                            style = {
                                color: '#FF0000'
                            };
                        }

                        return (<li key={i} style={style}>{`${title}`}</li>)
                    });
                }


                return (
                    <div key={i}>
                        <h3>{subject}</h3>
                        <label style={style}>{`وضعیت :  ${status}`}</label><br />
                        <label>{`شرح : ${description}`}</label>
                        <ul>
                            {dataList}
                        </ul>
                    </div>
                );

            });

            return (
                <>
                    <h1 style={style}>{`وضعیت کلی : ${status}`}</h1>
                    <hr />
                    <div>{results}</div>
                </>
            );
        }
        else
            return (
                <div className="text-center">
                    <CSpinner style={{ width: '4rem', height: '4rem' }} color="danger" variant="grow" />
                </div>
            );

    }

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>بررسی سلامتی وبگاه</strong>
                    </CCardHeader>
                    <CCardBody className="pb-5">
                        <div>{renderHealthCheck()}</div>
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}
export default HealthUI;
