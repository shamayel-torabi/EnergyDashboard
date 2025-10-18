import React, { useCallback, useEffect, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormSelect, CFormLabel,
    CPagination, CPaginationItem,
    CFormCheck,
    CFormSwitch,
    CSpinner
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { EquipmentEnergyChart } from './EquipmentEnergyChart';
import { useToast } from '../../components';
import { apiUrl } from '../../ApiConfig';

const EnergyListTable = React.memo(({ date, abnormal }) => {
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState(null);
    const [pageSize, setPageSize] = useState(25);
    const [currentPage, setCurrentPage] = useState(1);

    const toast = useToast();

    useEffect(() => {
        setCurrentPage(1);
        getMeterEnergyByDate();
    }, [date , abnormal, pageSize]);

    useEffect(() => {
        getMeterEnergyByDate();
    }, [currentPage]);

    const getMeterEnergyByDate = async () => {
        const url = `${apiUrl}/MeterEnergys/GetPaginatedMeterEnergyByDate/${date.toJSON()}?pageIndex=${currentPage}&pageSize=${pageSize}&abnormal=${abnormal}`;

        setLoading(true);
        setResult(null);

        try {
            let response = await fetch(url);
            if (response.ok) {
                let result = await response.json();
                setResult(result);
            }
            else {
                toast.showToast("error", "خطا در دریافت انرژی میتر.");
                const err = await response.json();
                console.error(err);
                setResult(null)
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در دریافت انرژی میتر.");
            console.error(e);
            setResult(null)
        }
        setLoading(false);
    }

    const handleInputChange = (event) => {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;

        setPageSize(value)
    }

    const changePage = (page) => {
        if (page === 0)
            return;
        if (page > result.totalPages)
            return;

        setCurrentPage(page)
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
            if (result && result.items.length > 0) {
                rows = result.items.map((v, k) => {

                    let d = v.dailyEnergy.map((e, i) => {
                        return { x: e.hour, y: e.activeEnergy }
                    })

                    return (
                        <tr key={k}>
                            <td style={{ width: "3%", verticalAlign: "middle" }}>{v.stationId}</td>
                            <td style={{ width: "7%", verticalAlign: "middle" }}>{v.stationName}</td>
                            <td style={{ width: "15%", verticalAlign: "middle" }}>{v.name}</td>
                            <td style={{ width: "10%", verticalAlign: "middle" }}>{v.serialNumber}</td>
                            <td style={{ width: "60%", verticalAlign: "middle" }}><EquipmentEnergyChart data={d} /></td>
                            <td className="text-center d-print-none" style={{ width: "5%", verticalAlign: "middle" }}><CFormCheck disabled defaultChecked={v.anomal} /></td>
                        </tr>
                    );
                })
            }
        }

        return rows;
    }

    const renderPagination = () => {
        if (!result)
            return null;

        if (result.totalPages <= 1)
            return null;

        let pageItems = [];
        let start, end;

        if (currentPage < 3) {
            start = 1;
            end = start + 4;
        }
        else if (currentPage > result.totalPages - 3) {
            start = result.totalPages - 4;
            end = result.totalPages
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
            <CPaginationItem key={'last'} onClick={e => changePage(result.totalPages)}>آخرین</CPaginationItem>
        )

        pageItems.push(next);
        pageItems.push(last);

        const pageTitle = `صفحه ${currentPage} از ${result.totalPages}`

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
                        <CCol xs={9}>
                            <CFormLabel htmlFor="rowNumber" className="float-end">تعداد ردیف جدول</CFormLabel>
                        </CCol>
                        <CCol xs={3} >
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
                            <th className="text-center" style={{ width: "3%" }}>شناسه</th>
                            <th className="text-center" style={{ width: "7%" }}>ایستگاه</th>
                            <th className="text-center" style={{ width: "15%" }}>نام تجهیز</th>
                            <th className="text-center" style={{ width: "10%" }}>شماره سریال</th>
                            <th className="text-center" style={{ width: "60%" }}>شکل</th>
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