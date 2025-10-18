import React, { Component } from 'react';
import PropTypes from 'prop-types';
import {
    CModal, CModalHeader, CModalBody, CModalFooter, CModalTitle,
    CNav, CNavItem, CNavLink, CTabContent, CTabPane,
    CCol, CRow,
    CForm, CButton, CFormInput, CFormTextarea,
    CFormLabel, CFormCheck, CFormSelect, CFormFeedback
} from '@coreui/react'

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { MeterForm } from './MeterForm';
import { ColorPicker } from '../../components';

export class PropertyDialog extends Component {
    constructor(props) {
        super(props);
        this.state = {
            model: this.props.property,
            validated: false,
            activeTab: props.schema.tabs[0].tabIndex
        }
    }

    static defaultProps = {
        isOpen: false,
        onSave: (e) => { console.log(e) },
    }

    static propTypes = {
        isOpen: PropTypes.bool,
        property: PropTypes.object,
        schema: PropTypes.object,
        onSave: PropTypes.func.isRequired,
        substationId: PropTypes.number,
    }


    toggle = (tab) => {
        if (this.state.activeTab !== tab) {
            this.setState({
                activeTab: tab,
            })
        }
    }

    handleCancel = (event) => {
        this.props.onSave(null);
    }

    handleSave = (event) => {
        event.preventDefault();
        let form = event.target;

        if (form.checkValidity() === false) {
            event.stopPropagation();
            this.setState({
                validated: true
            })
        }
        else {
            if (this.state.model)
                this.props.onSave(this.state.model)
            else
                this.props.onSave(null);
        }
    }

    handleInputChange = (event) => {
        let model = this.state.model;
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;

        model[event.target.name] = value;

        this.setState({
            model
        });
    }

    handleNumberChange = (event) => {
        let model = this.state.model;
        const value = Number(event.target.value);

        model[event.target.name] = value;

        this.setState({
            model: model
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

    handleColorChange = (event) => {
        let model = this.state.model;

        model[event.name] = event.value;

        this.setState({
            model: model
        });
    }


    renderForm() {
        let schema = this.props.schema;
        if (schema === null)
            return null;

        let tabs = schema.tabs;

        let navTabs = tabs.map((tab, key) => {
            return (
                <CNavItem key={key}>
                    <CNavLink
                        active={this.state.activeTab === key + 1}
                        onClick={() => { this.toggle(tab.tabIndex) }}>
                        {tab.tabName}
                    </CNavLink>
                </CNavItem>
            )
        });


        let tabContents = tabs.map((tab, key) => {
            let properties = Object.keys(tab.properties);

            let feilds = properties.map((val, key) => {
                let field = tab.properties[val];
                let title = field.title ? field.title : val;

                switch (field.type) {
                    case "string":
                        if (field.format === 'textarea') {
                            return (
                                <CRow key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <CFormTextarea
                                            name={val}
                                            id={val}
                                            value={this.state.model[val]}
                                            onChange={this.handleInputChange}
                                            required={field.required}
                                            disabled={field.disabled} />
                                        <CFormFeedback invalid>{`${title} باید وارد شود`}</CFormFeedback>
                                    </CCol>
                                </CRow>
                            )
                        }
                        else if (field.format === 'date') {
                            return (
                                <CRow key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <DatePickerComponent
                                            name={val}
                                            id={val}
                                            calendarMode="Islamic"
                                            value={this.state.model[val]}
                                            format="yyyy-MM-dd"
                                            enableRtl={true}
                                            firstDayOfWeek={6}
                                            change={this.handleDateChange}
                                            locale="fa"
                                            disabled={field.disabled}>
                                            <Inject services={[Islamic]} />
                                        </DatePickerComponent>
                                    </CCol>
                                </CRow>
                            )
                        }
                        else if (field.format === 'vcolor') {
                            return (
                                <CRow key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <ColorPicker
                                            name={val}
                                            id={val}
                                            value={this.state.model[val]}
                                            onChange={this.handleColorChange} />
                                    </CCol>
                                </CRow>
                            )
                        }
                        else if (field.format === 'color') {
                            return (
                                <CRow key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <CFormInput
                                            type="color"
                                            name={val}
                                            id={val}
                                            defaultValue={this.state.model[val]}
                                            onChange={this.handleInputChange} />
                                        <CFormFeedback invalid>{`${title} باید وارد شود`}</CFormFeedback>
                                    </CCol>
                                </CRow>
                            )
                        }

                        else if (field.enum) {
                            let opt = field.enum.map((v, k) => <option key={k} value={v.value}>{v.label}</option>);
                            return (
                                <CRow row key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <CFormSelect
                                            name={val}
                                            id={val}
                                            value={this.state.model[val]}
                                            onChange={this.handleInputChange}
                                            pattern={field.pattern}
                                            required={field.required}
                                            disabled={field.disabled}>
                                            {opt}
                                        </CFormSelect>
                                        <CFormFeedback invalid>{`${title} باید انتخاب شود`}</CFormFeedback>
                                    </CCol>
                                </CRow>
                            )
                        }
                        else if (field.group) {
                            let opt = field.group.map((g, k) => {
                                let o = g.items.map((item) => {
                                    let val = g.value + '-' + item.value;
                                    return (<option key={item.value} value={val}>{item.title}</option>);
                                })

                                return <optgroup key={k} label={g.title}>{o}</optgroup>
                            });
                            return (
                                <CRow row key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <CFormSelect
                                            name={val}
                                            id={val}
                                            value={this.state.model[val]}
                                            onChange={this.handleInputChange}
                                            pattern={field.pattern}
                                            required={field.required}
                                            disabled={field.disabled}>
                                            {opt}
                                        </CFormSelect>
                                        <CFormFeedback invalid>{`${title} باید انتخاب شود`}</CFormFeedback>
                                    </CCol>
                                </CRow>
                            )
                        }
                        else {
                            return (
                                <CRow row key={key} className="mb-1">
                                    <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                    <CCol sm={7}>
                                        <CFormInput
                                            name={val}
                                            id={val}
                                            value={this.state.model[val]}
                                            onChange={this.handleInputChange}
                                            pattern={field.pattern}
                                            required={field.required}
                                            disabled={field.disabled} />
                                        <CFormFeedback invalid>{`${title} باید وارد شود`}</CFormFeedback>
                                    </CCol>
                                </CRow>
                            )
                        }
                        break;
                    case "integer":
                        return (
                            <CRow row key={key} className="mb-1">
                                <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                <CCol sm={7}>
                                    <CFormInput
                                        type="number"
                                        name={val}
                                        id={val}
                                        min={field.min}
                                        max={field.max}
                                        value={this.state.model[val]}
                                        onChange={this.handleInputChange}
                                        required={field.required}
                                        disabled={field.disabled} />
                                    <CFormFeedback invalid>{`${title} باید وارد شود`}</CFormFeedback>
                                </CCol>
                            </CRow>
                        )
                        break;
                    case "number":
                        return (
                            <CRow row key={key} className="mb-1">
                                <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                <CCol sm={7}>
                                    <CFormInput
                                        name={val}
                                        id={val}
                                        value={this.state.model[val]}
                                        onChange={this.handleNumberChange}
                                        pattern={field.pattern}
                                        required={field.required}
                                        disabled={field.disabled} />
                                    <CFormFeedback invalid>{`${title} باید وارد شود`}</CFormFeedback>
                                </CCol>
                            </CRow>
                        )
                        break;
                    case "boolean":
                        return (
                            <CRow row key={key} className="mb-1">
                                <CFormLabel htmlFor={val} className="col-sm-5 col-form-label">{title}</CFormLabel>
                                <CCol sm={7}>
                                    <CFormCheck
                                        name={val}
                                        id={val}
                                        label={title}
                                        defaultChecked={this.state.model[val]}
                                        onChange={this.handleInputChange}
                                        disabled={field.disabled} />
                                    <CFormFeedback invalid>{`${title} باید انتخاب شود`}</CFormFeedback>
                                </CCol>
                            </CRow>
                        )
                        break;
                    case "meterform":
                        return (
                            <MeterForm className="mb-1"
                                key={key}
                                title={field.title}
                                name={val}
                                items={this.state.model[val]}
                                substationId={this.props.substationId}>
                            </MeterForm>
                        )
                        break;
                    case "object":
                        break;
                    default:
                        throw new Error("Not Implemented");
                }
                return null;
            });

            return (
                <CTabPane key={key} visible={this.state.activeTab === key + 1} className="mt-2" >
                    {feilds}
                </CTabPane>
            )
        })


        return (
            <CForm className='needs-validation' onSubmit={this.handleSave} noValidate validated={this.state.validated} id="propertyForm">
                <CNav variant="tabs" role="tablist">
                    {navTabs}
                </CNav >
                <CTabContent style={{ height: 320 }}>
                    {tabContents}
                </CTabContent>
            </CForm>
        );
    }

    render() {
        let form = this.renderForm();

        return (
            <CModal visible={this.props.isOpen} size="lg" alignment="center" className={this.props.className}>
                <CModalHeader closeButton={false}>
                    <CModalTitle>{this.props.schema.title}</CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <CRow>
                        <CCol md={12}>
                            {form}
                        </CCol>
                    </CRow>
                </CModalBody>
                <CModalFooter>
                    <CButton color="primary" type="submit" form="propertyForm">ذخیره</CButton>
                    <CButton color="secondary" type="button" onClick={this.handleCancel}>انصراف</CButton>
                </CModalFooter>
            </CModal>
        );
    }
}

