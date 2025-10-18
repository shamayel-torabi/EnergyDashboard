import React, { useEffect, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormLabel, CButton,
    CNav, CNavItem, CNavLink, CTabContent, CTabPane
} from '@coreui/react';
import { Workbook } from '@progress/kendo-ooxml';
import { saveAs } from '@progress/kendo-file-saver';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { useToast } from '../../../components';
import { Select } from '../../../components';
import { PersianCalendarUtil } from '../../../components/PersianCalendarUtil';
import { apiUrl } from '../../../ApiConfig';

const ReadEquipmentEnergy = ({ year, month, stationName, equipmentName, equipmentId }) => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);
    const [activeTab, setActiveTab] = useState('1');
    const toast = useToast();

    useEffect(() => {
        if (equipmentId)
            getEquipmentEnergyProfileHour(equipmentId, year, month);
    }, [year, month, equipmentId]);


    const toggle = (tab) => {
        if (activeTab !== tab) {
            setActiveTab(tab)
        }
    }

    const getEquipmentEnergyProfileHour = async (eId, y, m) => {
        const url = `${apiUrl}/EnergyProfiles/GetEquipmentEnergyProfileHour/${eId}/${y}/${m}`;

        setLoading(true);
        try {
            let response = await fetch(url);
            if (response.ok) {
                let d = await response.json();
                setData(d)
            }
            else {
                toast.showToast("error", "خطا در دریافت انرژی ماهیانه ایستگاه.");
                setData(null);
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در دریافت انرژی ماهیانه ایستگاه.");
            setData(null);
            console.error(e);
        }
        setLoading(false);
    }

    const exportToExcel = async () => {
        const columnLabel = ['B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y'];

        const hour = [];
        for (var i = 0; i < 24; i++)
            hour.push(i);

        const eHeader = hour.map(i => {
            let H = 'H' + i;
            return { index: i + 1, value: H }
        })

        const headerCells = [{ index: 0, value: 'تاریخ' }, ...eHeader];

        let header =
        {
            index: 0,
            cells: headerCells
        };

        let rowCount = 1;
        let importEnergy = data.map((item, k) => {
            let rowCells = [];
            rowCells.push(new Date(item.date).toLocaleDateString('fa-IR'));

            hour.map(i => {
                if (item.energies[i])
                    rowCells.push(item.energies[i].importWatt)
                else
                    rowCells.push(0);
            });

            let cells = rowCells.map((v, k) => {
                return { index: k, value: v }
            })

            let row =
            {
                index: k + 1,
                cells: cells
            }

            rowCount++;
            return row;
        });

        rowCount = 1;
        let exportEnergy = data.map((item, k) => {
            let rowCells = [];
            rowCells.push(new Date(item.date).toLocaleDateString('fa-IR'));

            hour.map(i => {
                if (item.energies[i])
                    rowCells.push(item.energies[i].exportWatt)
                else
                    rowCells.push(0);
            });

            let cells = rowCells.map((v, k) => {
                return { index: k, value: v }
            })

            let row =
            {
                index: k + 1,
                cells: cells
            }

            rowCount++;
            return row;
        });

        let sumCells = [];
        sumCells.push({ index: 0, value: 'مجموع' });
        hour.map(i => {
            let c = columnLabel[i];
            let f = '=SUM(' + c + '2:' + c + rowCount + ')';
            sumCells.push({ index: i + 1, formula: f })
        });

        let sum = {
            index: rowCount++,
            cells: sumCells
        };

        let aRows = [header, ...importEnergy, sum];
        let rRows = [header, ...exportEnergy, sum];

        let columns = [{ index: 0, width: 100 }];
        hour.map((v, k) => {
            columns.push({ index: k + 1, width: 100 })
        })


        let importWorkBookSheet = {
            name: 'import-' + equipmentName,
            columns: columns,
            rows: aRows,
            rtl: true
        }

        let exportWorkBookSheet = {
            name: 'export-' + equipmentName,
            columns: columns,
            rows: rRows,
            rtl: true
        }


        let workbookOptions = {
            creator: 'Shamayel Torabi',
            date: new Date().toJSON(),
            sheets: [
                importWorkBookSheet,
                exportWorkBookSheet
            ]
        }

        let workbook = new Workbook(workbookOptions);
        let dataURI = await workbook.toDataURL();
        let fileName = `${stationName}-${equipmentName}`;
        saveAs(dataURI, fileName);
    }

    const renderSubstationEnergyTable = (items) => {
        let count = items.length;

        let hour = [];
        for (var i = 0; i < 24; i++)
            hour.push(i);

        let header = hour.map(i => {
            return (
                <th key={i}>{'H' + i}</th>
            )
        })

        return (
            <div>
                <CNav variant="tabs">
                    <CNavItem>
                        <CNavLink
                            active={activeTab === '1'}
                            onClick={() => { toggle('1'); }}
                        >
                            انرژی ورودی
                        </CNavLink>
                    </CNavItem>
                    <CNavItem>
                        <CNavLink
                            active={activeTab === '2'}
                            onClick={() => { toggle('2'); }}
                        >
                            انرژی خروجی
                        </CNavLink>
                    </CNavItem>
                    <CNavItem>
                        <CNavLink
                            active={activeTab === '3'}
                            onClick={() => { toggle('3'); }}
                        >
                            انرژی خروجی منهای ورودی
                        </CNavLink>
                    </CNavItem>
                </CNav>
                <CTabContent>
                    <CTabPane visible={activeTab === '1'} className="mt-2">
                        <div className="table-responsive">
                            <table className="table table-sm table-bordered table-hover">
                                <thead className="thead-dark">
                                    <tr className="text-center">
                                        <th>تاریخ</th>
                                        {header}
                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        items.map((d, k) => {

                                            let energies = d.energies.map((e, k) => {
                                                return (
                                                    <td key={k}>{Number(e.importWatt).toLocaleString('fa-IR')}</td>
                                                );
                                            });
                                            return (
                                                <tr key={k}>
                                                    <td>{new Date(d.date).toLocaleDateString('fa-IR', { year: 'numeric', month: '2-digit', day: '2-digit' })}</td>
                                                    {energies}
                                                </tr>
                                            );
                                        })
                                    }
                                </tbody>
                            </table>
                        </div>
                    </CTabPane>
                    <CTabPane visible={activeTab === '2'} className="mt-2">
                        <div className="table-responsive">
                            <table className="table table-sm table-bordered table-hover">
                                <thead className="thead-dark">
                                    <tr className="text-center">
                                        <th>تاریخ</th>
                                        {header}
                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        items.map((d, k) => {
                                            let energies = d.energies.map((e, k) => {
                                                return (
                                                    <td key={k}>{Number(e.exportWatt).toLocaleString('fa-IR')}</td>
                                                );
                                            });
                                            return (
                                                <tr key={k}>
                                                    <td>{new Date(d.date).toLocaleDateString('fa-IR', { year: 'numeric', month: '2-digit', day: '2-digit' })}</td>
                                                    {energies}
                                                </tr>
                                            );
                                        })
                                    }
                                </tbody>
                            </table>
                        </div>
                    </CTabPane>this
                    <CTabPane visible={activeTab === '3'} className="mt-2">
                        <div className="table-responsive">
                            <table className="table table-sm table-bordered table-hover">
                                <thead className="thead-dark">
                                    <tr className="text-center">
                                        <th>تاریخ</th>
                                        {header}
                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        items.map((d, k) => {
                                            let energies = d.energies.map((e, k) => {
                                                return (
                                                    <td key={k}>{Number(e.exportWatt - e.importWatt).toLocaleString('fa-IR')}</td>
                                                );
                                            });
                                            return (
                                                <tr key={k}>
                                                    <td>{new Date(d.date).toLocaleDateString('fa-IR', { year: 'numeric', month: '2-digit', day: '2-digit' })}</td>
                                                    {energies}
                                                </tr>
                                            );
                                        })
                                    }
                                </tbody>
                            </table>
                        </div>
                    </CTabPane>
                </CTabContent>
            </div>
        );
    }

    if (loading)
        return (<span className="k-icon k-i-loading"></span>);

    if (data && data.length) {
        let content = renderSubstationEnergyTable(data);

        return (
            <div className="animated fadeIn substationenergy ">
                <CButton onClick={e => exportToExcel()}>ذخیره اکسل</CButton>
                <hr />
                {content}
            </div>
        );
    }
    else {
        return (
            <div className="animated fadeIn pt-3 text-center">داده ای موجود نیست</div>
        );
    }
};

const SubstationEnergyMonthly = (props) => {
    const date = new Date();
    let y = PersianCalendarUtil.hijriYear(date);
    let m = PersianCalendarUtil.hijriMonth(date);
    let gd = PersianCalendarUtil.gregorianDate(y, m, 5);

    const [loading, setLoading] = useState(false);
    const [stationsAuto, setStationsAuto] = useState([]);
    const [equipmentAuto, setEquipmentAuto] = useState([]);
    const [model, setModel] = useState({
        hYear: y,
        hMonth: m,
        recordDate: gd,
        stationId: undefined,
        stationName: undefined,
        equipmentId: undefined,
        equipmentName: undefined,
    });

    const toast = useToast();

    useEffect(() => {
        getStations();
    }, []);

    const getStations = async () => {
        const url = `${apiUrl}/Utility/GetSubstations`

        setLoading(true);
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
        setLoading(false);
    }

    const getEquipment = async (sId) => {
        const url = `${apiUrl}/Substations/GetSubstationEquipment/${sId}`;

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
        const substation = stationsAuto[index];

        setModel(prevState => ({
            ...prevState,
            stationId: substation.value,
            stationName: substation.label
        }));

        await getEquipment(substation.value);
    }

    const handleEquipmentChange = (event) => {
        const value = event.target.value;

        const index = equipmentAuto.findIndex(x => x.value === value)
        const equipment = equipmentAuto[index];

        setModel(prevState => ({
            ...prevState,
            equipmentId: equipment.value,
            equipmentName: equipment.label
        }));
    }

    const handleDateChange = (event) => {
        const value = event.value || new Date();

        const m = PersianCalendarUtil.hijriMonth(value);
        const y = PersianCalendarUtil.hijriYear(value);

        const gd = PersianCalendarUtil.gregorianDate(model.hYear, model.hMonth, 5);

        setModel(prevState => ({
            ...prevState,
            hYear: y,
            hMonth: m,
            recordDate: gd
        }));
    }

    const content = loading ? <p>Loading ...</p> :
        <ReadEquipmentEnergy
            year={model.hYear}
            month={model.hMonth}
            stationName={model.stationName}
            equipmentName={model.equipmentName}
            equipmentId={model.equipmentId} />


    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>
                            <span>گزارش ماهیانه انرژی </span>{' '}
                            <span>{model.stationName}</span>{' - '}
                            <span>{model.equipmentName}</span>
                        </strong>
                    </CCardHeader>
                    <CCardBody>
                        <CForm className='needs-validation' noValidate id="meterForm">
                            <CRow>
                                <CCol>
                                    <CRow>
                                        <CCol>
                                            <CFormLabel htmlFor="stationId">ایستگاه</CFormLabel>
                                        </CCol>
                                        <CCol>
                                            <Select required
                                                name="stationId" id="stationId"
                                                options={stationsAuto}
                                                value={model.stationId}
                                                onChange={handleStationChange} />
                                        </CCol>
                                    </CRow>
                                </CCol>
                                <CCol>
                                    <CRow>
                                        <CCol>
                                            <CFormLabel htmlFor="equipmentId">تجهیز</CFormLabel>
                                        </CCol>
                                        <CCol>
                                            <Select required
                                                name="equipmentId" id="equipmentId"
                                                options={equipmentAuto}
                                                value={model.equipmentId}
                                                onChange={handleEquipmentChange} />
                                        </CCol>
                                    </CRow>
                                </CCol>

                                <CCol>
                                    <CRow>
                                        <CCol>
                                            <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                                        </CCol>
                                        <CCol>
                                            <DatePickerComponent
                                                name="recordDate"
                                                id="recordDate"
                                                calendarMode="Islamic"
                                                format="MMM yyyy"
                                                enableRtl={true}
                                                firstDayOfWeek={6}
                                                value={model.recordDate}
                                                start="Year"
                                                depth="Year"
                                                change={handleDateChange}
                                                locale="fa">
                                                <Inject services={[Islamic]} />
                                            </DatePickerComponent>
                                        </CCol>
                                    </CRow>
                                </CCol>
                            </CRow>
                        </CForm>
                        <hr />
                        {content}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default React.memo(SubstationEnergyMonthly);
