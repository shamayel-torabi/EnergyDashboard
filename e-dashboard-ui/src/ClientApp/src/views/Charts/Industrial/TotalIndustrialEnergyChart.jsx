import { useEffect, useState } from 'react';

import {
    CCard, CCardBody,
    CRow, CCol,
} from '@coreui/react';

import {
    Chart,
    ChartLegend,
    ChartSeries,
    ChartSeriesItem,
    ChartTooltip,
    ChartTitle,
    ChartValueAxis,
    ChartValueAxisItem,
} from '@progress/kendo-react-charts';
import { useToast } from '../../../components';
import { apiUrl } from '../../../ApiConfig';

const TotalIndustrialEnergyChart = ({ year }) => {
    const [data, setData] = useState(undefined);
    const [pieData, setPieData] = useState(undefined);
    const [loading, setLoading] = useState(false);

    const toast = useToast();

    useEffect(() => {
        getIndustrialLoadEnergyYear(year);
    }, [year]);

    const getIndustrialLoadEnergyYear = async (year) => {
        const url = `${apiUrl}/DailyEnergy/GetIndustrialLoadEnergyYear/${year}`;

        setLoading(true)

        try {
            let response = await fetch(url);

            if (response.ok) {
                let d = await response.json();

                let sum = d.map(v => {
                    return v.activeEnergy
                }).reduce((a, b) => a + b, 0);

                let pdd = d.map(v => {
                    return {
                        value: Math.round(v.activeEnergy * 10000 / sum) / 100,
                        category: v.name,
                    }
                })

                let dd = d.map(v => {
                    return {
                        value: v.activeEnergy / 1000000,
                        category: v.name,
                    }
                });

                setData(dd);
                setPieData(pdd);
            }
            else {
                toast.showToast("error", "خطا در دریافت انرژی بارهای صنعتی.");
                let err = await response.json();
                console.error(err);
                setData(undefined);
                setPieData(undefined);
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در اتصال به سرور.");
            console.error(e);
            setData(undefined);
            setPieData(undefined);
        }
        setLoading(false)
    }

    const defaultTooltipRender = ({ point }) => {
        let formatedNumber = Number(point.value * 1000000).toLocaleString("fa-IR");
        return `${formatedNumber}Wh`;
    }

    const labelContent = (props) => {
        let formatedNumber = Number(props.percentage).toLocaleString("fa-IR", { style: 'percent', minimumFractionDigits: 2 });
        return `${props.dataItem.category} : ${formatedNumber}`;
    }

    const labelContentColumn = (props) => {
        let formatedNumber = Number(props.dataItem.value).toLocaleString("fa-IR");
        return `${formatedNumber}`;
    }

    const chartValueAxisItemRender = (e) => {
        return Number(e.value).toLocaleString('fa-IR');
    };

    if (loading)
        return (<span className="k-icon k-i-loading"></span>);

    return (
        <CRow className="mb-2">
            <CCol lg={6}>
                <CCard>
                    <CCardBody>
                        <Chart>
                            <ChartTitle text="درصد انرژی مصرفی بارهای صنعتی از ابتدای سال تاکنون" />
                            <ChartLegend position="left" />
                            <ChartSeries>
                                <ChartSeriesItem type="pie" data={pieData} labels={{ visible: true, content: labelContent }}>
                                </ChartSeriesItem>
                            </ChartSeries>
                        </Chart>
                    </CCardBody>
                </CCard>
            </CCol>
            <CCol lg={6}>
                <CCard>
                    <CCardBody>
                        <Chart>
                            <ChartTooltip render={defaultTooltipRender} />
                            <ChartLegend position="bottom" orientation="horizontal" />
                            <ChartTitle text="انرژی مصرفی بارهای صنعتی از ابتدای سال تاکنون" />
                            <ChartSeries>
                                <ChartSeriesItem type="column" data={data} categoryField="category" labels={{ visible: true, content: labelContentColumn }}>
                                </ChartSeriesItem>
                            </ChartSeries>
                            <ChartValueAxis>
                                <ChartValueAxisItem title={{ text: "MWh" }} labels={{ visible: true, content: chartValueAxisItemRender }} />
                            </ChartValueAxis>
                        </Chart>
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}

export { TotalIndustrialEnergyChart }

