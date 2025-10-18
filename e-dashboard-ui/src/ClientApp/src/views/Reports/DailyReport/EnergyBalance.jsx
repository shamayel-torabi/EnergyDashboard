import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormLabel, 
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { apiUrl } from '../../../ApiConfig';

class EnergyBalanceComponent extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: true,
            data: null,
        }
    }

    componentDidMount = async () => {
        let date = this.props.startDate;
        await this.getEnergyBalance(date);
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if ((nextProps.startDate !== this.props.startDate)) {
            let date = nextProps.startDate;
            await this.getEnergyBalance(date);
            return true;
        }
        return false;
    }

    getEnergyBalance = async (date) => {
        //const url = `${apiUrl}/api/DailyEnergy/EnergyBalance/${date.toJSON()}`;
        const url = `${apiUrl}/DailyEnergy/EnergyBalance/${date.toJSON()}`;

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
                    data: null,
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

        return (
            <table className="table table-sm table-bordered table-hover">
                <caption>تعداد باسبار: {count}</caption>
                <thead className="thead-dark">
                    <tr className="d-flex text-center">
                        <th className="col-6">نام باسبار</th>
                        <th className="col-6">مانده انرژی (KWh)</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        items.map((d, k) =>
                            <tr className="d-flex" key={k}>
                                <td className="col-6">{d.busBarName}</td>
                                <td className="col-6">{Number(d.activeEnergy/1000).toLocaleString('fa-IR')}</td>
                            </tr>
                        )
                    }
                </tbody>
            </table>
        );
    }

    renderEnergyBalance() {
        if (this.state.loading)
            return (<span className="k-icon k-i-loading"></span>);

        let data = this.state.data;
        if (data && data.length) {
            let content = data.map((d, i) => {
                let table = this.renderEnergyBalanceTable(d.busBars);
                return (
                    <div key={i}>
                        <h3>{d.substationName}</h3>
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

class EnergyBalance extends Component {
    constructor(props) {
        super(props);
        let yesterday = new Date(new Date().getTime() - 86400000);

        this.state = {
            loading: true,
            recordDate: yesterday,
        }
    }

    handleDateChange = (event) => {
        const value = event.value || new Date();
        let recordDate = this.state.recordDate;

        recordDate = value;

        this.setState({
            recordDate
        });
    }


    render() {
        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>گزارش تراز انرژی</strong>
                        </CCardHeader>
                        <CCardBody>
                            <CForm className='needs-validation' onSubmit={this.handleSubmit} noValidate id="meterForm">
                                <CRow>
                                    <CCol xs="auto">
                                        <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                                    </CCol>
                                    <CCol xs="auto">
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
                            </CForm>
                            <hr />
                            <EnergyBalanceComponent startDate={this.state.recordDate}></EnergyBalanceComponent>
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default EnergyBalance;
