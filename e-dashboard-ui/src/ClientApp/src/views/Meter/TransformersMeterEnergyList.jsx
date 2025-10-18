import { useEffect, useState } from 'react';

import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol, CSpinner,
    CFormSelect, CFormLabel, CFormCheck,
    CPagination, CPaginationItem
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { EquipmentEnergyChart } from './EquipmentEnergyChart';
import { useToast } from '../../components';
import { apiUrl } from '../../ApiConfig';

const EnergyListTable = ({ date }) => {
    const [loading, setLoading] = useState(false);
    const [meterEnergys, setMeterEnergys] = useState([]);
    const [pageSize, setPageSize] = useState(25);
    const [pageCounts, setPageCounts] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const toast = useToast();

    useEffect(() => {
        setCurrentPage(1);
        getMeterEnergyByDate(date, currentPage, pageSize);
    }, [date]);

    useEffect(() => {
        getMeterEnergyByDate(date, currentPage, pageSize);
    }, [currentPage, pageSize]);


    const getMeterEnergyByDate = async (d, cPage, pSize) => {
        const url = `${apiUrl}/MeterEnergys/GetPaginatedTransformersMeterEnergyByDate/${d.toJSON()}?pageIndex=${cPage}&pageSize=${pSize}`;

        setLoading(true);
        try {
            let response = await fetch(url);
            if (response.ok) {
                let result = await response.json();
                setMeterEnergys(result.items);
                setPageCounts(result.totalPages);
            }
            else {
                const err = await response.json();
                console.error(err);
                toast.showToast("error", "خطا در دریافت انرژی ترانس.");
                setMeterEnergys(null);
                setCurrentPage(1);
            }
        }
        catch (e) {
            console.error(e);
            toast.showToast("error", "خطای اتصال به سرور");
            setMeterEnergys(null);
            setCurrentPage(1);
        }

        setLoading(false);
    }

    const handlePageSizeChange = (event) => {
        const value = event.target.value;
        setPageSize(value)
    }


    const changePage = (page) => {
        if (page === 0)
            return;
        if (page > pageCounts)
            return;

        setCurrentPage(page);
    }

    const renderMeterEnergyTableRows = () => {
        let rows;
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
                rows = meterEnergys.map((v, k) => {

                    let d = v.dailyEnergy.map((e, i) => {
                        return { x: e.hour, y: e.activeEnergy }
                    })

                    return (
                        <tr key={k}>
                            <td style={{ width: "10%", verticalAlign: "middle" }}>{v.stationName}</td>
                            <td style={{ width: "15%", verticalAlign: "middle" }}>{v.name}</td>
                            <td style={{ width: "10%", verticalAlign: "middle" }}>{v.serialNumber}</td>
                            <td style={{ width: "60%", verticalAlign: "middle" }}><EquipmentEnergyChart data={d} /></td>
                            <td className="text-center d-print-none" style={{ width: "5%", verticalAlign: "middle" }}>
                                <CFormCheck disable defaultChecked={v.anomal} />
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
            <CPaginationItem key={'previous'} onClick={e => changePage(currentPage - 1)}>قبلی</CPaginationItem>
        )

        pageItems.push(first);
        pageItems.push(previous);

        for (let i = start; i <= end; i++) {
            let p = (
                <CPaginationItem key={i} active={i === currentPage} onClick={e => changePage(i)}>{i}</CPaginationItem>
            );
            pageItems.push(p);
        }

        let next = (
            <CPaginationItem key={'next'} onClick={e => changePage(currentPage + 1)}>بعدی</CPaginationItem>
        )

        let last = (
            <CPaginationItem key={'last'} onClick={e => changePage(pageCounts)}>آخرین</CPaginationItem>
        )

        pageItems.push(next);
        pageItems.push(last);

        const pageTitle = `صفحه ${currentPage} از ${pageCounts}`

        return (
            <CRow>
                <CCol>
                    <CRow className="mb-1">
                        <CPagination className="col-sm-12 col-form-label">
                            {pageItems}
                        </CPagination>
                    </CRow>
                </CCol>
                <CCol className="text-center">
                    <CRow className="mb-1">
                        <CFormLabel className="col-sm-12 col-form-label">{pageTitle}</CFormLabel>
                    </CRow>
                </CCol>
                <CCol>
                    <CRow className="mb-1">
                        <CCol xs={6}>
                            <CFormLabel htmlFor="rowNumber" className="float-end">تعداد ردیف جدول</CFormLabel>
                        </CCol>
                        <CCol xs={6} >
                            <CFormSelect name="rowNumber" id="rowNumber" value={pageSize} onChange={handlePageSizeChange}>
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
};

const TransformersMeterEnergyList = (props) => {
    const [date, setDate] = useState(new Date(Date.now() - 86400000))

    const handleDateChange = (event) => {
        const value = event.value || new Date();
        setDate(value)
    }

    return (
        <CCard className="animated fadeIn" style={{ marginBottom: '0.1rem' }}>
            <CCardHeader>
                <strong>فهرست انرژی ترانسفورماتورها</strong>
            </CCardHeader>
            <CCardBody>
                <CRow className="mb-1">
                    <CCol xs="12" sm="6" md="4" lg="3">
                        <CRow className="g-3 align-items-center">
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
                    <CCol xs="auto" sm="auto" md="auto" lg="auto"></CCol>
                </CRow>
                <CRow>
                    <CCol>
                        <EnergyListTable date={date} />
                    </CCol>
                </CRow>
            </CCardBody>
        </CCard>
    )
}

export default TransformersMeterEnergyList;