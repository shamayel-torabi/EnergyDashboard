import { useState } from 'react';
import PropTypes from 'prop-types'
import { useNavigate } from "react-router-dom";
import { v4 as uuidv4, v5 as uuidv5 } from 'uuid';

import {
    CCard, CCardHeader, CCardBody,
    CModal, CModalHeader, CModalBody, CModalFooter, CModalTitle,
    CCol, CRow,
    CForm, CButton, CFormInput, CFormLabel
} from '@coreui/react'
import { DatePickerComponent, TimePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { CRUDService } from '../../services';
import { useToast } from '../../components';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { apiUrl } from '../../ApiConfig';

const MY_NAMESPACE = 'b189f273-176f-4860-9fe5-23311a5d07c9';

const EnergyTariffRateForm = ({ onSave, energyTariffId }) => {
    const today = new Date();
    const now = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0, 0, 0).getTime();
    const toast = useToast();

    const [open, setOpen] = useState(true);
    const [model, setModel] = useState({
        id: '00000000-0000-0000-0000-000000000000',
        energyTariffId: energyTariffId,
        startTime: new Date(now),
        endTime: new Date(now + 3600000),
        rate: 1.0,
    });

    const handleInputChange = (event) => {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        const name = event.target.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const handleDateChange = (event) => {
        const value = event.value || new Date();
        const name = event.element.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const handleCancel = () => {
        setOpen(!open)
        onSave(null);
    }

    const handleSave = (event) => {

        if (model.startTime.getTime() >= model.endTime.getTime()) {
            toast.showToast("warn", "زمان شروع باید کوچکتر از زمان پایان باشد")
            return;
        }

        if (model.rate < 0) {
            toast.showToast("warn", "نرخ باید عدد بزرگتر از صفر باشد")
            return;
        }

        model.startTime = toTimespan(model.startTime);
        model.endTime = toTimespan(model.endTime);

        const s = `${model.startTime}-${model.endTime}`;
        model.id = uuidv5(s, MY_NAMESPACE);

        setOpen(!open);
        onSave(model);
    }

    const toTimespan = (date) => {
        let h = date.getHours();
        let m = date.getMinutes();

        if (h < 10)
            h = '0' + h;
        if (m < 10)
            m = '0' + m;

        return `${h}:${m}:00`;
    }

    return (
        <CModal visible={open}>
            <CForm>
                <CModalHeader closeButton={false}>
                    <CModalTitle>افزودن نرخ جدید</CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <CRow>
                        <CCol>
                            <CFormLabel htmlFor="startTime">ساعت آغاز</CFormLabel>
                        </CCol>
                        <CCol>
                            <TimePickerComponent
                                name="startTime"
                                id="startTime"
                                format="HH:mm"
                                value={model.startTime}
                                enableRtl={true}
                                change={handleDateChange}>
                            </TimePickerComponent>
                        </CCol>
                    </CRow>
                    <CRow>
                        <CCol>
                            <CFormLabel htmlFor="endTime" >ساعت پایان</CFormLabel>
                        </CCol>
                        <CCol>
                            <TimePickerComponent
                                name="endTime"
                                id="endTime"
                                format="HH:mm"
                                value={model.endTime}
                                enableRtl={true}
                                change={handleDateChange}>
                            </TimePickerComponent>
                        </CCol>
                    </CRow>
                    <CRow>
                        <CCol>
                            <CFormLabel htmlFor="rate" >نرخ</CFormLabel>
                        </CCol>
                        <CCol>
                            <CFormInput
                                name="rate"
                                id="rate"
                                defaultValue={model.rate}
                                onChange={handleInputChange}
                                required />
                        </CCol>
                    </CRow>
                </CModalBody>
                <CModalFooter>
                    <CButton color="primary" type="button" onClick={handleSave}>ذخیره</CButton>
                    <CButton color="secondary" type="button" onClick={handleCancel}>انصراف</CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
}

EnergyTariffRateForm.propTypes = {
    onSave: PropTypes.func.isRequired,
    energyTariffId: PropTypes.string.isRequired
}

const AddEnergyTariffForm = (props) => {
    const now = Date.now();
    const toast = useToast();
    const navigate = useNavigate();

    const [addModal, showAddModal] = useState(false);
    const [model, setModel] = useState({
        id: uuidv4(),
        name: 'نام تعرفه',
        startDate: new Date(now),
        endDate: new Date(now + 60 * 86400000),
        energyTariffRates: [],
    });

    const authService = useAuthorize();

    const handleInputChange = (event) => {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        const name = event.target.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const handleDateChange = (event) => {
        const value = event.value || new Date();
        const name = event.element.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const handleCancel = (event) => {
        navigate('/setting/energytariff')
    }

    const handleSave = async (event) => {
        const accessToken = authService.accessToken;
        const service = new CRUDService(`${apiUrl}/EnergyTariffs`, accessToken);

        if (model.startDate.getTime() >= model.endDate.getTime()) {
            toast.showToast("warn", "تاریخ شروع باید کوچکتر از تاریخ پایان باشد");
            return;
        }

        if (model.name.length === 0) {
            toast.showToast("warn", "نام تعرفه باید وارد شود");
            return;
        }

        try {
            await service.create(model);
            navigate('/setting/energytariff');
        } catch (e) {
            toast.showToast("error", e.title);
            console.error("خطا در افزودن به لیست تعرفه ها!", e);
        }
    }

    const onAddEnergytariffRate = (rate) => {
        if (rate) {
            let energyTariffRates = model.energyTariffRates.filter(a => a.id !== rate.id);

            if (!isRateValid(rate)) {
                toast.showToast("warn", "این تعرفه با دیگر تعرفه ها تداخل زمانی دارد!");
            }
            else {

                setModel(prevState => ({
                    ...prevState,
                    energyTariffRates: [...energyTariffRates, rate]
                }));
            }
        }

        showAddModal(false);
    }

    const onDeleteEnergytariffRate = (id) => {
        setModel(prevState => ({
            ...prevState,
            energyTariffRates: prevState.energyTariffRates.filter(a => a.id !== id)
        }));
    }

    const isRateValid = (rate) => {
        const rateStartTime = toTime(rate.startTime);
        const rateEndTime = toTime(rate.endTime);

        for (let etr of model.energyTariffRates) {
            const startTime = toTime(etr.startTime);
            const endTime = toTime(etr.endTime);

            let valid = (rateStartTime < startTime && rateEndTime <= startTime) || (rateStartTime >= endTime);

            if (!valid) {
                return false;
            }
        }

        return true;
    }

    const toTime = (timespan) => {
        let dateStr = `1970-01-01T${timespan}.000Z`;
        let v = Date.parse(dateStr);
        console.log('timespan', timespan, 'value:', v);
        return v;
    }


    const renderEnergyTariffRate = () => {
        const data = model.energyTariffRates;
        return (
            <>
                <CButton className="mb-1" onClick={(e) => showAddModal(true)}>افزودن نرخ جدید</CButton>
                <table className="table table-sm table-bordered table-hover">
                    <thead className="thead-dark">
                        <tr className="d-flex">
                            <th className="col-2">ردیف</th>
                            <th className="col-3">ساعت آغاز</th>
                            <th className="col-3">ساعت پایان</th>
                            <th className="col-2">نرخ</th>
                            <th className="col-2">عملیات</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            data.map((d, k) =>
                                <tr className="d-flex" key={k}>
                                    <td className="col-2">{k + 1}</td>
                                    <td className="col-3">{d.startTime}</td>
                                    <td className="col-3">{d.endTime}</td>
                                    <td className="col-2">{d.rate}</td>
                                    <td className="col-2 text-left">
                                        <CButton size="sm" color="danger" type="button" onClick={(e) => onDeleteEnergytariffRate(d.id)}>حذف تعرفه</CButton>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </>
        );
    }

    return (
        <div className="animated fadeIn">
            <CCard>
                <CCardHeader>
                    <strong>افزودن تعرفه جدید</strong>
                </CCardHeader>
                <CCardBody>
                    <CForm>
                        <CRow>
                            <CCol>
                                <CRow className="mb-3">
                                    <CFormLabel htmlFor="name" className="col-sm-4 col-form-label">نام</CFormLabel>
                                    <CCol sm={6}>
                                        <CFormInput
                                            name="name"
                                            id="name"
                                            value={model.name}
                                            onChange={handleInputChange} required />
                                    </CCol>
                                </CRow>
                            </CCol>
                            <CCol>
                                <CRow className="mb-3">
                                    <CFormLabel htmlFor="startDate" className="col-sm-4 col-form-label">تاریخ شروع</CFormLabel>
                                    <CCol sm={6}>
                                        <DatePickerComponent
                                            name="startDate"
                                            id="startDate"
                                            calendarMode="Islamic"
                                            value={model.startDate}
                                            format="dd MMM yyyy"
                                            enableRtl={true}
                                            firstDayOfWeek={6}
                                            change={handleDateChange}
                                            locale="fa">
                                            <Inject services={[Islamic]} />
                                        </DatePickerComponent>
                                    </CCol>
                                </CRow>
                            </CCol>
                            <CCol>
                                <CRow className="mb-3">
                                    <CFormLabel htmlFor="endDate" className="col-sm-4 col-form-label">تاریخ پایان</CFormLabel>
                                    <CCol sm={6}>
                                        <DatePickerComponent
                                            name="endDate"
                                            id="endDate"
                                            calendarMode="Islamic"
                                            value={model.endDate}
                                            format="dd MMM yyyy"
                                            enableRtl={true}
                                            firstDayOfWeek={6}
                                            change={handleDateChange}
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
                                {renderEnergyTariffRate()}
                                {addModal ? <EnergyTariffRateForm onSave={onAddEnergytariffRate} energyTariffId={model.id} /> : null}
                            </CCol>
                        </CRow>
                        <CRow>
                            <CCol>
                                <CButton color="primary" type="button" onClick={handleSave}>ذخیره</CButton>{' '}
                                <CButton color="secondary" type="button" onClick={handleCancel}>انصراف</CButton>
                            </CCol>
                        </CRow>
                    </CForm>
                </CCardBody>
            </CCard>
        </div>
    );
}
export default AddEnergyTariffForm;

