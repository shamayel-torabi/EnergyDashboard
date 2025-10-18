import React, { Component } from 'react';

import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel,
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { Select } from '../../../components';
import { apiUrl } from '../../../ApiConfig';

class SubstationEnergyBalanceComponent extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: true,
            data: undefined,
        }
    }

    componentDidMount = async () => {
        let { startDate, stationId } = this.props;
        await this.getSubstationEnergyBalance(stationId, startDate);
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if ((nextProps.startDate !== this.props.startDate) || (nextProps.stationId !== this.props.stationId) ) {
            await this.getSubstationEnergyBalance(nextProps.stationId, nextProps.startDate);
            return true;
        }
        return false;
    }

    getSubstationEnergyBalance = async (stationId, date) => {
        //const url = `${apiUrl}/api/DailyEnergy/SubstationEnergyBalance/${stationId}/${date.toJSON()}`;
        const url = `${apiUrl}/DailyEnergy/SubstationEnergyBalance/${stationId}/${date.toJSON()}`;

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
                console.log("errr");
                this.setState({
                    data: undefined,
                    loading: false,
                });
            }
        }
        catch (e) {
            console.error(e);
        }
    }

    renderEnergyBalanceTable(items) {
        let count = items.length;
        let sumExportWatt = 0, sumImportWatt = 0, sumExportVar = 0, sumImportVar = 0;

        return (
            <table className="table table-sm table-bordered table-hover">
                <caption>تعداد تجهیز: {count}</caption>
                <thead className="thead-dark">
                    <tr className="d-flex text-center">
                        <th className="col-4">نام تجهیز</th>
                        <th className="col-2">انرژی اکتیو خروجی (Wh)</th>
                        <th className="col-2">انرژی اکتیو ورودی (Wh)</th>
                        <th className="col-2">انرژی راکتیو خروجی (Varh)</th>
                        <th className="col-2">انرژی راکتیو ورودی (Varh)</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        items.map((d, k) => {
                            if (d.dailyEnergy) {
                                sumExportWatt += d.dailyEnergy.exportWatt;
                                sumImportWatt += d.dailyEnergy.importWatt;
                                sumExportVar += d.dailyEnergy.exportVar;
                                sumImportVar += d.dailyEnergy.importVar;

                                return (
                                    <tr className="d-flex" key={k}>
                                        <td className="col-4">{d.name}</td>
                                        <td className="col-2">{Number(d.dailyEnergy.exportWatt).toLocaleString('fa-IR')}</td>
                                        <td className="col-2">{Number(d.dailyEnergy.importWatt).toLocaleString('fa-IR')}</td>
                                        <td className="col-2">{Number(d.dailyEnergy.exportVar).toLocaleString('fa-IR')}</td>
                                        <td className="col-2">{Number(d.dailyEnergy.importVar).toLocaleString('fa-IR')}</td>
                                    </tr>
                                )
                            }
                            else
                                return null;
                        })
                    }
                </tbody>
                <tfoot>
                    <tr className="d-flex">
                        <th className="col-4">جمع کل</th>
                        <th className="col-2">{Number(sumExportWatt).toLocaleString('fa-IR')}</th>
                        <th className="col-2">{Number(sumImportWatt).toLocaleString('fa-IR')}</th>
                        <th className="col-2">{Number(sumExportVar).toLocaleString('fa-IR')}</th>
                        <th className="col-2">{Number(sumImportVar).toLocaleString('fa-IR')}</th>
                    </tr>
                    <tr className="d-flex">
                        <th className="col-4">تفاضل</th>
                        <th className="col-2"></th>
                        <th className="col-2">{Number(sumImportWatt - sumExportWatt).toLocaleString('fa-IR')}</th>
                        <th className="col-2"></th>
                        <th className="col-2">{Number(sumImportVar - sumExportVar).toLocaleString('fa-IR')}</th>
                    </tr>
                </tfoot>
            </table>
        );
    }

    renderEnergyBalance() {
        if (this.state.loading)
            return (<span className="k-icon k-i-loading"></span>);

        let data = this.state.data;
        if (data && data.length) {
            let content = data.map((d, i) => {
                let table = this.renderEnergyBalanceTable(d.equipments);
                return (
                    <div key={i}>
                        <h3>{d.busbarName}</h3>
                        {table}
                    </div>
                );
            })

            return (
                <ul className="animated fadeIn substationenergy ">
                    {content}
                </ul>
            );
        }
        else {
            return (
                <div className="animated fadeIn pt-3 text-center">داده ای موجود نیست</div>
            );
        }
    }

    render() {
        let energyBalance = this.renderEnergyBalance();
        return (
            <div className="animated fadeIn">
                {energyBalance}
            </div>
        );
    }
}

class SubstationEnergyBalance extends Component {
    constructor(props) {
        super(props);
        let yesterday = new Date(new Date().getTime() - 86400000);

        this.state = {
            loading: true,
            stationsAuto: [],
            model: {
                recordDate: yesterday,
                stationId: '',
            },
        }
    }

    componentDidMount = async () => {
        await this.getSubStations();
    }

    getSubStations = async () => {
        //const url = `${apiUrl}/api/Utility/GetSubstations`;
        const url = `${apiUrl}/Utility/GetSubstations`;

        try {
            let response = await fetch(url);
            if (response.ok) {
                let stationsAuto = await response.json();

                if(stationsAuto && stationsAuto.length){
                    let model = this.state.model;
                    model.stationId = stationsAuto[0].value;
                    this.setState({
                        loading: false,
                        stationsAuto: stationsAuto,
                        model,
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
        const { recordDate, stationId } = this.state.model;

        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>گزارش تراز انرژی ایستگاه</strong>
                        </CCardHeader>
                        <CCardBody>
                            <CRow>
                                <CCol md="6">
                                    <CRow className="mb-1">
                                        <CCol>
                                            <CFormLabel htmlFor="stationId" sm={4}>ایستگاه</CFormLabel>
                                        </CCol>
                                        <CCol>
                                            <Select
                                                disable={this.state.loading ? "ture" : "false"}
                                                name="stationId" id="stationId"
                                                options={this.state.stationsAuto}
                                                value={stationId}
                                                onChange={this.handleInputChange} />
                                        </CCol>
                                    </CRow>
                                </CCol>
                                <CCol md="6">
                                    <CRow className="mb-1">
                                        <CCol>
                                            <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                                        </CCol>
                                        <CCol>
                                            <DatePickerComponent
                                                name="recordDate"
                                                id="recordDate"
                                                calendarMode="Islamic"
                                                value={recordDate}
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
                            {stationId ? <SubstationEnergyBalanceComponent stationId={stationId} startDate={recordDate} /> : null}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default SubstationEnergyBalance;
