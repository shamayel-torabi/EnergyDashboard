import { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CButton
} from '@coreui/react';

import { withToast } from '../../../components';
import { apiUrl } from '../../../ApiConfig';

class MeterFailure extends Component {
    constructor(props) {
        super(props);

        this.state = {
            data: null,
            loading: true,
            message:''
        }
    }
    componentDidMount = async () => {
        await this.getMeterFailures();
    }

    getMeterFailures = async () => {
        //const url = `${apiUrl}/api/MeterReadingFailures/GetMeterFailures`;
        const url = `${apiUrl}/MeterReadingFailures/GetMeterFailures`;

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
                this.props.showToast("error","خطا در دریافت فهرست نواقص میتر.");
                console.log("خطا در دریافت فهرست نواقص میتر.");
                this.setState({
                    data: null,
                    loading: false,
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در دریافت فهرست نواقص میتر.");
            console.error(e);
        }
    }

    renderMeterFailuresTable(items) {
        let count = items.length;

        return (
            <table className="table table-sm table-bordered table-hover">
                <caption>تعداد رکورد: {count}</caption>
                <thead className="thead-dark">
                    <tr className="d-flex">
                        <th className="col-4 text-center">نام تجهیز</th>
                        <th className="col-4 text-center">تاریخ</th>
                        <th className="col-4 text-center">عملیات</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        items.map((d, k) =>
                            <tr className="d-flex" key={k}>
                                <td className="col-4">{d.equipmentName}</td>
                                <td className="col-4">{new Date(d.recordDate).toLocaleDateString('fa-IR')}</td>
                                <td className="col-4 text-left"><CButton size="sm" onClick={(e) => this.handleClick(d.equipmentId, d.recordDate)}>به روز رسانی</CButton></td>
                            </tr>
                        )
                    }
                </tbody>
            </table>
        );
    }

    renderMeterFailures() {
        if (this.state.loading)
            return (<span className="k-icon k-i-loading"></span>);

        let data = this.state.data;
        if (data && data.length) {
            let content = data.map((d, i) => {
                let table = this.renderMeterFailuresTable(d.items);
                return (
                    <div key={i}>
                        <h3>{d.substation}</h3>
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

    handleClick = async (equipmentId, date) => {
        console.log(equipmentId, date);

        //const url = `${apiUrl}/api/EnergyProfiles/UpdateEquipmentEnergyProfile/${equipmentId}/${date}`;
        const url = `${apiUrl}/EnergyProfiles/UpdateEquipmentEnergyProfile/${equipmentId}/${date}`;

        this.setState({
            loading: true,
        })

        try {
            let response = await fetch(url);
            if (response.ok) {
                let data = await response.json();
                console.log(data);
                this.setState({
                    message: data,
                    loading: false,
                });
            }
            else {
                this.props.showToast("error", "خطا در به روز رسانی نواقص میتر.");
                console.log("خطا در به روز رسانی نواقص میتر.");
                this.setState({
                    message: '',
                    loading: false,
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در به روز رسانی نواقص میتر.");
            console.error(e);
        }
    }

    render() {
        let failure = this.renderMeterFailures();
        return (
            <CRow className="animated fadeIn">
                <CCol>
                    <CCard>
                        <CCardHeader>
                            <strong>گزارش نواقص</strong>
                        </CCardHeader>
                        <CCardBody>
                            {failure}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default withToast(MeterFailure);
