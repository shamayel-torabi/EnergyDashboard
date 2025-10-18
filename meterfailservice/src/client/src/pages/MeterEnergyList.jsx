import React, { useCallback, useEffect, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormSelect, CFormLabel,
    CPagination, CPaginationItem,
    CFormCheck, CFormSwitch, CSpinner
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { EquipmentEnergyChart } from './EquipmentEnergyChart';


const EnergyListTable = React.memo(({ date, abnormal }) => {
    const [loading, setLoading] = useState(false);
    const [meterEnergys, setMeterEnergys] = useState([]);
    const [pageSize, setPageSize] = useState(25);
    const [pageCounts, setPageCounts] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    useEffect(() => {
        setCurrentPage(1);
        getMeterEnergyByDate(date, currentPage, pageSize, abnormal);
    }, [date])

    useEffect(() => {
        getMeterEnergyByDate(date, currentPage, pageSize, abnormal);
    }, [currentPage, pageSize, abnormal])

    const getMeterEnergyByDate = async (date, currentPage, pageSize, abNormal) => {
        const apiUrl = 'https://meterfailservice.hrec.local/api/MetersEnergy';
        const url = `${apiUrl}/GetPaginatedMeterEnergy/${date.toJSON()}?pageIndex=${currentPage}&pageSize=${pageSize}&abnormal=${abNormal}`;

        setLoading(true)
        setMeterEnergys([])

        try {
            let response = await fetch(url);
            if (response.ok) {
                let result = await response.json();
                setLoading(false);
                setMeterEnergys(result.value.items);
                setPageCounts(result.value.totalPages);
                setCurrentPage(currentPage)
            }
            else {
                const err = await response.json();
                console.error(err);

                setLoading(false);
                setMeterEnergys([]);
                setPageCounts(0);
                setCurrentPage(1)
            }
        }
        catch (e) {
            console.error(e);

            setLoading(false);
            setMeterEnergys([]);
            setPageCounts(0);
            setCurrentPage(1)
        }
    }

    const handleInputChange = (event) => {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;

        setPageSize(value)
    }

    const changePage = (page) => {
        if (page === 0)
            return;
        if (page > pageCounts)
            return;

        setCurrentPage(page)
    }

    const anomalyChange = async (event, v) => {
        const value = event.target.checked;
        v.anomaly = value;

        const url = `https://meterfailservice.hrec.local/api/MetersEnergy/${v.id}`;
        const data = {
            anomaly: value
        };

        try {
            let request = new Request(url, {
                method: 'PUT',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                }),
                body: JSON.stringify(data)
            })

            let response = await fetch(request);
            if (!response.ok) {
                const err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            console.error(e);
        }
    }

    const renderMeterEnergyTableRows = () => {
        let rows = null;
        if (loading) {
            rows = (
                <tr>
                    <td colSpan="6" className="text-center">
                        <CSpinner style={{ width: '4rem', height: '4rem' }} color="danger" variant="grow" />
                    </td>
                </tr>
            );
        }
        else {
            if (meterEnergys && meterEnergys.length > 0) {
                rows = meterEnergys.map((meterEnergy, k) => {

                    let d = meterEnergy.hourEnergys.map((e, i) => {
                        let activeEnergy = Math.abs(e.activeExport - e.activeImport);
                        return { x: e.hour, y: activeEnergy }
                    })

                    return (
                        <tr key={k}>
                            <td style={{ width: "10%", verticalAlign: "middle" }}>{meterEnergy.stationName}</td>
                            <td style={{ width: "10%", verticalAlign: "middle" }}>{meterEnergy.name}</td>
                            <td style={{ width: "10%", verticalAlign: "middle" }}>{meterEnergy.serialNumber}</td>
                            <td style={{ width: "65%", verticalAlign: "middle" }}><EquipmentEnergyChart data={d} /></td>
                            <td className="text-center d-print-none" style={{ width: "5%", verticalAlign: "middle" }}>
                                <CFormCheck defaultChecked={meterEnergy.anomaly} onChange={event => anomalyChange(event, meterEnergy)} />
                            </td>
                        </tr>
                    );
                })
            }
        }

        return rows;
    }

    const renderPagination = () => {
        let pageItems = [];
        let start, end;

        if (currentPage < 3) {
            start = 1;
            end = start + 4;
        }
        else if (currentPage > pageCounts - 3) {
            start = pageCounts - 4;
            end = pageCounts
        }
        else {
            start = currentPage - 2;
            end = currentPage + 2
        }

        let first = (
            <CPaginationItem key={'first'} onClick={e => changePage(1)}>اولین</CPaginationItem>
        )

        let previous = (
            <CPaginationItem key={'previous'} onClick={e => changePage(currentPage - 1)}>قبل</CPaginationItem>
        )

        pageItems.push(first);
        pageItems.push(previous);

        for (let i = start; i <= end; i++) {
            let p = (
                <CPaginationItem key={i} active={i == currentPage} onClick={e => changePage(i)}>{i}</CPaginationItem>
            );
            pageItems.push(p);
        }

        let next = (
            <CPaginationItem key={'next'} onClick={e => changePage(currentPage + 1)}>بعد</CPaginationItem>
        )

        let last = (
            <CPaginationItem key={'last'} onClick={e => changePage(pageCounts)}>آخرین</CPaginationItem>
        )

        pageItems.push(next);
        pageItems.push(last);

        const pageTitle = `صفحه ${currentPage} از ${pageCounts}`

        return (
            <CRow>
                <CCol md={4}>
                    <CRow className="mb-1">
                        <CCol>
                            <CPagination>
                                {pageItems}
                            </CPagination>
                        </CCol>
                    </CRow>
                </CCol>
                <CCol md={4} className="text-center">
                    <CRow className="mb-1">
                        <CCol>
                            <CFormLabel>{pageTitle}</CFormLabel>
                        </CCol>
                    </CRow>
                </CCol>
                <CCol md={4}>
                    <CRow className="mb-1">
                        <CCol xs={6}>
                            <CFormLabel htmlFor="rowNumber" className="float-end">تعداد ردیف جدول</CFormLabel>
                        </CCol>
                        <CCol xs={6} >
                            <CFormSelect name="rowNumber" id="rowNumber" value={pageSize} onChange={handleInputChange}>
                                <option>10</option>
                                <option>25</option>
                                <option>50</option>
                                <option>100</option>
                            </CFormSelect>
                        </CCol>
                    </CRow>
                </CCol>
            </CRow>
        )
    }

    return (
        <>
            <div className="table-responsive">
                <table className="table table-sm table-hover table-bordered">
                    <thead className="thead-dark">
                        <tr>
                            <th className="text-center" style={{ width: "10%" }}>ایستگاه</th>
                            <th className="text-center" style={{ width: "10%" }}>نام تجهیز</th>
                            <th className="text-center" style={{ width: "10%" }}>شماره سریال</th>
                            <th className="text-center" style={{ width: "65%" }}>شکل</th>
                            <th className="text-center d-print-none" style={{ width: "5%" }}>ناهنجار</th>
                        </tr>
                    </thead>
                    <tbody>
                        {renderMeterEnergyTableRows()}
                    </tbody>
                </table>
            </div>
            <div>
                {renderPagination()}
            </div>
        </>
    )
});


const MeterEnergyList = () => {
    const [date, setDate] = useState(new Date(Date.now() - 86400000));
    const [abnormal, setAbnormal] = useState(false);

    const handleDateChange = useCallback((event) => {
        const value = event.value || new Date();
        setDate(value)
    }, [date]);

    const handleAbnormal = useCallback((event) => {
        setAbnormal(event.target.checked);
    }, [abnormal]);

    return (
        <CCard className="animated fadeIn" style={{ marginBottom: '0.1rem' }}>
            <CCardHeader>
                <strong>فهرست انرژی میتر</strong>
            </CCardHeader>
            <CCardBody>
                <CRow className="mb-2">
                    <CCol >
                        <CRow>
                            <CCol xs="auto">
                                <CFormLabel htmlFor="date">تاریخ</CFormLabel>
                            </CCol>
                            <CCol xs="auto">
                                <DatePickerComponent
                                    name="date"
                                    id="date"
                                    calendarMode="Islamic"
                                    format="dd MMM yyyy"
                                    enableRtl={true}
                                    firstDayOfWeek={6}
                                    value={date}
                                    change={handleDateChange}
                                    locale="fa">
                                    <Inject services={[Islamic]} />
                                </DatePickerComponent>
                            </CCol>
                        </CRow>
                    </CCol>
                    <CCol>
                        <CRow>
                            <CCol xs="auto">
                                <CFormLabel htmlFor="abnormal">ناهنجار</CFormLabel>
                            </CCol>
                            <CCol xs="auto">
                                <CFormSwitch defaultChecked={abnormal} onChange={handleAbnormal} />
                            </CCol>
                        </CRow>
                    </CCol>
                </CRow>
                <CRow>
                    <CCol>
                        <EnergyListTable date={date} abnormal={abnormal} />
                    </CCol>
                </CRow>
            </CCardBody>
        </CCard>
    )
}

export default MeterEnergyList;