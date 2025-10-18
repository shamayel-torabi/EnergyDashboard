import React from 'react';

import {
    Chart,
    ChartSeries,
    ChartSeriesItem,
    ChartCategoryAxis,
    ChartCategoryAxisItem,
    ChartValueAxis,
    ChartValueAxisItem,
    ChartSeriesItemTooltip,
    ChartTooltip,
} from "@progress/kendo-react-charts";


export const EquipmentEnergyChart = React.memo(({ data }) => {
    const categories = data.map((v, i) => {
        return v.x;
    });

    const d = data.map((v, i) => {
        return v.y;
    });

    const tooltipRender = ({ point }) => (<span><b>{point.value.toLocaleString('fa-IR')}</b></span>);

    return (

        <div>
            <Chart style={{ height: 250 }}>
                <ChartTooltip />
                <ChartValueAxis>
                    <ChartValueAxisItem title={{ text: "MWh" }} />
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
        </div>
    )
});