import { useEffect, useState } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel,
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { useToast } from '../../../components';
import { PersianCalendarUtil } from '../../../components/PersianCalendarUtil';
import { persian } from '../../../global';

import {
    Chart,
    ChartLegend,
    ChartSeries,
    ChartSeriesItem,
    ChartTooltip,
    ChartTitle,
} from '@progress/kendo-react-charts';
import { apiUrl } from '../../../ApiConfig';

const PowerPlantEnergyChart = ({ year }) => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);

    const toast = useToast();

    useEffect(() => {
        getPowerplantEnergy(year);
    }, [year]);

    const getPowerplantEnergy = async (y) => {
        const url = `${apiUrl}/DailyEnergy/GetPowerPlantsMonthly/${y}`;

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

    const renderPowerPlantEnergyChartSeriesItem = (units) => {
        let d = units.map((v, k) => {
            return {
                month: persian.main.fa.dates.calendars.islamic.months.format.wide[v.month],
                activeEnergy: v.activeEnergy / 1000000
            }
        })

        return (
            <ChartSeriesItem type="column" data={d} field="activeEnergy" categoryField="month">
            </ChartSeriesItem>
        );
    }

    const renderPowerPlantEnergyChart = (d, k) => {
        let chartSeriesItem = renderPowerPlantEnergyChartSeriesItem(d.energies);
        const defaultTooltipRender = ({ point }) => point.value.toLocaleString('fa-IR');

        return (
            <CCol md="12" lg="6" key={k}>
                <CCard>
                    <CCardBody>
                        <Chart>
                            <ChartTooltip render={defaultTooltipRender} />
                            <ChartTitle text={"انرژی تولیدی ماهیانه نیروگاه " + d.powerPlantName + " MWh"} />
                            <ChartLegend position="bottom" orientation="horizontal" />
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

const PowerPlantMonthly = (props) => {
    const date = new Date();
    const y = PersianCalendarUtil.hijriYear(date);

    const [model, setModel] = useState({
        hYear: y,
        recordDate: date
    });

    const handleDateChange = (event) => {
        const value = event.value || new Date();

        const mdl = {
            hYear: PersianCalendarUtil.hijriYear(value),
            recordDate: value
        };

        setModel(mdl);
    }

    return (
        <CCard className="animated fadeIn">
            <CCardHeader>
                <strong>نمودار تولید ماهیانه نیروگاه</strong>
            </CCardHeader>
            <CCardBody>
                <CRow>
                    <CCol xs="auto">
                        <CFormLabel htmlFor="recordDate">تاریخ</CFormLabel>
                    </CCol>
                    <CCol xs="auto">
                        <DatePickerComponent
                            name="recordDate"
                            id="recordDate"
                            calendarMode="Islamic"
                            format="yyyy"
                            enableRtl={true}
                            firstDayOfWeek={6}
                            value={model.recordDate}
                            start="Decade"
                            depth="Decade"
                            change={handleDateChange}
                            locale="fa">
                            <Inject services={[Islamic]} />
                        </DatePickerComponent>
                    </CCol>
                </CRow>
                <hr />
                <PowerPlantEnergyChart year={model.hYear} />
            </CCardBody>
        </CCard>
    );
}

export default PowerPlantMonthly;
