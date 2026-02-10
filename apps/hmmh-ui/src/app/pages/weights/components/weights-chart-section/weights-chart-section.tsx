import type { ReactNode } from 'react';
import type { TooltipProps } from 'recharts';
import { CardShell } from '../../../../shared/card-shell/card-shell';
import { HealthChart } from '../../../../shared/health-chart/health-chart';
import chartStyles from '../../../../shared/health-chart/health-chart.module.css';
import { StatGrid } from '../../../../shared/stat-grid/stat-grid';
import type { StatItem } from '../../../../hooks/use-weights-page-data';

interface ChartStatusInfo {
  status: 'loading' | 'empty' | 'no-data' | 'ready';
  message: string;
}

interface WeightsChartSectionProps {
  chartData: Array<Record<string, unknown>>;
  xAxisProps: Record<string, unknown>;
  yAxisProps: Record<string, unknown>;
  weightStats: StatItem[];
  weightChartStatus: ChartStatusInfo;
}

const renderWeightTooltip = ({ payload }: TooltipProps<number, string>) => {
  const rawValue = payload?.[0]?.value;
  const numericValue = Number(rawValue);

  if (!Number.isFinite(numericValue)) {
    return null;
  }

  return <div className={chartStyles.tooltip}>{numericValue.toFixed(1)} kg</div>;
};

export const WeightsChartSection = ({
  chartData,
  xAxisProps,
  yAxisProps,
  weightStats,
  weightChartStatus,
}: WeightsChartSectionProps) => {
  return (
    <CardShell title="Weight trend">
      <HealthChart
        kind="area"
        data={chartData}
        dataKey="date"
        series={[{ name: 'weight', color: 'teal.6' }]}
        unit="kg"
        xAxisProps={xAxisProps}
        yAxisProps={yAxisProps}
        status={weightChartStatus.status}
        statusMessage={weightChartStatus.message}
        tooltipContent={renderWeightTooltip as (props: TooltipProps<number, string>) => ReactNode}
      />
      <StatGrid items={weightStats} />
    </CardShell>
  );
};
