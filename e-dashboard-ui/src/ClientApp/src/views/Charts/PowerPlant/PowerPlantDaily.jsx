import { useEffect, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel,
} from '@coreui/react';


import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { useToast } from '../../../components';
import { PersianCalendarUtil } from '../../../components';

import {
    Chart,
    ChartSeries,
    ChartSeriesItem,
    ChartTooltip,
    ChartTitle,
    ChartCategoryAxis,
    ChartCategoryAxisItem,
} from '@progress/kendo-react-charts';
import { apiUrl } from '../../../ApiConfig';

const PowerPlantEnergyChart = ({ month, year }) => {

    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);

    const toast = useToast();

    useEffect(() => {
        getPowerplantEnergy(year, month);
    }, [year, month]);


    const getPowerplantEnergy = async (y, m) => {
        let url = `${apiUrl}/DailyEnergy/GetPowerPlantsEnergyDaily/${y}/${m}`;

        setLoading(true);
        try {
            let response = await fetch(url);

            if (response.ok) {
                let d = await response.json();
                setData(d)
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

    const renderPowerPlantEnergyChartSeriesItem = (da) => {
        const d = da.map(v => {
            return { day: v.day, activeEnergy: v.activeEnergy / 1000000 }
        })

        return (
            <ChartSeriesItem type="column" data={d} field="activeEnergy" categoryField="day">
            </ChartSeriesItem>
        );
    }

    const renderPowerPlantEnergyChart = (d, k) => {
        let chartSeriesItem = renderPowerPlantEnergyChartSeriesItem(d.energies);
        const defaultTooltipRender = ({ point }) => point.value.toLocaleString('fa-IR');

        return (
            <CCol md="6" lg="6" key={k}>
                <CCard>
                    <CCardBody>
                        <Chart>
                            <ChartTooltip render={defaultTooltipRender} />
                            <ChartTitle text={"انرژی تولیدی روزانه نیروگاه " + d.powerPlantName + " MWh"} />
                            <ChartCategoryAxis>
                                <ChartCategoryAxisItem title={{ text: "روز" }} />
                            </ChartCategoryAxis>
                            <ChartSeries>
                                {chartSeriesItem}
                            </ChartSeries>
                        </Chart>
                    </CCardBody>
                </CCard>
            </CCol>
        );
    }

    if (loading)
        return (<span className="k-icon k-i-loading"></span>);

    if (data && data.length) {
        const charts = data.map((d, k) => {
            return renderPowerPlantEnergyChart(d, k);
        })
        return (
            <CRow>
                {charts}
            </CRow>
        );
    }
    else {
        return (
            <div className="animated fadeIn pt-3 text-center">داده ای موجود نیست</div>
        );
    }
};

const PowerPlantDaily = (props) => {
    const date = new Date();
    const m = PersianCalendarUtil.hijriMonth(date);
    const y = PersianCalendarUtil.hijriYear(date)

    const [model, setModel] = useState({
        hYear: y,
        hMonth: m,
        recordDate: PersianCalendarUtil.gregorianDate(y, m, 5)
    });


    const handleDateChange = (event) => {
        const value = event.value || new Date();

        const y = PersianCalendarUtil.hijriYear(value);
        const m = PersianCalendarUtil.hijriMonth(value);

        const mdl = {
            hYear: y,
            hMonth: m,
            recordDate: PersianCalendarUtil.gregorianDate(y, m, 5)
        }

        setModel(mdl);
    }

    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>نمودار تولید روزانه نیروگاهها</strong>
                    </CCardHeader>
                    <CCardBody>
                        <CRow>
                            <CCol xs="auto">
                                <CFormLabel htmlFor="recordDate">ماه</CFormLabel>
                            </CCol>
                            <CCol xs="auto">
                                <DatePickerComponent
                                    name="recordDate"
                                    id="recordDate"
                                    calendarMode="Islamic"
                                    format="MMMM yyyy"
                                    enableRtl={true}
                                    firstDayOfWeek={6}
                                    value={model.recordDate}
                                    start="Year"
                                    depth="Year"
                                    change={handleDateChange}
                                    locale="fa">
                                    <Inject services={[Islamic]} />
                                </DatePickerComponent>
                            </CCol>
                        </CRow>
                        <hr />
                        <PowerPlantEnergyChart month={model.hMonth} year={model.hYear} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default PowerPlantDaily;
