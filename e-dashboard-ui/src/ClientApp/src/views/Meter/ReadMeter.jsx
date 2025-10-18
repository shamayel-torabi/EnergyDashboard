import { useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel,
    CForm, CButton, CFormInput, CSpinner
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { useToast } from '../../components';
import { apiUrl } from '../../ApiConfig';

const ReadMeter = (props) => {
    const [loading, setLoading] = useState(false);
    const [records, setRecords] = useState(null);
    const [model, setModel] = useState({
        recordDate: new Date(Date.now() - 86400000),
        serialNumber: '',
    })

    const toast = useToast();
    const authService = useAuthorize();

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

    const handleSubmit = async (event) => {
        event.preventDefault();
        const { recordDate, serialNumber } = model;
        await fetchMeterRecords(recordDate, recordDate, serialNumber);
    }

    const fetchMeterRecords = async (startDate, endDate, serialNumber) => {
        const url = `${apiUrl}/MeterEnergys/UpdateMeterEnergy`;
        const accessToken = authService.accessToken;
        
        setLoading(true);

        var payload = {
            startDate: startDate,
            endDate: endDate,
            serialNumber: serialNumber,
            update: false
        }

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
                let r = response.json();
                setRecords(r)
            }
            else if (response.status == 404) {
                const err = await response.json();
                toast.showToast("error", "خطای عدم مجوز دسترسی.");
                console.error(err);
                setRecords(null);
            }
            else {
                const err = await response.json();
                toast.showToast("error", err.title);
                console.error(err);
                setRecords(null);
            }
        }
        catch (e) {
            toast.showToast("error", "خطای اتصال به سرور");
            console.error(e);
            setRecords(null);
        }

        setLoading(false)
    }

    const renderRecordsTable = () => {
        let data = records;
        if (data && data.length) {
            let count = data.length;
            return (
                <table className="table table-sm table-bordered table-hover">
                    <caption>تعداد رکورد: {count}</caption>
                    <thead className="thead-dark">
                        <tr className="d-flex">
                            <th className="col-2">شناسه میتر</th>
                            <th className="col-2">تاریخ</th>
                            <th className="col-2">انرژی اکتیو خروجی (MWh)</th>
                            <th className="col-2">انرژی اکتیو ورودی (MWh)</th>
                            <th className="col-2">انرژی راکتیو خروجی (MVarh)</th>
                            <th className="col-2">انرژی راکتیو ورودی (MVarh)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            data.map((d, k) =>
                                <tr className="d-flex" key={k}>
                                    <td className="col-2">{d.meterId}</td>
                                    <td className="col-2">{new Date(d.recordDate).toLocaleString('fa-IR')}</td>
                                    <td className="col-2">{d.energyActiveExport}</td>
                                    <td className="col-2">{d.energyActiveImport}</td>
                                    <td className="col-2">{d.energyReactiveExport}</td>
                                    <td className="col-2">{d.energyReactiveImport}</td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            );
        }
        else {
            return (
                <div className="animated fadeIn pt-3 text-center">داده ای از سایت مدیریت شبکه دریافت نشد</div>
            );
        }
    }

    const renderRecords = () => {
        const content = renderRecordsTable()
        if (loading)
            return (
                <div className="animated fadeIn pt-3 text-center">
                    <CSpinner style={{ width: '4rem', height: '4rem' }} color="danger" variant="grow" />
                </div>
            );
        else {
            return (
                <div>
                    {content}
                </div>
            );
        }
    }

    return (
        <div className="animated fadeIn">
            <CRow>
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>دریافت اطلاعات از مدیریت شبکه</strong>
                        </CCardHeader>
                        <CCardBody>
                            <CForm className='row gx-3 gy-2 align-items-center needs-validation' onSubmit={handleSubmit} noValidate id="meterForm">
                                <CCol sm={5}>
                                    <CRow className="g-3 align-items-center">
                                        <CCol xs="auto">
                                            <CFormLabel htmlFor="serialNumber">شماره سریال</CFormLabel>
                                        </CCol>
                                        <CCol xs="auto">
                                            <CFormInput
                                                name="serialNumber"
                                                id="serialNumber"
                                                value={model.serialNumber}
                                                onChange={handleInputChange}
                                            />
                                        </CCol>
                                    </CRow>
                                </CCol>
                                <CCol sm={5}>
                                    <CRow className="g-3 align-items-center">
                                        <CCol xs="auto">
                                            <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                                        </CCol>
                                        <CCol xs="auto">
                                            <DatePickerComponent
                                                name="recordDate"
                                                id="recordDate"
                                                calendarMode="Islamic"
                                                value={model.recordDate}
                                                format="dd MMM yyyy"
                                                enableRtl={true}
                                                firstDayOfWeek={6}
                                                change={handleDateChange}
                                                locale="fa">
                                                <Inject services={[Islamic]}
                                                />
                                            </DatePickerComponent>
                                        </CCol>
                                    </CRow>
                                </CCol>
                                <CCol xs="auto">
                                    <CButton className="float-end" type="submit">ارسال</CButton>
                                </CCol>
                            </CForm>
                            <hr />
                            {renderRecords()}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        </div>
    );
}
export default ReadMeter;
