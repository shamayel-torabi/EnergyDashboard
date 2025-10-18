import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormLabel
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { withToast } from '../../../components';
import { Select } from '../../../components';
import { apiUrl } from '../../../ApiConfig';


const ReadStationEnergy = withToast(class extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            loading: false,
        }
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if ((nextProps.startDate !== this.props.startDate) || (nextProps.stationId !== this.props.stationId)) {
            let date = nextProps.startDate;
            let substationId = nextProps.stationId;
            await this.fetchStationEnergy(substationId, date);
            return true;
        }
        return false;
    }

    fetchStationEnergy = async (substationId, date) => {
        const url = `${apiUrl}/DailyEnergy/GetSubstationEnergy/${substationId}/${date.toJSON()}`;

        this.setState({
            loading: true,
        })

        try {
            let response = await fetch(url);
            if (response.ok) {
                let data = await response.json();
                //console.log(data);
                this.setState({
                    data: data,
                    loading: false,
                });
            }
            else {
                this.props.showToast("error", "خطا در دریافت انرژی روزانه ایستگاه.");
                console.error("errr");
                this.setState({
                    data: null,
                    loading: false,
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در دریافت انرژی روزانه ایستگاه.");
            this.setState({
                data: null,
                loading: false,
            });
            console.error(e);
        }
    }

    renderSubstationEnergyTable(items) {
        let count = items.length;
        let sumExportWatt = 0, sumImportWatt = 0, sumExportVar = 0, sumImportVar = 0;

        return (
            <table className="table table-sm table-bordered table-hover">
                <thead className="thead-dark">
                    <tr className="d-flex">
                        <th className="col-4">تجهیز</th>
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
                                    <td className="col-4">{d.equipmentName}</td>
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
                        <th className="col-2">{Number(sumExportWatt).toLocaleString('fa-IR')}</th>
                        <th className="col-2">{Number(sumImportWatt).toLocaleString('fa-IR')}</th>
                        <th className="col-2">{Number(sumExportVar).toLocaleString('fa-IR')}</th>
                        <th className="col-2">{Number(sumImportVar).toLocaleString('fa-IR')}</th>
                    </tr>
                </tfoot>
                <caption><span>انرژی اکتیو خالص: {Number(sumImportWatt - sumExportWatt).toLocaleString('fa-IR')} </span></caption>
            </table>
            
        );
    }

    render() {
        if (this.state.loading)
            return (<span className="k-icon k-i-loading"></span>);

        let data = this.state.data;
        if (data && data.length) {
            let content = this.renderSubstationEnergyTable(data);

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
});

class SubstationEnergyDaily extends Component {
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
        const url = `${apiUrl}/Utility/GetSubstations`

        try {
            let response = await fetch(url);
            if (response.ok) {
                let st = await response.json();
                if(st && st.length){
                    let model = this.state.model;
                    model.stationId = st[0].value;
                    this.setState({
                        model,
                        stationsAuto: st
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
        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>گزارش روزانه انرژی ایستگاه</strong>
                        </CCardHeader>
                        <CCardBody>
                            <CForm className='needs-validation' onSubmit={this.handleSubmit} noValidate id="meterForm">
                                <CRow>
                                    <CCol>
                                        <CRow>
                                            <CCol>
                                                <CFormLabel htmlFor="stationId">ایستگاه</CFormLabel>
                                            </CCol>
                                            <CCol sm={8}>
                                                <Select required
                                                    name="stationId" id="stationId"
                                                    options={this.state.stationsAuto}
                                                    value={this.state.model.stationId}
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
                                                    value={this.state.model.recordDate}
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
                            </CForm>
                            <hr />
                            <ReadStationEnergy startDate={this.state.model.recordDate} stationId={this.state.model.stationId} />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default SubstationEnergyDaily;
