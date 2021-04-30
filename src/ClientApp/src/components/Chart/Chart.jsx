import React from 'react';
import { Line } from 'react-chartjs-2';

export const StatsChart = ({chartData}) => {
    const data = {
        labels: chartData.map((_, index) => `${index} ход`),
        datasets: [
            {
                data: chartData.map(data => data.ill),
                label: 'Ill ',
                fill: false,
                lineTension: 0.1,
                backgroundColor: 'rgba(75,192,192,0.4)',
                borderColor: 'rgba(75,192,192,1)',
            },
            {
                data: chartData.map(data => data.recovered),
                label: 'Recovered',
                fill: false,
                lineTension: 0.1,
                backgroundColor: 'rgba(75,192,192,0.4)',
                borderColor: 'green',
            },
            {
                data: chartData.map(data => data.dead),
                label: 'Dead',
                fill: false,
                lineTension: 0.1,
                borderColor: 'black',
            },
            {
                data: chartData.map(data => data.healthy),
                label: 'Healthy',
                fill: false,
                lineTension: 0.1,
                borderColor: 'green',
            }
        ]
    }

    return (
        <div>
            <Line data={data} />
        </div>
    );
};
