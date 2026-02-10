import type { ReactNode } from 'react';
import type { TooltipProps } from 'recharts';
import { SimpleGrid } from '@mantine/core';
import { HealthChart } from '../../../../shared/health-chart/health-chart';
import chartStyles from '../../../../shared/health-chart/health-chart.module.css';
import { StatGrid } from '../../../../shared/stat-grid/stat-grid';
import { CardShell } from '../../../../shared/card-shell/card-shell';
import type { StatItem } from '../../../../hooks/use-dashboard-data';
import styles from './dashboard-widgets-section.module.css';

interface ChartStatusInfo {
  status: 'loading' | 'empty' | 'no-data' | 'ready';
  message: string;
}

interface DashboardWidgetsSectionProps {
  weightChartData: Array<Record<string, unknown>>;
  calorieChartData: Array<Record<string, unknown>>;
  weightChartStatus: ChartStatusInfo;
  calorieChartStatus: ChartStatusInfo;
  weightStats: StatItem[];
  calorieStats: StatItem[];
  xAxisProps: Record<string, unknown>;
  yAxisProps: Record<string, unknown>;
  onWeightsClick: () => void;
  onCaloriesClick: () => void;
}

const renderWeightTooltip = ({ payload }: TooltipProps<number, string>) => {
  const rawValue = payload?.[0]?.value;
  const numericValue = Number(rawValue);

  if (!Number.isFinite(numericValue)) {
    return null;
  }

  return <div className={chartStyles.tooltip}>{numericValue.toFixed(1)} kg</div>;
};

const renderCaloriesTooltip = ({ payload }: TooltipProps<number, string>) => {
  const rawValue = payload?.[0]?.value;
  const numericValue = Number(rawValue);

  if (!Number.isFinite(numericValue)) {
    return null;
  }

  return <div className={chartStyles.tooltip}>{`${numericValue.toFixed(0)} kcal`}</div>;
};

export const DashboardWidgetsSection = ({
  weightChartData,
  calorieChartData,
  weightChartStatus,
  calorieChartStatus,
  weightStats,
  calorieStats,
  xAxisProps,
  yAxisProps,
  onWeightsClick,
  onCaloriesClick,
}: DashboardWidgetsSectionProps) => {
  return (
    <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg" className={styles.grid}>
      <CardShell
        title="Weight trend"
        onClick={onWeightsClick}
        headerAlign="center"
        headerDirection="column"
        tone="soft"
      >
        <HealthChart
          kind="area"
          data={weightChartData}
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

      <CardShell
        title="Daily calories"
        onClick={onCaloriesClick}
        headerAlign="center"
        headerDirection="column"
        tone="soft"
      >
        <HealthChart
          kind="area"
          data={calorieChartData}
          dataKey="date"
          series={[{ name: 'calories', color: 'teal.6' }]}
          unit="kcal"
          xAxisProps={xAxisProps}
          status={calorieChartStatus.status}
          statusMessage={calorieChartStatus.message}
          tooltipContent={renderCaloriesTooltip as (props: TooltipProps<number, string>) => ReactNode}
        />
        <StatGrid items={calorieStats} />
      </CardShell>
    </SimpleGrid>
  );
};
