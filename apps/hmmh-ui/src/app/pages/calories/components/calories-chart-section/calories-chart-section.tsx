import type { ReactNode } from 'react';
import type { TooltipProps } from 'recharts';
import { SegmentedControl } from '@mantine/core';
import { CardShell } from '../../../../shared/card-shell/card-shell';
import { HealthChart } from '../../../../shared/health-chart/health-chart';
import chartStyles from '../../../../shared/health-chart/health-chart.module.css';
import { StatGrid } from '../../../../shared/stat-grid/stat-grid';
import type { StatItem } from '../../../../hooks/use-calories-page-data';

interface ChartStatusInfo {
  status: 'loading' | 'empty' | 'no-data' | 'ready';
  message: string;
}

interface CaloriesChartSectionProps {
  chartMode: 'total' | 'parts';
  onChartModeChange: (value: 'total' | 'parts') => void;
  dailySeries: Array<Record<string, unknown>>;
  chartSeries: Array<{ name: string; color: string }>;
  xAxisProps: Record<string, unknown>;
  yAxisProps: Record<string, unknown>;
  calorieStats: StatItem[];
  calorieChartStatus: ChartStatusInfo;
}

const renderCaloriesTooltip = ({ payload }: TooltipProps<number, string>) => {
  if (!payload || payload.length === 0) {
    return null;
  }

  const labels: Record<string, string> = {
    total: 'Total',
    morning: 'Morning',
    midday: 'Midday',
    evening: 'Evening',
    night: 'Night',
  };

  const rows = payload
    .map((entry) => ({ key: entry.name ?? '', value: Number(entry.value) }))
    .filter((entry) => Number.isFinite(entry.value) && entry.value > 0);

  if (rows.length === 0) {
    return null;
  }

  return (
    <div className={chartStyles.tooltip}>
      {rows.map((row) => (
        <div key={row.key} className={chartStyles.tooltipRow}>
          <span>{labels[row.key] ?? row.key}</span>
          <span>{`${row.value.toFixed(0)} kcal`}</span>
        </div>
      ))}
    </div>
  );
};

export const CaloriesChartSection = ({
  chartMode,
  onChartModeChange,
  dailySeries,
  chartSeries,
  xAxisProps,
  yAxisProps,
  calorieStats,
  calorieChartStatus,
}: CaloriesChartSectionProps) => {
  return (
    <CardShell
      title="Daily calories"
      actions={(
        <SegmentedControl
          size="xs"
          value={chartMode}
          onChange={(value) => onChartModeChange(value as 'total' | 'parts')}
          data={[
            { label: 'Total', value: 'total' },
            { label: 'By day part', value: 'parts' },
          ]}
        />
      )}
    >
      <HealthChart
        kind="line"
        data={dailySeries}
        dataKey="date"
        series={chartSeries}
        unit="kcal"
        xAxisProps={xAxisProps}
        yAxisProps={yAxisProps}
        status={calorieChartStatus.status}
        statusMessage={calorieChartStatus.message}
        tooltipContent={renderCaloriesTooltip as (props: TooltipProps<number, string>) => ReactNode}
      />
      <StatGrid items={calorieStats} />
    </CardShell>
  );
};
