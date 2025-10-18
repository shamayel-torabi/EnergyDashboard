import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel, CButton
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { Workbook } from '@progress/kendo-ooxml';
import { saveAs } from '@progress/kendo-file-saver';
import { Select } from '../../../components';
import { ToggleContent } from '../../../components/ToggleContent';
import { apiUrl } from '../../../ApiConfig';

class ReadStationEnergy extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            loading: true,
        }
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if ((nextProps.startDate !== this.props.startDate) || (nextProps.stationId !== this.props.stationId)) {
            let date = nextProps.startDate;
            let stationId = nextProps.stationId;
            await this.getSubstationEnergy(date, stationId);
            return true;
        }
        return false;
    }

    getSubstationEnergy = async (startDate, stationId) => {
        //const url = `${apiUrl}/api/EnergyProfiles/GetSubstationEnergy/${stationId}/${startDate.toJSON()}`;
        const url = `${apiUrl}/EnergyProfiles/GetSubstationEnergy/${stationId}/${startDate.toJSON()}`;

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
                console.error("Error GetSubstationEnergy");
                this.setState({
                    data: null,
                    loading: false,
                });
            }
        }
        catch (e) {
            console.log(e);
        }
    }

    renderSubstationEnergyTable(items) {
        let count = items.length;
        let sumExportWatt = 0, sumImportWatt = 0, sumExportVar = 0, sumImportVar = 0;
        return (
            <table className="table table-sm table-bordered table-hover">
                <caption>تعداد رکورد: {count}</caption>
                <thead className="thead-dark">
                    <tr className="d-flex">
                        <th className="col-4">تاریخ</th>
                        <th className="col-2">انرژی اکتیو خروجی (Wh)</th>
                        <th className="col-2">انرژی اکتیو ورودی (Wh)</th>
                        <th className="col-2">انرژی راکتیو خروجی (Varh)</th>
                        <th className="col-2">انرژی راکتیو ورودی (Varh)</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        items.map((d, k) => {
                            sumExportWatt += d.exportWatt;
                            sumImportWatt += d.importWatt;
                            sumExportVar += d.exportVar;
                            sumImportVar += d.importVar;
                            return (
                                <tr className="d-flex" key={k}>
                                    <td className="col-4">{new Date(d.recordDate).toLocaleString('fa-IR')}</td>
                                    <td className="col-2">{Number(d.exportWatt).toLocaleString('fa-IR')}</td>
                                    <td className="col-2">{Number(d.importWatt).toLocaleString('fa-IR')}</td>
                                    <td className="col-2">{Number(d.exportVar).toLocaleString('fa-IR')}</td>
                                    <td className="col-2">{Number(d.importVar).toLocaleString('fa-IR')}</td>
                                </tr>
                            );
                        })
                    }
                </tbody>
                <tfoot>
                    <tr className="d-flex">
                        <th className="col-4">جمع کل</th>
                        <th className="col-2">{sumExportWatt}</th>
                        <th className="col-2">{sumImportWatt}</th>
                        <th className="col-2">{sumExportVar}</th>
                        <th className="col-2">{sumImportVar}</th>
                    </tr>
                </tfoot>
            </table>
        );
    }

    exportToExcel = async ()=> {
        const data = this.state.data;

        let workBookSheets = data.map(d => {
            let header =
            {
                index: 0,
                cells: [
                    { index: 0, value: 'recordDate' },
                    { index: 1, value: 'importWatt' },
                    { index: 2, value: 'exportWatt' },
                    { index: 3, value: 'importVar' },
                    { index: 4, value: 'exportVar' },
                ]
            };

            let r = d.items.map((item, k) => {
                let row =
                {
                    index: k + 1,
                    cells: [
                        { index: 0, value: item.recordDate },
                        { index: 1, value: item.importWatt },
                        { index: 2, value: item.exportWatt },
                        { index: 3, value: item.importVar },
                        { index: 4, value: item.exportVar },
                    ]
                }
                return row;
            });

            let rows = [header, ...r];


            let workBookSheet = {
                name: d.equipmentName,
                columns: [
                    { index: 0, width: 150 },
                    { index: 1, width: 200 },
                    { index: 2, width: 200 },
                    { index: 3, width: 200 },
                    { index: 4, width: 200 },
                ],
                rows: rows
            }

            return workBookSheet;
        })

        let workbookOptions = {
            creator: 'Shamayel Torabi',
            date: new Date().toJSON(),
            sheets: workBookSheets
        } 

        let workbook = new Workbook(workbookOptions);
        let dataURI = await workbook.toDataURL();

        saveAs(dataURI, "text.xlsx");
    }
    
    export = () => {
        this.exportToExcel();
    }

    render() {
        if (this.state.loading)
            return (<span className="k-icon k-i-loading"></span>);

        let data = this.state.data;

        if (data && data.length) {
            let content = data.map((d, i) => {
                let table = this.renderSubstationEnergyTable(d.items);

                return (
                    <ToggleContent key={i} title={d.equipmentName}>
                        {table}
                    </ToggleContent>
                );
            })

            return (
                <div>
                    <CButton onClick={this.export}>ذخیره اکسل</CButton>
                    <hr />
                    <ul className="animated fadeIn substationenergy ">
                        {content}
                    </ul>
                </div>
            );
        }
        else {
            return (
                <div className="animated fadeIn pt-3 text-center">داده ای موجود نیست</div>
            );
        }
    }
}

class StationEnergy extends Component {
    constructor(props) {
        super(props);
        let yesterday = new Date(new Date().getTime() - 86400000);

        this.state = {
            stationsAuto: [],
            model: {
                recordDate: yesterday,
                stationId: '',
            }
        }
    }

    componentDidMount = async () => {
        await this.getStations();
    }

    getStations = async () => {
        //const url = `${apiUrl}/api/Utility/GetSubstations`;
        const url = `${apiUrl}/Utility/GetSubstations`;

        try {
            let response = await fetch(url);
            if (response.ok) {
                let st = await response.json();
                if(st && st.length){
                    let model = this.state.model;
                    model.stationId = st[0].value;
                    this.setState({
                        model,
                        stationsAuto:st
                    });   
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

    handleDateChange = (event) => {
        const value = event.value;
        let model = this.state.model;

        model.recordDate = value;

        this.setState({
            model: model
        });
    }

    handleInputChange = (event) => {
        let model = this.state.model;
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;

        model[event.target.name] = value;

        this.setState({
            model: model
        });
    }

    render() {
        let { stationsAuto, model } = this.state;
        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>گزارش ساعتی انرژی ایستگاه</strong>
                        </CCardHeader>
                        <CCardBody>
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
                                                onChange={this.handleInputChange} />
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
                                                value={model.recordDate}
                                                format="dd MMM yyyy"
                                                enableRtl={true}
                                                firstDayOfWeek={6}
                                                change={this.handleDateChange}
                                                locale="fa">
                                                <Inject services={[Islamic]} />
                                            </DatePickerComponent>
                                        </CCol>
                                    </CRow>
                                </CCol>
                            </CRow>
                            <hr />
                            <ReadStationEnergy startDate={model.recordDate} stationId={model.stationId} />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default StationEnergy;
