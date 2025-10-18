import { useEffect, useState } from 'react';
import PropTypes from 'prop-types'
import {
    CCard, CCardHeader, CCardBody,
    CModal, CModalHeader, CModalBody, CModalFooter, CModalTitle,
    CCol, CRow,
    CForm, CButton, CFormInput, CFormLabel, CFormFeedback
} from '@coreui/react'

import { CRUDService } from '../../services';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { apiUrl } from '../../ApiConfig';

const AddForm = ({onSave}) => {
    const [validated, setValidated] = useState(false)
    const [open, setOpen] = useState(true)
    const [model, setModel] = useState({ name: undefined })

    const handleInputChange = (event) => {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        const name = event.target.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }

    const handleCancel = () => {
        setOpen(false)
        onSave(null);
    }

    const handleSubmit = (event) => {
        event.preventDefault();

        let form = event.currentTarget;
        if (form.checkValidity() === false) {
            event.stopPropagation();
            console.error("اطلاعات فرم معتبر نیست!");
        }
        else {
            setValidated(true)
            setOpen(false)
            onSave(model)
        }
    }

    return (
        <CModal visible={open}>
            <CForm className='g-3 needs-validation' onSubmit={handleSubmit} noValidate validated={validated}>
                <CModalHeader closeButton={false}>
                    <CModalTitle>افزودن بهره بردار جدید</CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <CRow>
                        <CCol>
                            <CFormLabel htmlFor="name">نام</CFormLabel>
                        </CCol>
                        <CCol>
                            <CFormInput
                                name="name"
                                id="name"
                                defaultValue={model.name}
                                onChange={handleInputChange}
                                required />
                            <CFormFeedback invalid>نام باید وارد شود</CFormFeedback>
                        </CCol>
                    </CRow>
                </CModalBody>
                <CModalFooter>
                    <CButton color="primary" type="submit">ذخیره</CButton>
                    <CButton color="secondary" type="button" onClick={handleCancel}>انصراف</CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
}

AddForm.propTypes = {
    onSave: PropTypes.func.isRequired,
}

const PowerplantOperators = (props) => {
    const [data, setDate] = useState([]);
    const [addModal, showAddModal] = useState(false);
    const [loading, setLoading] = useState(false);

    const authService = useAuthorize();

    useEffect(() => {
        getPowerplantOperators();
    }, []);

    const getPowerplantOperators = async () => {
        const accessToken = authService.accessToken;;
        const service = new CRUDService(`${apiUrl}/PowerplantOperators`, accessToken);

        setLoading(true);
        try {
            let d = await service.readAll();
            setDate(d);
        }
        catch (e) {
            console.error(e);
        }
        setLoading(false);
    }

    const onSaveAddModal = async (item) => {
        const accessToken = authService.accessToken;;
        const service = new CRUDService(`${apiUrl}/PowerplantOperators`, accessToken);

        setLoading(true);

        if (item) {
            try {
                let ret = await service.create(item);
                setDate(prevState => [...prevState, ret]);
            } catch (e) {
                console.error(e);
            }
        }
        setLoading(false);
    }

    const removeItem = async (id) => {
        const accessToken = authService.accessToken;
        const service = new CRUDService(`${apiUrl}/PowerplantOperators`, accessToken);

        setLoading(true);

        try {
            let ret = await service.delete(id);
            setDate(prevState => prevState.filter(x => x.id !== ret.id));
        } catch (e) {
            console.error(e);
        }
        setLoading(false);
    }

    const renderTable = () => {
        let count = data.length;

        return (
            <>
                <CButton onClick={e => showAddModal(true)}>افزودن بهره بردار نیروگاه جدید</CButton>
                <hr />
                <table className="table table-sm table-bordered table-hover">
                    <caption>تعداد مشترکین : {count}</caption>
                    <thead className="thead-dark">
                        <tr className="d-flex">
                            <th className="col-4">شناسه</th>
                            <th className="col-6">نام</th>
                            <th className="col-2">عملیات</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            data.map((d, k) =>
                                <tr className="d-flex" key={k}>
                                    <td className="col-4">{d.id}</td>
                                    <td className="col-6">{d.name}</td>
                                    <td className="col-2 text-left">
                                        <CButton color="danger" size="sm" onClick={e => removeItem(d.id)}>حذف</CButton>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </>
        );
    }
    const contents = loading
        ? <div className="animated fadeIn pt-3 text-center">بارگذاری داده ...</div>
        : renderTable();

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>فهرست بهره بردار نیروگاه</strong>
                    </CCardHeader>
                    <CCardBody>
                        {contents}
                        {addModal ? <AddForm onSave={onSaveAddModal}></AddForm> : null}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default PowerplantOperators;
