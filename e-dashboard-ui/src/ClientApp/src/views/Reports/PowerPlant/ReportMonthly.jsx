import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormLabel, CButton,
    CNav, CNavItem, CNavLink, CTabContent, CTabPane
} from '@coreui/react';
import { Workbook } from '@progress/kendo-ooxml';
import { saveAs } from '@progress/kendo-file-saver';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { withToast , Select, PersianCalendarUtil} from '../../../components';
import { apiUrl } from '../../../ApiConfig';

const ReadEquipmentEnergy = withToast(class extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            loading: false,
            activeTab: '1'
        }
    }

    toggle = (tab) => {
        if (this.state.activeTab !== tab) {
            this.setState({
                activeTab: tab
            });
        }
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if ((nextProps.year !== this.props.year) || (nextProps.month !== this.props.month) || (nextProps.unitId !== this.props.unitId)) {
            let year = nextProps.year;
            let month = nextProps.month;
            let unitId = nextProps.unitId;
            await this.getUnitEnergyProfileHour(unitId, year, month);
            return true;
        }
        return false;
    }

    getUnitEnergyProfileHour = async (unitId, year, month) => {
        if(unitId === undefined)
            return;

        //const url = `${apiUrl}/api/EnergyProfiles/GetUnitEnergyProfileHour/${unitId}/${year}/${month}`;
        const url = `${apiUrl}/EnergyProfiles/GetUnitEnergyProfileHour/${unitId}/${year}/${month}`;

        this.setState({
            loading: true,
        })

        try {
            let response = await fetch(url);
            if (response.ok) {
                let data = await response.json();

                this.setState({
                    data: data,
                    loading: false,
                });
            }
            else {
                this.props.showToast("error", "خطا در دریافت انرژی ماهیانه واحد نیروگاهی.");
                console.error("errr");
                this.setState({
                    data: null,
                    loading: false,
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در دریافت انرژی ماهیانه واحد نیروگاهی.");
            this.setState({
                data: null,
                loading: false,
            });
            console.error(e);
        }
    }

    exportToExcel = async () => {
        const data = this.state.data;
        const columnLabel = ['B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y'];

        const hour = [];
        for (var i = 0; i < 24; i++)
            hour.push(i);

        const eHeader = hour.map(i => {
            let H = 'H' + i;
            return { index: i + 1, value:H}
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
                return { index: k, value:v}
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
        hour.map((v,k) => {
            columns.push({ index: k+1, width: 100 })
        })


        let importWorkBookSheet = {
            name: 'import-' + this.props.unitName,
            columns: columns,
            rows: aRows,
            rtl:true
        }

        let exportWorkBookSheet = {
            name: 'export-' + this.props.unitName,
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
        let fileName = `${this.props.powerPlantName}-${this.props.unitName}`;
        saveAs(dataURI, fileName);
    }

    export = () => {
        this.exportToExcel();
    }

    renderSubstationEnergyTable(items) {
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
            <>
                <CNav variant="tabs">
                    <CNavItem>
                        <CNavLink
                            active={this.state.activeTab === '1'}
                            onClick={() => { this.toggle('1'); }}
                        >
                            جدول انرژی ورودی
                          </CNavLink>
                    </CNavItem>
                    <CNavItem>
                        <CNavLink
                            active={this.state.activeTab === '2'}
                            onClick={() => { this.toggle('2'); }}
                        >
                            جدول انرژی خروجی
                        </CNavLink>
                    </CNavItem>
                </CNav>
                <CTabContent>
                    <CTabPane visible={this.state.activeTab === '1'} className="mt-2">
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
                                                    <td>{new Date(d.date).toLocaleDateString('fa-IR',{ year: 'numeric', month: '2-digit', day: '2-digit' })}</td>
                                                    {energies}
                                                </tr>
                                            );
                                        })
                                    }
                                </tbody>
                            </table>
                        </div>
                    </CTabPane>
                    <CTabPane visible={this.state.activeTab === '2'} className="mt-2">
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
                                                    <td>{new Date(d.date).toLocaleDateString('fa-IR',{ year: 'numeric', month: '2-digit', day: '2-digit' })}</td>
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
            </>
        );
    }

    render() {
        if (this.state.loading)
            return (<span className="k-icon k-i-loading"></span>);

        let data = this.state.data;
        if (data && data.length) {
            let content = this.renderSubstationEnergyTable(data);

            return (
                <div className="animated fadeIn substationenergy ">
                    <CButton onClick={this.export}>ذخیره اکسل</CButton>
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
    }
});

class ReportMonthly extends Component {
    constructor(props) {
        super(props);

        const date = new Date();
        let y = PersianCalendarUtil.hijriYear(date);
        let m = PersianCalendarUtil.hijriMonth(date);
        let gd = PersianCalendarUtil.gregorianDate(y, m, 1);

        this.state = {
            powerPlantsAuto: [],
            equipmentAuto:[],
            model: {
                hYear: y,
                hMonth: m,
                recordDate: gd,
                powerPlantId: undefined,
                powerPlantName: '',
                unitId: undefined,
                unitName:'',
            }
        }
    }

    componentDidMount = async () => {
        await this.getPowerPlants();
    }

    getPowerPlants = async () => {
        //const url = `${apiUrl}/api/Utility/GetPowerplantOperators`;
        const url = `${apiUrl}/Utility/GetPowerplantOperators`;

        try {
            let response = await fetch(url);
            
            if (response.ok) {
                let ppOperator = await response.json();
                if (ppOperator && ppOperator.length) {
   
                    let model = this.state.model;
                    model.powerPlantId = ppOperator[0].value;
                    model.powerPlantName = ppOperator[0].label;
                    this.setState({
                        model,
                        powerPlantsAuto: ppOperator
                    });
    
                    this.getUnits(ppOperator[0].value);    
                }
            }
            else {
                console.error(response.statusText);
            }
        }
        catch (e) {
            console.error(e);
        }
    }

    getUnits = async (powerPlantId) => {
        //const url = `${apiUrl}/api/Utility/GetPowerplantUnits/${powerPlantId}`;
        const url = `${apiUrl}/Utility/GetPowerplantUnits/${powerPlantId}`;

        try {
            let response = await fetch(url);

            if (response.ok) {
                let units = await response.json();

                if (units && units.length > 0) {
                    let model = this.state.model;
                    model.unitId = units[0].value;
                    model.unitName = units[0].label;
                    this.setState({
                        model,
                        equipmentAuto: units
                    });
                }
                else {
                    let model = this.state.model;
                    model.unitId = undefined;

                    this.setState({
                        model,
                        equipmentAuto: []
                    });
                }
            }
            else {
                this.setState({
                    equipmentAuto: []
                });

                console.error(response.statusText);
            }
        }
        catch (e) {
            console.error(e);
        }
    }

    handleDateChange = (event) => {
        const value = event.value || new Date();
        let model = this.state.model;

        let hMonth = PersianCalendarUtil.hijriMonth(value);
        let hYear = PersianCalendarUtil.hijriYear(value);

        model.hMonth = hMonth;
        model.hYear = hYear;

        this.setState({
            model: model
        });
    }

    handlePowerPlantChange = async (event) => {
        let model = this.state.model;
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;

        model[event.target.name] = value;

        let index = this.state.powerPlantsAuto.findIndex(x => x.value == value)
        model.powerPlantName = this.state.powerPlantsAuto[index].label;

        await this.getUnits(value);

        this.setState({
            model: model
        });
    }

    handleEquipmentChange = (event) => {
        let model = this.state.model;
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;

        model[event.target.name] = value;

        let index = this.state.equipmentAuto.findIndex(x => x.value == value)
        model.unitName = this.state.equipmentAuto[index].label;

        this.setState({
            model: model
        });
    }

    render() {
        let { powerPlantsAuto, equipmentAuto, model } = this.state;
        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>
                                <span>گزارش ماهیانه انرژی نیروگاه </span>{' '}
                                <span>{model.powerPlantName}</span>{' - '}
                                <span>{model.unitName}</span>
                            </strong>
                        </CCardHeader>
                        <CCardBody>
                            <CForm className='needs-validation' onSubmit={this.handleSubmit} noValidate id="meterForm">
                                <CRow>
                                    <CCol>
                                        <CRow>
                                            <CCol>
                                                <CFormLabel htmlFor="stationId">نیروگاه</CFormLabel>
                                            </CCol>
                                            <CCol>
                                                <Select required
                                                    name="powerPlantId" id="powerPlantId"
                                                    options={powerPlantsAuto}
                                                    value={model.powerPlantId}
                                                    onChange={this.handlePowerPlantChange} />
                                            </CCol>
                                        </CRow>
                                    </CCol>
                                    <CCol>
                                        <CRow>
                                            <CCol>
                                                <CFormLabel htmlFor="unitId">تجهیز</CFormLabel>
                                            </CCol>
                                            <CCol>
                                                <Select required
                                                    name="unitId" id="unitId"
                                                    options={equipmentAuto}
                                                    value={model.unitId}
                                                    onChange={this.handleEquipmentChange} />
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
                                                    change={this.handleDateChange}
                                                    locale="fa">
                                                    <Inject services={[Islamic]} />
                                                </DatePickerComponent>
                                            </CCol>
                                        </CRow>
                                    </CCol>
                                </CRow>
                            </CForm>
                            <hr />
                            <ReadEquipmentEnergy
                                year={model.hYear}
                                month={model.hMonth}
                                powerPlantName={model.powerPlantName}
                                unitName={model.unitName}
                                unitId={model.unitId} />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default ReportMonthly;
