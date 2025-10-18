import React, { useState, useEffect } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CForm, CFormInput, CFormLabel,
    CButton, CFormCheck,
    CModal, CModalHeader, CModalBody, CModalFooter, CModalTitle,
} from '@coreui/react';
import CIcon from '@coreui/icons-react'
import { cilPencil } from '@coreui/icons'
import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import DataTable from "../../components/DataTable";
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { useToast } from '../../components';
import { apiUrl } from '../../ApiConfig';

const EditMeter = (props) => {
    const [model, setModel] = useState(props.model);

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
        props.onSave(props.mode, null);
    }

    const handleSave = async (event) => {
        event.preventDefault();
        let form = event.target;

        if (form.checkValidity() === false) {
            event.stopPropagation();
        }
        else {
            if (model) {
                props.onSave(props.mode, model);
            }
            else
                props.onSave(props.mode, null);
        }
    }

    const title = `${props.title} میتر`;

    return (

        <CModal visible size="lg" alignment="center" className={props.className}>
            <CModalHeader closeButton={false}>
                <CModalTitle>{title}</CModalTitle>
            </CModalHeader>
            <CModalBody>
                <CRow>
                    <CCol md={12}>
                        <CForm className='needs-validation' onSubmit={handleSave} noValidate id="addMeterForm">
                            <CRow className="mb-1">
                                <CFormLabel htmlFor="meterId" className="col-sm-5 col-form-label">شناسه میتر</CFormLabel>
                                <CCol sm={7} >
                                    <CFormInput
                                        name="meterId"
                                        id="meterId"
                                        readOnly
                                        value={model.meterId} />
                                </CCol>
                            </CRow>
                            <CRow className="mb-1">
                                <CFormLabel htmlFor="serialNumber" className="col-sm-5 col-form-label">شماره سریال</CFormLabel>
                                <CCol sm={7} >
                                    <CFormInput
                                        name="serialNumber"
                                        id="serialNumber"
                                        readOnly
                                        value={model.serialNumber} />
                                </CCol>
                            </CRow>
                            <CRow className="mb-1">
                                <CFormLabel htmlFor="startOperationDate" className="col-sm-5 col-form-label">تاریخ شروع بهره برداری</CFormLabel>
                                <CCol sm={7} >
                                    <DatePickerComponent
                                        name="startOperationDate"
                                        id="startOperationDate"
                                        calendarMode="Islamic"
                                        value={model.startOperationDate}
                                        change={handleDateChange}
                                        format="yyyy-MM-dd"
                                        enableRtl={true}
                                        firstDayOfWeek={6}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>
                            <CRow className="mb-1">
                                <CFormLabel htmlFor="endOperationDate" className="col-sm-5 col-form-label">تاریخ پایان بهره برداری</CFormLabel>
                                <CCol sm={7} >
                                    <DatePickerComponent
                                        name="endOperationDate"
                                        id="endOperationDate"
                                        calendarMode="Islamic"
                                        value={model.endOperationDate}
                                        change={handleDateChange}
                                        format="yyyy-MM-dd"
                                        enableRtl={true}
                                        firstDayOfWeek={6}
                                        locale="fa">
                                        <Inject services={[Islamic]} />
                                    </DatePickerComponent>
                                </CCol>
                            </CRow>

                            <CRow className="mb-3">
                                <div className="col-sm-7 offset-sm-5">
                                    <CFormCheck
                                        type="checkbox"
                                        id="active"
                                        name="active"
                                        label="وضعیت"
                                        defaultChecked={model.active}
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </CRow>

                            <CRow className="mb-3">
                                <div className="col-sm-7 offset-sm-5">
                                    <CFormCheck
                                        type="checkbox"
                                        id="dismount"
                                        name="dismount"
                                        label="خارج از رده"
                                        defaultChecked={model.dismount}
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </CRow>

                            <CRow className="mb-3">
                                <div className="col-sm-7 offset-sm-5">
                                    <CFormCheck
                                        type="checkbox"
                                        id="selectByEquipment"
                                        name="selectByEquipment"
                                        label="انتخاب شده"
                                        defaultChecked={model.selectByEquipment}
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </CRow>

                        </CForm>
                    </CCol>
                </CRow>
            </CModalBody>
            <CModalFooter>
                <CButton color="primary" type="submit" form="addMeterForm">ذخیره</CButton>
                <CButton color="secondary" type="button" onClick={handleCancel}>انصراف</CButton>
            </CModalFooter>
        </CModal>
    );
}

const MeterDataTable = (props) => {
    const [showDialog, setShowDialog] = useState(false);
    const [mode, setMode] = useState('Add');
    const [meter, setMeter] = useState(undefined);
    const [meters, setMeters] = useState([]);
    const [loading, setLoading] = useState(false);

    const authService = useAuthorize();
    const toast = useToast();

    useEffect(() => {
        const getMeters = async () => {
            const url = `${apiUrl}/Meters`;

            try {
                let response = await fetch(url);
                if (response.ok) {
                    let mtrs = await response.json();
                    setMeters(mtrs)
                }
                else {
                    toast.showToast("error", "خطا در دریافت میتر.");
                    let err = await response.json();
                    console.error(err);
                }
            }
            catch (e) {
                toast.showToast("error", "خطا در دریافت میتر.");
                console.error(e);
            }
        }

        getMeters();
    }, [],);

    const addMeter = (e) => {
        setMode('Add');
        setShowDialog(true)
    }

    const editMeter = (meterId) => {
        let found = meters.find(element => element.meterId === meterId);

        setMode('Edit');
        setMeter(found);
        setShowDialog(true);
    }

    const deleteMeter = async (meterId) => {
        let mtrs = meters;
        let mtr = mtrs.find(element => element.meterId === meterId);
        const index = mtrs.indexOf(mtr);

        let m = await deleteMeterFromDadabase(meterId);

        if (m) {
            mtrs.splice(index, 1);
            setMeters(mtrs)
        }
    }

    const saveMeter = async (mode, mtr) => {
        let mtrs = meters;
        if (mtr && mode === 'Add') {
            let m = addMeterToDatabase(mtr);
            if (m)
                mtrs.push(mtr);
        } else if (mtr && mode === 'Edit') {
            updateMeterInDatabase(mtr);
        }
        setMeters(mtrs);
        setShowDialog(false);
    }

    const deleteMeterFromDadabase = async (meterId) => {
        const url = `${apiUrl}/Meters/${meterId}`;

        const accessToken = authService.accessToken;;

        setLoading(true);
        let ret = null;

        try {
            let req = new Request(url, {
                method: 'DELETE',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${accessToken}`
                })
            })

            let response = await fetch(req);

            if (response.ok) {
                ret = await response.json();
            }
            else if (response.status === 401 || response.status === 403) {
                toast.showToast("error", "خطای عدم مجوز دسترسی.");
                const err = await response.json();
                console.error(err);
                ret = null;
            }
            else {
                toast.showToast("error", "حذف میتر از بانک اطلاعاتی با خطا مواجه شده است.");
                const err = await response.json();
                console.error(err);
                ret = null;
            }

        }
        catch (e) {
            toast.showToast("error", "حذف میتر از بانک اطلاعاتی با خطا مواجه شده است.");
            console.error(e);
        }
        setLoading(false);
        return ret;
    }

    const addMeterToDatabase = async (meter) => {
        const url = `${apiUrl}/Meters`;

        const accessToken = authService.accessToken;;
        setLoading(true);

        let ret = null;

        try {
            let req = new Request(url, {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${accessToken}`
                }),
                body: JSON.stringify(meter)
            })

            let response = await fetch(req);
            if (response.ok) {
                ret = await response.json();
            }
            else if (response.status === 401 || response.status === 403) {
                toast.showToast("error", "خطای عدم مجوز دسترسی.");
                const err = await response.json();
                console.error(err);
            }
            else {
                toast.showToast("error", "افزودن میتر به بانک اطلاعاتی با خطا مواجه شده است.");
                const err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            toast.showToast("error", "افزودن میتر به بانک اطلاعاتی با خطا مواجه شده است.");
            console.error("افزودن میتر به بانک اطلاعاتی با خطا مواجه شده است.");
        }

        setLoading(false);
        return ret;
    }

    const updateMeterInDatabase = async (meter) => {
        const url = `${apiUrl}/Meters/${meter.meterId}`;

        const accessToken = authService.accessToken;;
        setLoading(true);

        try {
            let req = new Request(url, {
                method: 'PUT',
                headers: new Headers({
                    'Authorization': ` Bearer ${accessToken}`
                }),
                body: JSON.stringify(meter)
            })

            let response = await fetch(req);

            if (response.ok) {
                toast.showToast("info", 'میتر به روز رسانی شد');
            }
            else if (response.status === 401 || response.status === 403) {
                toast.showToast("error", "خطای عدم مجوز دسترسی.");
                const err = await response.json();
                console.error(err);
            }
            else {
                toast.showToast("error", "به روز رسانی میتر در بانک اطلاعاتی با خطا مواجه شده است.");
                const err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            toast.showToast("error", "به روز رسانی میتر در بانک اطلاعاتی با خطا مواجه شده است.");
            console.error(e);
        }
        setLoading(false)
    }

    const updateMeterTable = async (e) => {
        const url = `${apiUrl}/Meters/UpdateMeterTabels`;
        const accessToken = authService.accessToken;
        setLoading(true)

        try {
            let req = new Request(url, {
                method: 'PUT',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${accessToken}`
                })
            })

            let response = await fetch(req);

            if (response.ok) {
                toast.showToast("info", 'جدول میتر به روز رسانی شد');
            }
            else if (response.status === 401 || response.status === 403) {
                toast.showToast("error", "خطای عدم مجوز دسترسی.");
                const err = await response.json();
                console.error(err);
            }
            else {
                toast.showToast("error", "خطا در به روز رسانی جدول میتر.");
                const err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            toast.showToast("error", "خطای اتصال به سرور");
            console.error(e);
        }
        setLoading(false);
    }

    const renderEditMeterDialog = () => {
        const title = mode === 'Add' ? 'افزودن' : 'ویرایش';
        return showDialog ? (<EditMeter onSave={saveMeter} title={title} model={meter} mode={mode} />) : null;
    }

    const columns = [
        {
            name: "شناسه",
            selector: "meterId",
            sortable: true
        },
        {
            name: "ایستگاه",
            selector: "stationName",
            sortable: true
        },
        {
            name: "نام",
            selector: "name",
            sortable: true
        },
        {
            name: "شماره سریال",
            selector: "serialNumber",
            sortable: true,
        },
        {
            name: "شروع بهره برداری",
            selector: "startOperationDate",
            sortable: true,
            cell: row => <span>{new Date(row.startOperationDate).toLocaleDateString('fa-IR', { year: 'numeric', month: '2-digit', day: '2-digit' })}</span>,
        },
        {
            name: "پایان بهره برداری",
            selector: "endOperationDate",
            sortable: true,
            cell: row => <span>{new Date(row.endOperationDate).toLocaleDateString('fa-IR', { year: 'numeric', month: '2-digit', day: '2-digit' })}</span>,
        },
        {
            name: "انتخاب",
            selector: "selectByEquipment",
            sortable: true,
            center: true,
            cell: row => <input type="checkbox" defaultChecked={row.selectByEquipment} />,
        },
        {
            name: "فعال",
            selector: "active",
            sortable: true,
            center: true,
            cell: row => <input type="checkbox" defaultChecked={row.active} />,
        },
        {
            name: "خارج از رده",
            selector: "dismount",
            sortable: true,
            center: true,
            cell: row => <input type="checkbox" defaultChecked={row.dismount} />,
        },
        {
            name: "عملیات",
            center: true,
            minWidth: "120px",
            cell: row => (
                <CButton color="primary" size="sm" type="button" onClick={e => editMeter(row.meterId)} title="ویرایش">
                    <CIcon icon={cilPencil} height={16} />
                </CButton>
            ),
        },
    ];
    const data = meters;
    const editMeterForm = renderEditMeterDialog();
    const spin = loading ? <span className="mr-3 spinner-border spinner-border-sm">{' '}</span> : null;

    return (
        <CCard className="animated fadeIn" style={{ marginBottom: '0.1rem' }}>
            <CCardHeader>
                <strong>فهرست میتر</strong>
                <CButton type="button" className="float-end" onClick={updateMeterTable}>
                    {spin}
                    {"    "}
                    <span>به روز رسانی جدول میتر</span>
                </CButton>
            </CCardHeader>
            <CCardBody>
                <DataTable
                    columns={columns}
                    data={data}
                    defaultSortField="stationName"
                    pagination
                    striped
                    noHeader
                />
                {editMeterForm}
            </CCardBody>
        </CCard>
    )
}

export default MeterDataTable;