import React, { Component } from 'react';
import {
    CModal, CModalHeader, CModalBody, CModalFooter, CModalTitle,
    CCol, CRow,
    CForm, CButton, CFormFeedback,
    CFormLabel, CFormCheck, CFormSelect
} from '@coreui/react'
import { DateTimePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { apiUrl } from '../../ApiConfig';


class AddMeter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            model: this.props.model,
            formInvalid: false,
            dateInvalid: false
        };
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

    handleSelectChange = (event) => {
        let model = this.state.model;
        let meters = this.props.meters;
        let formInvalid = false;

        const meterId = Number(event.target.value);
        let m = meters.find(x => x.meterId === meterId);

        model.meterId = meterId;
        model.serialNumber = m.serialNumber;
        model.name = m.name;
        model.mountDate = m.startOperationDate;
        model.dismountDate = m.endOperationDate;

        if (meterId === 0)
            formInvalid = true;

        this.setState({
            model: model,
            formInvalid: formInvalid,
        });
    }

    handleDateChange = (event) => {
        let model = this.state.model;
        const value = event.value;

        model[event.element.name] = value;

        this.setState({
            model: model
        });
    }

    handleCancel = (event) => {
        let mode = this.props.mode;
        this.props.onSave(mode, null);
    }

    handleSave = (event) => {
        const mode = this.props.mode;
        const model = this.state.model;

        const sdate = new Date(model.mountDate).getTime();
        const edate = new Date(model.dismountDate).getTime();

        if (model.meterId === 0) {
            this.setState({
                formInvalid: true
            })
        }
        else if (edate < sdate) {
            this.setState({
                dateInvalid: true
            })
        }
        else {
            if (model)
                this.props.onSave(mode, model)
            else
                this.props.onSave(mode, null);
        }
    }

    renderForm() {
        let model = this.state.model;
        let options = this.props.meters.map((v, k) => {
            return (<option key={k} value={v.meterId}>{`${v.serialNumber} - ${v.name}`}</option>);
        });
        return (
            <CForm id="addMeter">
                <CRow className="mb-1" >
                    <CFormLabel htmlFor="meterId" className="col-sm-5 col-form-label">سریال میتر</CFormLabel>
                    <CCol sm={7}>
                        <CFormSelect
                            name="meterId"
                            id="meterId"
                            defaultValue={model.meterId}
                            onChange={this.handleSelectChange}>
                            {options}
                        </CFormSelect>
                        {this.state.formInvalid ? <CFormFeedback>میتر باید انتخاب شود</CFormFeedback > : null}
                    </CCol>
                </CRow>
                <CRow row>
                    <CFormLabel htmlFor="mountDate" className="col-sm-5 col-form-label">تاریخ نصب</CFormLabel>
                    <CCol sm={7}>
                        <DateTimePickerComponent
                            name="mountDate"
                            id="mountDate"
                            calendarMode="Islamic"
                            value={model.mountDate}
                            change={this.handleDateChange}
                            format="yyyy-MM-dd HH:mm"
                            enableRtl={true}
                            firstDayOfWeek={6}
                            locale="fa">
                            <Inject services={[Islamic]} />
                        </DateTimePickerComponent>
                    </CCol>
                </CRow>
                <CRow row>
                    <CFormLabel htmlFor="dismountDate" className="col-sm-5 col-form-label">تاریخ جدا سازی</CFormLabel>
                    <CCol sm={7}>
                        <DateTimePickerComponent
                            name="dismountDate"
                            id="dismountDate"
                            calendarMode="Islamic"
                            value={model.dismountDate}
                            change={this.handleDateChange}
                            format="yyyy-MM-dd HH:mm"
                            enableRtl={true}
                            firstDayOfWeek={6}
                            locale="fa">
                            <Inject services={[Islamic]} />
                        </DateTimePickerComponent>
                        {this.state.dateInvalid ? <CFormFeedback color='danger'>تاریخ نصب باید کوچکتر از تاریخ جداسازی باشد.</CFormFeedback > : null}
                    </CCol>
                </CRow>
                <CRow row >
                    <CFormLabel htmlFor="active" className="col-sm-5 col-form-label">وضعیت</CFormLabel>
                    <CCol sm={7}>
                        <CFormCheck
                            name="active"
                            id="active"
                            defaultChecked={model.active}
                            onChange={this.handleInputChange} />
                    </CCol>
                </CRow>
            </CForm>
        )
    }

    render() {
        let form = this.renderForm();

        return (
            <CModal visible={true} alignment="center" className={this.props.className}>
                <CModalHeader closeButton={false}>
                    <CModalTitle>انتخاب میتر</CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <CRow>
                        <CCol md={12}>
                            {form}
                        </CCol>
                    </CRow>
                </CModalBody>
                <CModalFooter>
                    <CButton color="primary" type="button" onClick={this.handleSave}>ذخیره</CButton>
                    <CButton color="secondary" type="button" onClick={this.handleCancel}>انصراف</CButton>
                </CModalFooter>
            </CModal>
        )
    }
}

export class MeterForm extends Component {
    constructor(props) {
        super(props);

        this.state = {
            items: props.items,
            showModal: false,
            mode: 'Add',
            selectedMeter: null,
            meters: [],
        }
    }

    componentDidMount = async () => {
        await this.getSubstationMeters(this.props.substationId);
    }

    getSubstationMeters = async (substationId) => {
        const url = `${apiUrl}/Utility/GetSubstationMeters/${substationId}`;

        try {
            let response = await fetch(url);
            if (response.ok) {
                let meters = await response.json();
                this.setState({
                    meters: meters,
                })
            }
            else {
                console.error("خطا در دریافت میتر.");
            }
        }
        catch (e) {
            console.error("خطا در اتصال به سرور دریافت میتر.");
        }
    }

    addMeter = (e) => {
        let d = new Date();
        let mdate = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
        let sdate = new Date(d.getFullYear() + 30, d.getMonth(), d.getDate(), 0, 0, 0, 0);

        let model = {
            meterId: 0,
            serialNumber: 'سریال',
            name: 'نامعلوم',
            mountDate: mdate,
            dismountDate: sdate,
            active: true
        };

        this.setState({
            showModal: true,
            mode: 'Add',
            selectedMeter: model,
        })
    }

    editMeter = (meterId) => {
        let meter = this.state.items.find(element => element.meterId === meterId);

        this.setState({
            showModal: true,
            mode: 'Edit',
            selectedMeter: meter,
        });
    }

    deleteMeter = (meterId) => {
        let items = this.state.items;
        let found = items.find(element => element.meterId === meterId);

        const index = items.indexOf(found);

        if (index !== -1) {
            items.splice(index, 1);
        }

        this.setState({
            items: items
        });
    }

    saveMeter = (mode, meter) => {
        if (meter) {
            switch (mode) {
                case 'Add':
                    this.addNewMeter(meter);
                    break;
                case 'Edit':
                    this.editNewMeter(meter);
                    break;
                default:
                    break;
            }
        }
        else {
            this.setState({
                showModal: false,
            });
        }
    }

    addNewMeter = (meter) => {
        let items = this.state.items;
        const meterMountdate = new Date(meter.mountDate).getTime();

        for (let index in items) {
            let itemMountdate = new Date(items[index].mountDate).getTime();

            if (meter.serialNumber === items[index].serialNumber && meterMountdate === itemMountdate) {
                console.log('find Duplicate Meter:', items[index]);
                items.splice(index, 1);
            }
            else {
                items[index].active = false;
            }
        }

        items.push(meter);

        this.setState({
            showModal: false,
            items: items
        });
    }

    editNewMeter = (meter) => {
        let items = this.state.items;

        if (meter.active) {
            for (let item of items) {
                if (item.meterId === meter.meterId)
                    continue;

                item.active = false;
            }
        }

        this.setState({
            showModal: false,
            items: items
        });
    }

    renderAddMeter = () => {
        let { showModal, meters, selectedMeter, mode } = this.state;

        if (showModal) {
            return (
                <AddMeter
                    onSave={this.saveMeter}
                    meters={meters}
                    model={selectedMeter}
                    mode={mode} />
            );
        }
        else
            return null;
    }

    renderItems = () => {
        let dateOption = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
        let items = this.props.items;
        let rows;
        if (items && items.length > 0) {
            rows = this.props.items.map((v, k) => {
                return (
                    <tr key={k}>
                        <td style={{ width: "10%" }}>{v.meterId}</td>
                        <td style={{ width: "10%" }}>{v.serialNumber}</td>
                        <td style={{ width: "15%" }}>{v.name}</td>
                        <td style={{ width: "20%" }}>{new Date(v.mountDate).toLocaleString('fa-IR', dateOption)}</td>
                        <td style={{ width: "20%" }}>{new Date(v.dismountDate).toLocaleString('fa-IR', dateOption)}</td>
                        <td className="text-center" style={{ width: "5%" }}><CFormCheck checked={v.active} disabled /></td>
                        <td className="text-center" style={{ width: "20%" }}>
                            <CButton size="sm" type="button" onClick={e => this.deleteMeter(v.meterId)}>حذف</CButton>{' '}
                            <CButton size="sm" type="button" onClick={e => this.editMeter(v.meterId)}>ویرایش</CButton>
                        </td>
                    </tr>
                )
            })
        }
        else
            rows = null;


        return (
            <table className="table table-sm table-hover table-bordered">
                <thead className="thead-dark">
                    <tr>
                        <td className="text-center" style={{ width: "10%" }}>شناسه</td>
                        <td className="text-center" style={{ width: "10%" }}>سریال</td>
                        <td className="text-center" style={{ width: "15%" }}>تجهیز</td>
                        <td className="text-center" style={{ width: "20%" }}>تاریخ نصب</td>
                        <td className="text-center" style={{ width: "20%" }}>تاریخ جداسازی</td>
                        <td className="text-center" style={{ width: "5%" }}>وضعیت</td>
                        <td className="text-center" style={{ width: "20%" }}>عملیات</td>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
        )
    }

    render() {
        return (
            <div>
                <label>{this.props.title}</label>
                <CButton type="button" onClick={this.addMeter} className="float-end">افزودن</CButton>
                <hr />
                <div className="table-responsive" style={{ height: '250px', overflowY: 'auto' }}>
                    {this.renderItems()}
                </div>
                {this.renderAddMeter()}
            </div>
        )
    }
}
