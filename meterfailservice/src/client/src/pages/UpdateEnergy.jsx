import { useEffect, useRef, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormLabel, CButton
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';

const UpdateEnergy = () => {
    const now = Date.now();
    const [result, setResult] = useState('');
    const [loading, setLoading] = useState(false);
    const [recordDate, setRecordDate] = useState(new Date(now - 1 * 86400000));

    const handleDateChange = (event) => {
        const value = event.value || new Date();
        const name = event.element.name;

        setRecordDate(value)
    }

    const updateEnergyProfile = async () => {
        const url = 'https://meterfailservice.hrec.local/api/MetersEnergy';

        setLoading(true);
        setResult(`شروع به روز رسانی در تاریخ ${recordDate.toLocaleDateString('fa-IR')} .`);

        const data = {
            recordDate: recordDate.toISOString()
        };
        try {
            let request = new Request(url, {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                }),
                body: JSON.stringify(data)
            })

            let response = await fetch(request);
            if (response.ok) {
                setResult(`به روز رسانی در تاریخ ${recordDate.toLocaleDateString('fa-IR')} با موفقیت انجام شد.`);
            }
            else {
                const err = await response.json();
                console.error(err);

                setResult(err.error);
            }
        }
        catch (e) {
            setResult('خطای اتصال به سرور !');
            console.error(e);
        }

        setLoading(false);
    }

    const handleSubmit = async (event) => {
        event.preventDefault();
        await updateEnergyProfile();
    }

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
                                <CFormLabel htmlFor="recordDate" className="col-sm-4 col-form-label">تاریخ</CFormLabel>
                                <CCol sm={8}>
                                    <DatePickerComponent
                                        name="recordDate"
                                        id="recordDate"
                                        calendarMode="Islamic"
                                        value={recordDate}
                                        format="dd MMM yyyy"
                                        enableRtl={true}
                                        firstDayOfWeek={6}
                                        change={handleDateChange}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>
                            <CButton type="submit">
                                <span>ارسال</span>
                                {spin}
                            </CButton>
                        </CForm>
                    </CCardBody>
                </CCard>
            </CCol>
            <CCol lg="6">
                <CCard>
                    <CCardHeader>
                        <strong>فهرست پیامها</strong>
                    </CCardHeader>
                    <CCardBody style={{ height: "calc(100vh - 105px - 4.3rem)", overflowY: "scroll" }}>
                        {result}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    )
}

export default UpdateEnergy;