import { AreaChart, LineChart } from '@mantine/charts';
import { Text } from '@mantine/core';
import type { ComponentProps, ReactNode } from 'react';
import type { TooltipProps } from 'recharts';
import styles from './health-chart.module.css';

type ChartKind = 'area' | 'line';

type AreaChartProps = ComponentProps<typeof AreaChart>;
type LineChartProps = ComponentProps<typeof LineChart>;

type Series = { name: string; color: string };

type ChartStatus = 'loading' | 'empty' | 'no-data' | 'ready';

interface HealthChartProps {
  kind: ChartKind;
  data: Array<Record<string, unknown>>;
  dataKey: string;
  series: Series[];
  unit?: string;
  height?: number;
  curveType?: AreaChartProps['curveType'];
  xAxisProps?: AreaChartProps['xAxisProps'];
  yAxisProps?: AreaChartProps['yAxisProps'];
  status: ChartStatus;
  statusMessage: string;
  tooltipContent?: (props: TooltipProps<number, string>) => ReactNode;
}

export const HealthChart = ({
  kind,
  data,
  dataKey,
  series,
  unit,
  height = 120,
  curveType = 'monotone',
  xAxisProps,
  yAxisProps,
  status,
  statusMessage,
  tooltipContent,
}: HealthChartProps) => {
  const renderChart = () => {
    if (kind === 'area') {
      return (
        <AreaChart
          h={height}
          w="100%"
          data={data}
          dataKey={dataKey}
          series={series}
          curveType={curveType}
          withDots
          dotProps={{ r: 3 }}
          activeDotProps={{ r: 5 }}
          strokeWidth={3}
          areaProps={{ stroke: 'var(--mantine-color-teal-6)', fillOpacity: 0.15 }}
          gridAxis="x"
          withXAxis
          withYAxis
          xAxisProps={xAxisProps}
          yAxisProps={yAxisProps}
          unit={unit}
          tooltipProps={tooltipContent ? { content: tooltipContent } : undefined}
        />
      );
    }

    return (
      <LineChart
        h={height}
        w="100%"
        data={data}
        dataKey={dataKey}
        series={series}
        curveType={curveType as LineChartProps['curveType']}
        withDots
        dotProps={{ r: 3 }}
        activeDotProps={{ r: 5 }}
        strokeWidth={3}
        gridAxis="x"
        withXAxis
        withYAxis
        xAxisProps={xAxisProps}
        yAxisProps={yAxisProps}
        unit={unit}
        tooltipProps={tooltipContent ? { content: tooltipContent } : undefined}
      />
    );
  };

  return (
    <div className={styles.sparkline}>
      {status === 'ready' ? (
        renderChart()
      ) : (
        <Text size="sm" c="dimmed" className={styles.emptyState}>
          {statusMessage}
        </Text>
      )}
    </div>
  );
};
