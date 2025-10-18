import { useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel,
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { PersianCalendarUtil } from '../../../components/PersianCalendarUtil';
import { TotalIndustrialEnergyChart } from './TotalIndustrialEnergyChart';

const IndustrialYear = (props) => {
    const date = new Date();

    const [model, setModel] = useState({
        hYear: PersianCalendarUtil.hijriYear(date),
        recordDate: date
    });

    const handleDateChange = (event) => {
        const value = event.value || new Date();

        const mdl = {
            hYear: PersianCalendarUtil.hijriYear(value),
            recordDate: value
        };

        setModel(mdl);
    }

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>نمودار مصرف سالیانه بارهای صنعتی</strong>
                    </CCardHeader>
                    <CCardBody>
                            <CRow>
                                <CCol xs="auto">
                                    <CFormLabel htmlFor="recordDate">سال</CFormLabel>
                                </CCol>
                                <CCol xs="auto">
                                    <DatePickerComponent
                                        name="recordDate"
                                        id="recordDate"
                                        calendarMode="Islamic"
                                        format="yyyy"
                                        enableRtl={true}
                                        firstDayOfWeek={6}
                                        value={model.recordDate}
                                        start="Decade"
                                        depth="Decade"
                                        change={handleDateChange}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>
                        <hr />
                        <TotalIndustrialEnergyChart year={model.hYear} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default IndustrialYear;
