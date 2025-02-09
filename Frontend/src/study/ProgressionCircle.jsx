import React from 'react';

// Function to ensure percentage is between 0 and 100
const cleanPercentage = (percentage) => {
    const tooLow = !Number.isFinite(+percentage) || percentage < 0;
    const tooHigh = percentage > 100;
    return tooLow ? 0 : tooHigh ? 100 : +percentage;
};

const Circle = ({ color, percentage }) => {
    const radius = 70;
    const circle = 2 * Math.PI * radius;
    const strokePct = ((100 - percentage) * circle) / 100;

    return (
        <circle
            r={radius}
            cx={100}
            cy={100}
            fill="transparent"
            stroke={strokePct !== circle ? color : ''}
            strokeWidth="2rem"
            strokeDasharray={circle}
            strokeDashoffset={circle}
        >
            <animate
                attributeName="stroke-dashoffset"
                from={circle}
                to={strokePct}
                dur="1.5s"
                begin="0s"
                fill="freeze"
                keyTimes="0; 1"
                keySplines="0.42, 0, 0.58, 1"
                calcMode="spline"
            />
        </circle>
    );
};

// Text component to display percentage in the center
const Text = ({ percentage }) => {
    return (
        <text
            x="50%"
            y="50%"
            dominantBaseline="central"
            textAnchor="middle"
            fontSize="1.5em"
            fontWeight="bold"
        >
            {percentage.toFixed(2)}%
        </text>
    );
};

// Main ProgressionCircle component
const ProgressionCircle = ({ percentage, color }) => {
    const pct = cleanPercentage(percentage); // Ensure percentage is between 0 and 100

    return (
        <svg width={200} height={200}>
            <g transform="rotate(-90 100 100)">
                <Circle color="lightgrey" percentage={100} />
                <Circle color={color} percentage={pct} />
            </g>
            <Text percentage={pct} />
        </svg>
    );
};

export default ProgressionCircle;
