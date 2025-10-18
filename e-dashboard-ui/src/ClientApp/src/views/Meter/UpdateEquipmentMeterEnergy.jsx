import { useState, useEffect, useRef } from 'react';
import {
    CRow, CCol,
    CCard, CCardHeader, CCardBody,
    CFormLabel, CButton,
    CForm, CFormCheck,
    CProgress,
} from '@coreui/react';
import { ArcGauge } from '@progress/kendo-react-gauges';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Select } from '../../components';
import { MessageList } from '../Message/MessageList';
import { useToast } from '../../components';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { apiUrl, notificationUrl } from '../../ApiConfig';

const UpdateEquipmentMeterEnergy = (props) => {
    const now = Date.now();

    const [result, setResult] = useState('');
    const [loading, setLoading] = useState(false);
    const [progress, setProgress] = useState(0);
    const [message, setMessage] = useState('');
    const [stationsAuto, setStationsAuto] = useState([]);
    const [equipmentAuto, setEquipmentAuto] = useState([]);
    const [model, setModel] = useState({
        startDate: new Date(now - 7 * 86400000),
        endDate: new Date(now - 86400000),
        updateRecords: false,
        stationId: '',
        stationName: '',
        equipmentId: '',
        equipmentName: '',
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
            if (progressMessage.messageScope === 2)
                setProgress(progressMessage.progress)
        });

        hubConnection.current.on('progressText', (progressTextMessage) => {
            if (progressTextMessage.messageScope === 2)
                setMessage(progressTextMessage.text)
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection :('));

        getStations();

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        };
    }, []);

    const getStations = async () => {
        const url = `${apiUrl}/Utility/GetSubstations`

        try {
            let response = await fetch(url);
            if (response.ok) {
                let st = await response.json();
                if (st && st.length) {
                    setStationsAuto(st);

                    setModel(prevState => ({
                        ...prevState,
                        stationId: st[0].value,
                        stationName: st[0].label
                    }));

                    getEquipment(st[0].value);
                }
            }
            else {
                const err = await response.json();
                toast.showToast("error", err.title);
                console.error(err);
            }
        }
        catch (e) {
            console.error(e);
        }
    }

    const getEquipment = async (stationId) => {
        const url = `${apiUrl}/Substations/GetSubstationEquipment/${stationId}`;

        try {
            let response = await fetch(url);
            if (response.ok) {
                let equpments = await response.json();

                if (equpments && equpments.length) {
                    setEquipmentAuto(equpments);

                    setModel(prevState => ({
                        ...prevState,
                        equipmentId: equpments[0].value,
                        equipmentName: equpments[0].label
                    }));
                }
            }
            else {
                const err = await response.json();
                toast.showToast("error", err.title);
                console.error(err);
            }
        }
        catch (e) {
            console.error(e);
        }
    }

    const handleStationChange = async (event) => {
        const value = event.target.value;

        const index = stationsAuto.findIndex(x => x.value === value)
        const stationName = stationsAuto[index].label;
        const stationId = stationsAuto[index].value;

        setModel(prevState => ({
            ...prevState,
            stationId: stationId,
            stationName: stationName
        }));

        await getEquipment(value);
    }

    const handleEquipmentChange = (event) => {
        const value = event.target.value;

        const index = equipmentAuto.findIndex(x => x.value === value)
        const equipmentName = equipmentAuto[index].label;

        setModel(prevState => ({
            ...prevState,
            equipmentId: value,
            equipmentName: equipmentName
        }));
    }

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

    const updateEquipmentEnergyMeter = async () => {
        let startDate = model.startDate.toJSON();
        let endDate = model.endDate.toJSON();
        let updateRecords = model.updateRecords;
        let equipmentId = model.equipmentId;

        if (startDate.valueOf() > endDate.valueOf()) {
            toast.showToast("error", "تاریخ شروع باید از تاریخ پایان کوچکتر باشد");
            return;
        }

        const url = `${apiUrl}/MeterEnergys/UpdateEquipmentMeterEnergy`;
        const accessToken = authService.accessToken;

        var payload = {
            equipmentId: equipmentId,
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
                setResult(data.result)
            }
            else {
                const err = await response.json();
                toast.showToast("error", "خطا در به روز رسانی جدول پروفایل انرژی.");
                console.error(err);
                setResult('');
            }
        }
        catch (e) {
            toast.showToast("error", "خطا اتصال به سرور در به روز رسانی جدول پروفایل انرژی.");
            console.error(e);
            setResult('');
        }
        setLoading(false);
    }

    const handleSubmit = async (event) => {
        event.preventDefault();
        await updateEquipmentEnergyMeter();
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
                        <strong>بارگیری انرژی تجهیز از مدیریت شبکه</strong>
                    </CCardHeader>
                    <CCardBody style={{ height: "calc(100vh - 105px - 6.85rem)" }}>
                        <CForm onSubmit={handleSubmit}>
                            <CRow className="mb-1">
                                <CFormLabel htmlFor="stationId" className="col-sm-4 col-form-label">ایستگاه</CFormLabel>
                                <CCol sm={8}>
                                    <Select required
                                        name="stationId" id="stationId"
                                        options={stationsAuto}
                                        value={model.stationId}
                                        onChange={handleStationChange} />
                                </CCol>
                            </CRow>

                            <CRow className="mb-1">
                                <CFormLabel htmlFor="equipmentId" className="col-sm-4 col-form-label">تجهیز</CFormLabel>
                                <CCol sm={8}>
                                    <Select required
                                        name="equipmentId" id="equipmentId"
                                        options={equipmentAuto}
                                        value={model.equipmentId}
                                        onChange={handleEquipmentChange} />
                                </CCol>
                            </CRow>

                            <CRow className="mb-1">
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
                            <CRow className="mb-1">
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
                                <CCol sm={4}></CCol>
                                <CCol sm={8}>
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
                    <CCardBody style={{ height: "calc(100vh - 105px - 6.85rem)", overflowY: "scroll" }}>
                        <MessageList scope={1} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default UpdateEquipmentMeterEnergy;
