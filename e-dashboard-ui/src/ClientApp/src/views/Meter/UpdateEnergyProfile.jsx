import { useEffect, useRef, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormCheck, CFormLabel,
    CProgress, CButton
} from '@coreui/react';
import { ArcGauge } from '@progress/kendo-react-gauges';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { MessageList } from '../Message/MessageList';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { useToast } from '../../components';
import { apiUrl, notificationUrl } from '../../ApiConfig';

const UpdateEnergyProfile = (props) => {
    const now = Date.now();

    const [result, setResult] = useState('');
    const [loading, setLoading] = useState(false);
    const [progress, setProgress] = useState(0);
    const [message, setMessage] = useState('');
    const [model, setModel] = useState({
        startDate: new Date(now - 7 * 86400000),
        endDate: new Date(now - 86400000),
        updateRecords: false,
    });

    const toast = useToast();
    const authService = useAuthorize();
    const hubConnection = useRef(null);

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .build();


        hubConnection.current.on('progress', (progressMessage) => {
            if (progressMessage.messageScope === 2)
                setProgress(progressMessage.progress)
        });

        hubConnection.current.on('progressText', (progressTextMessage) => {
            if (progressTextMessage.messageScope === 2)
                setMessage(progressTextMessage.text)
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection :('));

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        };
    }, []);

    const handleInputChange = (event) => {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        const name = event.target.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const handleDateChange = (event) => {
        const value = event.value || new Date();
        const name = event.element.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const updateEnergyProfile = async () => {
        const { startDate, endDate, updateRecords } = model;

        if (startDate.valueOf() > endDate.valueOf()) {
            toast.showToast("error", "تاریخ شروع باید از تاریخ پایان کوچکتر باشد");
            return;
        }

        const url = `${apiUrl}/EnergyProfiles/UpdateEnergyProfile/${startDate.toJSON()}/${endDate.toJSON()}/${updateRecords}`;
        const accessToken = authService.accessToken;

        setLoading(true);
        setProgress(0);

        try {
            let request = new Request(url, {
                method: 'GET',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${accessToken}`
                })
            })

            let response = await fetch(request);
            if (response.ok) {
                const data = await response.json();
                setResult(data.result)
            }
            else {
                const err = await response.json();
                console.error(err);
                toast.showToast("error", "خطا در به روز رسانی جدول پروفایل انرژی.");
                setResult('خطا در به روز رسانی جدول پروفایل انرژی.');
            }
        }
        catch (e) {
            toast.showToast("error", "خطای اتصال به سرور");
            console.error(e);
            setResult('خطای اتصال به سرور');
        }

        setLoading(false);
    }

    const handleSubmit = async (event) => {
        event.preventDefault();
        await updateEnergyProfile();
    }

    const arcCenterRenderer = (value, color) => {
        return (<h3 style={{ color: color }}>{value}%</h3>);
    };

    const resultText = loading ? null : result;
    const spin = loading ? <span className="mr-3 spinner-border spinner-border-sm"></span> : null;

    return (
        <CRow className="animated fadeIn">
            <CCol lg="6">
                <CCard>
                    <CCardHeader>
                        <strong>به روز رسانی انرژی تجهیزات</strong>
                    </CCardHeader>
                    <CCardBody style={{ height: "calc(100vh - 105px - 4.3rem)" }}>
                        <CForm onSubmit={handleSubmit}>
                            <CRow>
                                <CFormLabel htmlFor="startDate" className="col-sm-4 col-form-label">از تاریخ</CFormLabel>
                                <CCol sm={8}>
                                    <DatePickerComponent
                                        name="startDate"
                                        id="startDate"
                                        calendarMode="Islamic"
                                        value={model.startDate}
                                        format="dd MMM yyyy"
                                        enableRtl={true}
                                        firstDayOfWeek={6}
                                        change={handleDateChange}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>
                            <CRow>
                                <CFormLabel htmlFor="endDate" className="col-sm-4 col-form-label">لغایت</CFormLabel>
                                <CCol sm={8}>
                                    <DatePickerComponent
                                        name="endDate"
                                        id="endDate"
                                        calendarMode="Islamic"
                                        value={model.endDate}
                                        format="dd MMM yyyy"
                                        enableRtl={true}
                                        firstDayOfWeek={6}
                                        change={handleDateChange}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>
                            <CRow className="mb-1">
                                <CFormLabel htmlFor="updateRecords" className="col-sm-4 col-form-label"></CFormLabel>
                                <CCol sm={8}>
                                    <CFormCheck label="رونویسی رکوردها؟"
                                        id="updateRecords"
                                        name="updateRecords"
                                        defaultChecked={model.updateRecords}
                                        onChange={handleInputChange}
                                    />
                                </CCol>
                            </CRow>


                            <CButton type="submit">
                                <span>ارسال</span>
                                {spin}
                            </CButton>
                        </CForm>
                        <hr />
                        <div className="text-center">{message}</div>
                        <CProgress animated value={progress}></CProgress>
                        <hr />
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
            <CCol lg="6">
                <CCard>
                    <CCardHeader>
                        <strong>فهرست پیامها</strong>
                    </CCardHeader>
                    <CCardBody style={{ height: "calc(100vh - 105px - 4.3rem)", overflowY: "scroll" }}>
                        <MessageList scope={2} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default UpdateEnergyProfile;
