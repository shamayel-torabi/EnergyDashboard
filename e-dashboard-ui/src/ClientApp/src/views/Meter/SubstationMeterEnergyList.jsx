import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel, CFormCheck, CSpinner
} from '@coreui/react';
import { EquipmentEnergyChart } from './EquipmentEnergyChart';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { Select } from '../../components/Select';
import { apiUrl } from '../../ApiConfig';

class EnergyListTable extends Component {
    constructor(props) {
        super(props);

        this.state = {
            loading: false,
            substationsEnergy: [],
        }
    }

    componentDidMount = async () => {
        let date = this.props.date;
        let stationId = this.props.stationId

        await this.getStationEnergyByDate(stationId, date);
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if (nextProps.date !== this.props.date || nextProps.stationId !== this.props.stationId) {
            let date = nextProps.date;
            let stationId = nextProps.stationId

            await this.getStationEnergyByDate(stationId, date);
            return true;
        }
        return false;
    }

    getStationEnergyByDate = async (stationId, date) => {
        const url = `${apiUrl}/MeterEnergys/GetSubstationEnergy/${stationId}/${date.toJSON()}`;


        this.setState({
            loading: true,
            substationsEnergy: []
        })

        try {
            let response = await fetch(url);
            if (response.ok) {
                let result = await response.json();

                this.setState({
                    loading: false,
                    substationsEnergy: result,
                })
            }
            else {
                const err = await response.json();
                console.error(err);
                this.setState({
                    loading: false,
                    substationsEnergy: [],
                })
            }
        }
        catch (e) {
            console.error(e);
            this.setState({
                loading: false,
                substationsEnergy: [],
            })
        }
    }

    renderSubstationsEnergyTableRows() {
        let rows;
        if (this.state.loading) {
            rows = (
                <tr>
                    <td colSpan="6" className="text-center">
                        <CSpinner style={{ width: '4rem', height: '4rem' }} color="danger" variant="grow" />
                    </td>
                </tr>
            );
        }
        else {
            const substationsEnergy = this.state.substationsEnergy;

            if (substationsEnergy && substationsEnergy.length > 0) {
                rows = substationsEnergy.map((sub, k) => {

                    let eqRow;
                    if (sub.equipments && sub.equipments.length) {

                        eqRow = sub.equipments.map((eq, i) => {
                            let d = eq.dailyEnergy.map((e, i) => {
                                return { x: e.hour, y: e.activeEnergy }
                            })

                            return (
                                <tr key={i}>
                                    <td style={{ width: "15%", verticalAlign: "middle" }}>{eq.name}</td>
                                    <td style={{ width: "10%", verticalAlign: "middle" }}>{eq.serialNumber}</td>
                                    <td style={{ width: "60%", verticalAlign: "middle" }}><EquipmentEnergyChart data={d} /></td>
                                    <td className="text-center" style={{ width: "5%", verticalAlign: "middle" }}><CFormCheck disable defaultChecked={eq.anomal} /></td>
                                </tr>
                            );
                        })
                    }

                    return eqRow;
                })
            }
        }

        return rows
    }

    render() {
        const rows = this.renderSubstationsEnergyTableRows();

        return (
            <React.Fragment>
                <div className="table-responsive">
                    <table className="table table-sm table-hover table-bordered">
                        <thead className="thead-dark">
                            <tr>
                                <th className="text-center" style={{ width: "15%" }}>نام تجهیز</th>
                                <th className="text-center" style={{ width: "10%" }}>شماره سریال</th>
                                <th className="text-center" style={{ width: "60%" }}>شکل</th>
                                <th className="text-center" style={{ width: "5%" }}>ناهنجار</th>
                            </tr>
                        </thead>
                        <tbody>
                            {rows}
                        </tbody>
                    </table>
                </div>
            </React.Fragment>
        )
    }
}

class SubstationMeterEnergyList extends Component {
    constructor(props) {
        super(props);
        let yesterday = new Date(Date.now() - 86400000);

        this.state = {
            date: yesterday,
            stationId: 1,
            loading: true,
            substations: [],
        }
    }

    componentDidMount = async () => {
        await this.getSubstations();
    }

    getSubstations = async () => {
        //const url = `${apiUrl}/api/Meters/GetSubstations`;
        const url = `${apiUrl}/Meters/GetSubstations`;

        this.setState({
            loading: true,
            substations: [],
        })

        try {
            let response = await fetch(url);
            if (response.ok) {
                let result = await response.json();

                this.setState({
                    loading: false,
                    stationId: result[0].value,
                    substations: result,
                })
            }
            else {
                console.error("خطا در دریافت فهرست ایستگاهها.");
                this.setState({
                    loading: false,
                    substations: [],
                })
            }
        }
        catch (e) {
            console.error("خطا در دریافت فهرست ایستگاهها.");
            this.setState({
                loading: false,
                substations: [],
            })
        }
    }

    handleDateChange = (event) => {
        const value = event.value || new Date();

        this.setState({
            date: value
        });
    }

    handleInputChange = (event) => {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        this.setState({
            stationId: value
        });
    }


    render() {
        return (
            <CCard className="animated fadeIn" style={{ marginBottom: '0.1rem' }}>
                <CCardHeader>
                    <strong>فهرست انرژی میترهای ایستگاه</strong>
                </CCardHeader>
                <CCardBody>
                    <CRow className="mb-1">
                        <CCol xl="4" lg="5" md="5">
                            <CRow className="g-3 align-items-center">
                                <CCol xs="auto">
                                    <CFormLabel htmlFor="stationId">ایستگاه</CFormLabel>
                                </CCol>
                                <CCol xs="auto">
                                    <Select required
                                        name="stationId" id="stationId"
                                        value={this.state.stationId}
                                        options={this.state.substations}
                                        onChange={this.handleInputChange} />
                                </CCol>
                            </CRow>
                        </CCol>
                        <CCol xl="4" lg="2" md="2"></CCol>
                        <CCol xl="4" lg="5" md="5">
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
                                        value={this.state.date}
                                        change={this.handleDateChange}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>
                        </CCol>
                    </CRow>
                    <hr />
                    <CRow>
                        <CCol>
                            <EnergyListTable date={this.state.date} stationId={this.state.stationId} />
                        </CCol>
                    </CRow>
                </CCardBody>
            </CCard>
        )
    }
}

export default SubstationMeterEnergyList;