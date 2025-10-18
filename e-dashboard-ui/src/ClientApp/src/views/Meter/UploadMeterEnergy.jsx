import { useEffect, useRef, useState } from 'react';
import {
    CRow, CCol,
    CCard, CCardHeader, CCardBody,
    CButton, CSpinner,
    CForm
} from '@coreui/react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { ArcGauge } from '@progress/kendo-react-gauges';
import { UploadFile } from '../../components';
import { useAuthorize } from '../../api-authorization'
import { useToast } from '../../components';
import { apiUrl, notificationUrl } from '../../ApiConfig';

const UploadMeterEnergy = (props) => {
    const [result, setResult] = useState('');
    const [loading, setLoading] = useState(false);
    const [progress, setProgress] = useState(0);
    const [message, setMessage] = useState('');
    const [model, setModel] = useState({
        quarterly: 1,
    });

    const toast = useToast();
    const hubConnection = useRef(null);
    const authService = useAuthorize();

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .build();


        hubConnection.current.on('progress', (progressMessage) => {
            setProgress(progressMessage.progress)
        });

        hubConnection.current.on('progressText', (progressTextMessage) => {
            setMessage(progressTextMessage.text)
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection :('));

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        };
    }, []);

    const formSubmit = async (event) => {
        event.preventDefault();
        let url;

        const accessToken = authService.accessToken;

        if (model.quarterly === 1)
            url = `${apiUrl}/Meters/UploadMeterFiles`;
        else
            url = `${apiUrl}/Meters/UploadMeterFilesHourly`;


        let form = event.target;
        let files = form.elements['files'].files;
        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            let file = files[i];

            formData.append('files', file);
        }

        setLoading(true);

        try {
            let request = new Request(url, {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${accessToken}`
                }),
                body: formData
            });

            let response = await fetch(request);
            if (response.ok) {
                let data = await response.json();
                setResult(data.result)
            }
            else {
                const err = await response.json();
                toast.showToast("error", "خطا در به روز رسانی.");
                console.error(err);
            }

        }
        catch (e) {
            console.error('Upload create error', e);
            toast.showToast("error", "خطای اتصال به سرور.");
        }

        setLoading(false);
    }

    const arcCenterRenderer = (value, color) => {
        return (<h3 style={{ color: color }}>{value}%</h3>);
    };

    const resultText = loading ? <CSpinner color="primary" /> : result;

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>بارگذاری فایل میتر</strong>
                    </CCardHeader>
                    <CCardBody>
                        <div>
                            <p>
                                شما می توانید فایلهای قرائت شده میترها به بانک اطلاعاتی اضافه کنید.
                                برای این منظور فایلها قرائت شده را از فهرست انتخاب کنید و برای درج در بانک اطلاعاتی دکمه ارسال را کلیک کنید.
                                در ادامه مراحل پیشرفت کار نشان داده خواهد شد.
                            </p>
                        </div>
                        <CForm encType="multipart/form-data" onSubmit={formSubmit}>
                            <CRow className="mb-3">
                                <CCol sm={12}>
                                    <UploadFile label="انتخاب فایل" multiple name="files"></UploadFile>
                                </CCol>
                            </CRow>
                            <CRow className="mb-3">
                                <CCol sm={12}>
                                    <CButton type="submit">ارسال</CButton>
                                </CCol>
                            </CRow>
                        </CForm>
                        <hr />
                        <div className="text-center">{message}</div>
                        <div className="text-center">
                            <ArcGauge
                                arcCenterRender={arcCenterRenderer}
                                value={progress}
                                transitions={false}
                                scale={{
                                    //labels: { format: 'c', color: labelsColor, visible: true },
                                    majorTicks: { visible: true, color: '#f44ad2' },
                                    minorTicks: { visible: true, color: '#f44ad2' },
                                    rangeSize: 10,
                                    rangeLineCap: 'round',
                                    rangePlaceholderColor: '#e6e5e5',
                                    startAngle: -180,
                                    endAngle: 180,
                                    reverse: true
                                }} />
                        </div>
                        <hr />
                        <div className="text-center">{resultText}</div>
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default UploadMeterEnergy;
