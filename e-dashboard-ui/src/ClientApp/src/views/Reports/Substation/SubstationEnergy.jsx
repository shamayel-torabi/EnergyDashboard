import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { withToast } from '../../../components';
import { ToggleContent } from '../../../components/ToggleContent';
import { apiUrl } from '../../../ApiConfig';

const ReadStationEnergy = withToast(class extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            loading: true,
        }
    }

    componentDidMount = async () => {
        let date = this.props.startDate;
        let substationId = this.props.stationId;
        await this.fetchStationEnergy(substationId, date);
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if (nextProps.startDate !== this.props.startDate) {
            let date = nextProps.startDate;
            let substationId = nextProps.stationId;
            await this.fetchStationEnergy(substationId, date);
            return true;
        }
        return false;
    }

    fetchStationEnergy = async (substationId,date) => {
        //const url = `${apiUrl}/api/EnergyProfiles/GetSubstationEnergy/${substationId}/${date.toJSON()}`;
        const url = `${apiUrl}/EnergyProfiles/GetSubstationEnergy/${substationId}/${date.toJSON()}`;

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
                this.props.showToast("error", "خطا در دریافت انرژی ایستگاه.");
                console.error("errr");
                this.setState({
                    data: null,
                    loading: false,
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در دریافت انرژی ایستگاه.");
            console.error(e);
        }
    }

    renderSubstationEnergyTable(items) {
        let count = items.length;
        let sumExportWatt = 0, sumImportWatt = 0, sumExportVar = 0, sumImportVar = 0;
        return (
            <table className="table table-sm table-bordered table-hover">
                <caption>تعداد رکورد: {count}</caption>
                <thead className="thead-dark">
                    <tr className="d-flex text-center">
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
                                    <td className="col-2">{d.exportWatt}</td>
                                    <td className="col-2">{d.importWatt}</td>
                                    <td className="col-2">{d.exportVar}</td>
                                    <td className="col-2">{d.importVar}</td>
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

class SubstationEnergy extends Component {
    constructor(props) {
        super(props);

        let yesterday = new Date(new Date().getTime() - 86400000);

        this.state = {
            substationId: this.props.match.params.substationId,
            substationName:'',
            recordDate: yesterday,
        }
    }

    componentDidMount = async () => {
        let substationId = this.state.substationId;
        //const url = `${apiUrl}/api/Substations/GetSubstationName/${substationId}`;
        const url = `${apiUrl}/Substations/GetSubstationName/${substationId}`;

        try {
            let response = await fetch(url);
            if (response.ok) {
                let data = await response.text();
                this.setState({
                    substationName:data
                });
            }
            else {
                this.props.showToast("error", "خطا در دریافت نام ایستگاه.");
                console.error("خطا در دریافت نام ایستگاه.");
                this.setState({
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در دریافت نام ایستگاه.");
            console.error(e);
        }
    }

    handleDateChange = (event) => {
        let value = event.value || new Date();

        this.setState({
            recordDate: value
        });
    }

    render() {
        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>گزارش ساعتی انرژی ایستگاه {this.state.substationName}</strong>
                        </CCardHeader>
                        <CCardBody>
                            <CRow>
                                <CRow>
                                    <CCol>
                                        <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                                    </CCol>
                                    <CCol>
                                        <DatePickerComponent
                                            name="recordDate"
                                            id="recordDate"
                                            calendarMode="Islamic"
                                            value={this.state.recordDate}
                                            format="dd MMM yyyy"
                                            enableRtl={true}
                                            firstDayOfWeek={6}
                                            change={this.handleDateChange}
                                            locale="fa">
                                            <Inject services={[Islamic]} />
                                        </DatePickerComponent>
                                    </CCol>
                                </CRow>
                            </CRow>
                            <hr />
                            <ReadStationEnergy startDate={this.state.recordDate} stationId={this.state.substationId} />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default withToast(SubstationEnergy);
