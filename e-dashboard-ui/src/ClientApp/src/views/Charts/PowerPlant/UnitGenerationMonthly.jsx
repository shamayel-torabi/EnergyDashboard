import { useState, useEffect } from 'react';
import {
    CCard, CCardHeader, CCardBody,
    CRow, CCol,
    CFormLabel, CFormCheck,
} from '@coreui/react';

import { DatePickerComponent, Islamic, Inject } from '@syncfusion/ej2-react-calendars';
import { useToast } from '../../../components';
import { PersianCalendarUtil } from '../../../components/PersianCalendarUtil';
import { persian } from '../../../global';

import {
    Chart,
    ChartLegend,
    ChartSeries,
    ChartTooltip,
    ChartTitle,
    ChartSeriesItem,
    ChartSeriesItemTooltip
} from '@progress/kendo-react-charts';
import { apiUrl } from '../../../ApiConfig';


const PowerPlantEnergyChart = ({ year, stack }) => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);

    const toast = useToast();

    useEffect(() => {
        getPowerplantEnergy(year)
    }, [year])


    const getPowerplantEnergy = async (y) => {
        const url = `${apiUrl}/DailyEnergy/GetUnitMonthly/${y}`;

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
        return (
            units.map((unit, k) => {
                let d = unit.energies.map((v, k) => {
                    return {
                        month: persian.main.fa.dates.calendars.islamic.months.format.wide[v.month],
                        activeEnergy: v.activeEnergy / 1000000
                    }
                });
                let name = unit.unitName;

                const tooltipRender = ({ point }) => (<span><b>{name}:{point.value.toLocaleString('fa-IR')}</b></span>);

                return (
                    <ChartSeriesItem key={k} type="column" stack={stack} data={d} field="activeEnergy" categoryField="month">
                        <ChartSeriesItemTooltip render={tooltipRender} />
                    </ChartSeriesItem>
                );

            })
        );
    }

    const renderPowerPlantEnergyChart = (d, k) => {
        const chartSeriesItem = renderPowerPlantEnergyChartSeriesItem(d.units);
        return (
            <CCol md="12" lg="6" key={k}>
                <CCard>
                    <CCardBody>
                        <Chart>
                            <ChartTooltip />
                            <ChartTitle text={"انرژی تولیدی ماهیانه واحدهای نیروگاه " + d.powerPlantName} />
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
}

const UnitGenerationMonthly = (props) => {
    const date = new Date();

    const [model, setModel] = useState({
        hYear: PersianCalendarUtil.hijriYear(date),
        recordDate: date,
        stackChart: false,
    });

    const handleDateChange = (event) => {
        const value = event.value || new Date();

        const mdl = {
            hYear: PersianCalendarUtil.hijriYear(value),
            recordDate: value,
            stackChart: model.stackChart
        };

        setModel(mdl);
    }

    const handleInputChange = (event) => {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        const name = event.target.name;

        setModel(prevState => ({
            ...prevState,
            [name]: value
        }));
    }


    return (
        <CRow className="animated fadeIn">
            <CCol>
                <CCard>
                    <CCardHeader>
                        <strong>نمودار ماهیانه تولید واحدهای نیروگاه</strong>
                    </CCardHeader>
                    <CCardBody>
                        <CRow>
                            <CCol>
                                <CRow>
                                    <CCol xs="auto">
                                        <CFormLabel htmlFor="recordDate">سال</CFormLabel>
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
                            </CCol>
                            <CCol>
                                <CRow>
                                    <CCol>
                                        <CFormCheck
                                            id="stackChart"
                                            name="stackChart"
                                            defaultChecked={model.stackChart}
                                            label="نمودار تجمعی"
                                            onChange={handleInputChange} >
                                        </CFormCheck>
                                    </CCol>
                                </CRow>
                            </CCol>
                        </CRow>
                        <hr />
                        <PowerPlantEnergyChart year={model.hYear} stack={model.stackChart} />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export default UnitGenerationMonthly;
