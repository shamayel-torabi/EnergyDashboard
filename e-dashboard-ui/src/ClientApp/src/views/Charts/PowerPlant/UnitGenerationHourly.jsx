import React, { useState, useEffect } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel,
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { Select } from '../../../components';
import { useToast } from '../../../components';

import {
    Chart,
    ChartSeries,
    ChartSeriesItem,
    ChartTooltip,
    ChartTitle,
    ChartCategoryAxis,
    ChartCategoryAxisItem,
    ChartValueAxis,
    ChartValueAxisItem,
    ChartSeriesItemTooltip
} from '@progress/kendo-react-charts';
import { apiUrl } from '../../../ApiConfig';

const categories = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];

const PowerPlantEnergyChart = ({ recordDate, powerPlantId }) => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);

    const toast = useToast();

    useEffect(() => {
        getPowerplantEnergy();
    }, [recordDate, powerPlantId]);


    const getPowerplantEnergy = async () => {
        const url = `${apiUrl}/EnergyProfiles/GetPowerPlantEnergy/${powerPlantId}/${recordDate.toJSON()}`;

        setLoading(true);

        try {
            let response = await fetch(url);

            if (response.ok) {
                let d = await response.json();
                setData(d);
            }
            else {
                toast.showToast("error", "خطا در دریافت انرژی نیروگاه.");
                let err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در اتصال به سرور.");
            console.error(e);
        }

        setLoading(false);
    }

    const renderUnitEnergyCharts = () => {
        const chartSeriesItems = data.map((v, k) => {
            let d = v.items.map(item => {
                return item.importWatt / 1000000
            })
            let name = v.equipmentName;

            const tooltipRender = ({ point }) => (<span><b>{`MWh ${point.value.toLocaleString('fa-IR')}`}</b></span>);
            const title = `انرژی ساعتی واحد ${name}`;
            return (
                <CCol key={k} xs="12" sm="12" md="12" lg="6" xl="6">
                    <Chart>
                        <ChartTitle text={title} />
                        <ChartTooltip />
                        <ChartValueAxis>
                            <ChartValueAxisItem title={{ text: "MWh" }} min={0} max={300} />
                        </ChartValueAxis>
                        <ChartCategoryAxis>
                            <ChartCategoryAxisItem title={{ text: "ساعت" }} categories={categories} startAngle={45} />
                        </ChartCategoryAxis>
                        <ChartSeries>
                            <ChartSeriesItem type="line" data={d}>
                                <ChartSeriesItemTooltip render={tooltipRender} />
                            </ChartSeriesItem>
                        </ChartSeries>
                    </Chart>
                </CCol>
            );
        })

        return chartSeriesItems;
    }

    const renderCharts = () => {
        const charts = renderUnitEnergyCharts();
        return (
            <CRow>
                {charts}
            </CRow>
        )

    }

    if (loading)
        return (<span className="k-icon k-i-loading"></span>);

    if (data && data.length) {
        const charts = renderCharts();
        return charts;
    }
    else {
        return (
            <div className="animated fadeIn pt-3 text-center">داده ای موجود نیست</div>
        );
    }
};

const UnitGenerationHourly = (props) => {
    const yesterday = new Date(new Date().getTime() - 86400000);

    const [powerPlantId, setPowerPlantId] = useState(0);
    const [recordDate, setRecordDate] = useState(yesterday);
    const [powerPlants, setPowerPlants] = useState([]);
    const toast = useToast();

    useEffect(() => {
        getPowerPlants();
    }, []);


    const getPowerPlants = async () => {
        const url = `${apiUrl}/Utility/GetPowerplantOperators`;
        try {
            let response = await fetch(url);
            if (response.ok) {
                let ppOperator = await response.json();

                setPowerPlantId(ppOperator[0].value);
                setPowerPlants(ppOperator)
            }
            else {
                toast.showToast("error", "خطا در دریافت نیروگاهها.");
                let err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در اتصال به سرور.");
            console.error(e);
        }
    }

    const handleDateChange = (event) => {
        const value = event.value || new Date();
        setRecordDate(value);
    }

    const handleSelectChange = (event) => {
        const value = event.target.value;
        setPowerPlantId(value)
    }

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>گزارش روزانه تولید واحدها</strong>
                    </CCardHeader>
                    <CCardBody>
                        <CRow>
                            <CCol>
                                <CRow>
                                    <CCol xs="auto">
                                        <CFormLabel htmlFor="powerPlantId">نیروگاه</CFormLabel>
                                    </CCol>
                                    <CCol>
                                        <Select required
                                            name="powerPlantId" id="powerPlantId"
                                            options={powerPlants}
                                            value={powerPlantId}
                                            onChange={handleSelectChange} />
                                    </CCol>
                                </CRow>
                            </CCol>
                            <CCol>
                                <CRow>
                                    <CCol xs="auto">
                                        <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                                    </CCol>
                                    <CCol>
                                        <DatePickerComponent
                                            name="recordDate"
                                            id="recordDate"
                                            calendarMode="Islamic"
                                            value={recordDate}
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
                        <PowerPlantEnergyChart recordDate={recordDate} powerPlantId={powerPlantId} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default UnitGenerationHourly;
