import { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";
import {
    CCard, CCardHeader, CCardBody,
    CCol, CRow,
    CButton
} from '@coreui/react'
import { CRUDService } from '../../services';
import { useToast } from '../../components';
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { apiUrl } from '../../ApiConfig';

const EnergyTariff = (props) => {
    const [loading, setLoading] = useState(false);
    const [energyTariffs, setEnergyTariffs] = useState([]);

    const toast = useToast();
    const navigate = useNavigate();

    const authService = useAuthorize();

    useEffect(() => {
        const getEnergyTariffs = async () => {
            const accessToken = authService.accessToken;;
            const service = new CRUDService(`${apiUrl}/EnergyTariffs`, accessToken);

            setLoading(true)
            try {
                let d = await service.readAll();
                setEnergyTariffs(d);
            }
            catch (e) {
                toast.showToast("error", e.title);
                console.error(e);
            }
            setLoading(false);
        };

        getEnergyTariffs();
    }, []);

    const removeItem = async (id) => {
        const accessToken = authService.accessToken;
        const service = new CRUDService(`${apiUrl}/EnergyTariffs`, accessToken);

        try {
            const retId = await service.delete(id);
            setEnergyTariffs(prevState => prevState.filter(x => x.id === retId));
        } catch (e) {
            toast.showToast("error", e.title);
        }
    }

    const renderTable = () => {
        return (
            <>
                <CButton onClick={e => navigate('/setting/addenergytariff')}>افزودن تعرفه جدید</CButton>
                <hr />
                <table className="table table-sm table-bordered table-hover">
                    <caption>تعداد تعرفه موجود : {energyTariffs?.length}</caption>
                    <thead className="thead-dark">
                        <tr className="d-flex">
                            <th className="col-1">ردیف</th>
                            <th className="col-3">نام</th>
                            <th className="col-3">تاریخ شروع</th>
                            <th className="col-3">تاریخ پایان</th>
                            <th className="col-2">عملیات</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            energyTariffs?.map((d, k) =>
                                <tr className="d-flex" key={k}>
                                    <td className="col-1">{k + 1}</td>
                                    <td className="col-3">{d.name}</td>
                                    <td className="col-3">{new Date(d.startDate).toLocaleDateString('fa-IR')}</td>
                                    <td className="col-3">{new Date(d.endDate).toLocaleDateString('fa-IR')}</td>
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
        ? <div className="animated fadeIn pt-3 text-center">بارگذاری داده...</div>
        : renderTable();

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>فهرست تعرفه ها</strong>
                    </CCardHeader>
                    <CCardBody>
                        {contents}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default EnergyTariff;
