import React, { Component } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormLabel,
} from '@coreui/react';
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { Select, withToast , ToggleContent} from '../../../components';
import { apiUrl } from '../../../ApiConfig';

const ReadPowerPlantEnergy = withToast(class extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null,
            loading: true,
        }
    }

    shouldComponentUpdate = async (nextProps, nextState) => {
        if ((nextProps.recordDate !== this.props.recordDate) || (nextProps.powerPlantId !== this.props.powerPlantId)) {
            let date = nextProps.recordDate;
            let powerPlantId = nextProps.powerPlantId;
            await this.getPowerplantEnergy(powerPlantId, date);
            return true;
        }
        return false;
    }

    getPowerplantEnergy = async (powerPlantId, date) => {
        //const url = `${apiUrl}/api/EnergyProfiles/GetPowerPlantEnergy/${powerPlantId}/${date.toJSON()}`;
        const url = `${apiUrl}/EnergyProfiles/GetPowerPlantEnergy/${powerPlantId}/${date.toJSON()}`;

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
                this.props.showToast("error", "خطا در دریافت انرژی نیروگاه.");
                console.error("خطا در دریافت انرژی نیروگاه.");
                this.setState({
                    data: null,
                    loading: false,
                });
            }
        }
        catch (e) {
            this.props.showToast("error", "خطا در اتصال به سرور.");
            console.error(e);
            this.setState({
                data: null,
                loading: false,
            });
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
                            )}
                        )
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

class EnergyTable extends Component {
    constructor(props) {
        super(props);

        let yesterday = new Date(new Date().getTime() - 86400000);

        this.state = {
            powerPlants: [],
            model: {
                recordDate: yesterday,
                powerPlantId: '',
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
                if (ppOperator && ppOperator.length){
    
                    let model = this.state.model;
                    model.powerPlantId = ppOperator[0].value;
                    this.setState({
                        model,
                        powerPlants: ppOperator
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
        const value = event.value || new Date();
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
                            <strong>گزارش نیروگاه</strong>
                        </CCardHeader>
                        <CCardBody>
                            <CForm className='needs-validation' onSubmit={this.handleSubmit} noValidate id="meterForm">
                                <CRow>
                                    <CCol>
                                        <CRow>
                                            <CCol>
                                                <CFormLabel htmlFor="powerPlantId">نیروگاه</CFormLabel>
                                            </CCol>
                                            <CCol sm={8}>
                                                <Select required
                                                    name="powerPlantId" id="powerPlantId"
                                                    options={this.state.powerPlants}
                                                    value={this.state.model.powerPlantId}
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
                            <ReadPowerPlantEnergy recordDate={this.state.model.recordDate} powerPlantId={this.state.model.powerPlantId} />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        );
    }
}

export default EnergyTable;
