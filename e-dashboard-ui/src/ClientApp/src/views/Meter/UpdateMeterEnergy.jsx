import React, { useEffect, useRef, useState } from 'react';
import {
    CCard, CCardBody, CCardHeader,
    CCol, CRow,
    CForm, CButton,
    CFormLabel, CFormCheck
} from '@coreui/react'

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { ArcGauge } from '@progress/kendo-react-gauges';
import { MessageList } from '../Message/MessageList';
import { useToast } from '../../components';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { apiUrl, notificationUrl } from '../../ApiConfig';

const ProgressMessaage = ({ messages }) => {
    const messageList = messages.map((m, i) => {
        return <li key={i}>{m}</li>
    });

    return (
        <ul>
            {messageList}
        </ul>
    )
};

const UpdateMeterEnergy = (props) => {
    const now = Date.now();

    const [result, setResult] = useState('');
    const [loading, setLoading] = useState(false);
    const [progress, setProgress] = useState(0);
    const [messages, setMessages] = useState([]);
    const [model, setModel] = useState({
        startDate: new Date(now - 7 * 86400000),
        endDate: new Date(now - 86400000),
        updateRecords: false,
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
            if (progressMessage.messageScope === 1)
                setProgress(progressMessage.progress)
        });

        hubConnection.current.on('progressText', (progressTextMessage) => {
            if (progressTextMessage.messageScope === 1) {
                setMessages(prevState => {
                    prevState.unshift(progressTextMessage.text)
                    if (prevState.length > 10)
                        prevState.pop();

                    return prevState;
                });
            }
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

    const updateMetersEnergyTable = async () => {
        let startDate = model.startDate;
        let endDate = model.endDate;
        let updateRecords = model.updateRecords;

        if (startDate.valueOf() > endDate.valueOf()) {
            toast.showToast("error", "تاریخ شروع باید از تاریخ پایان کوچکتر باشد");
            return;
        }

        const url = `${apiUrl}/MeterEnergys/UpdateMetersEnergy`;
        const accessToken = authService.accessToken;

        var payload = {
            startDate: startDate,
            endDate: endDate,
            update: updateRecords
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
                body: JSON.stringify(payload)
            })

            let response = await fetch(request);
            if (response.ok) {
                const data = await response.json();
                setResult(data.result);
            }
            else {
                const err = await response.json();
                console.error(err);
                toast.showToast("error", "خطا در به روز رسانی جدول انرژی میتر.");
                setResult('خطا در به روز رسانی جدول انرژی میتر.');
            }
        }
        catch (e) {
            console.error(e);
            toast.showToast("error", "خطای اتصال به سرور");
            setResult('خطا در اتصال به سرور.');
        }
        setLoading(false);
    }

    const handleSubmit = async (event) => {
        event.preventDefault();
        setProgress(0);
        setMessages([]);
        await updateMetersEnergyTable();
    }

    const arcCenterRenderer = (value, color) => {
        return (<h3 style={{ color: color }}>{value}%</h3>);
    };

    const resultText = loading ? null : result;
    const spin = loading ? <span className="mr-3 spinner-border spinner-border-sm">{' '}</span> : null;

    return (
        <CRow className="animated fadeIn">
            <CCol md="6">
                <CCard>
                    <CCardHeader>
                        <strong>به روز رسانی جدول انرژی میتر</strong>
                    </CCardHeader>
                    <CCardBody style={{ height: "calc(100vh - 105px - 4.3rem)" }}>
                        <CForm onSubmit={handleSubmit}>

                            <CRow className="mb-2">
                                <CCol>
                                    <CFormLabel htmlFor="startDate" >از تاریخ</CFormLabel>
                                </CCol>
                                <CCol>
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


                            <CRow className="mb-2">
                                <CCol>
                                    <CFormLabel htmlFor="endDate">لغایت</CFormLabel>
                                </CCol>
                                <CCol >
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
                                <CCol>
                                    <CFormLabel htmlFor="updateRecords"></CFormLabel>
                                </CCol>
                                <CCol >
                                    <CFormCheck label="رونویسی رکوردها"
                                        id="updateRecords"
                                        name="updateRecords"
                                        defaultChecked={model.updateRecords}
                                        onChange={handleInputChange}
                                    />
                                </CCol>
                            </CRow>
                            <CButton type="submit">
                                <span>ارسال</span>
                                {'   '}
                                {spin}
                            </CButton>
                        </CForm>
                        <hr />
                        <div className="text-center">
                            <ArcGauge
                                arcCenterRender={arcCenterRenderer}
                                value={progress}
                                transitions={false}
                                scale={{
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
                        <div className="text-center">
                            <ProgressMessaage messages={messages} />
                        </div>
                    </CCardBody>
                </CCard>
            </CCol>
            <CCol md="6">
                <CCard>
                    <CCardHeader>
                        <strong>فهرست پیامها</strong>
                    </CCardHeader>
                    <CCardBody style={{ height: "calc(100vh - 105px - 4.3rem)", overflowY: "scroll" }}>
                        <MessageList scope={1} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default UpdateMeterEnergy;
