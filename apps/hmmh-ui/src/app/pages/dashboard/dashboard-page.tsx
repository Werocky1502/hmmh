import { Text } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/auth-context';
import { useDocumentTitle } from '../../hooks/use-document-title';
import { DateRangeFilter } from '../../shared/date-range-filter/date-range-filter';
import { PageBlock } from '../../shared/page-block/page-block';
import { PageShell } from '../../shared/page-shell/page-shell';
import { UserMenu } from '../../shared/user-menu/user-menu';
import styles from './dashboard-page.module.css';
import { DashboardTodaySection } from './components/dashboard-today-section/dashboard-today-section';
import { DashboardWidgetsSection } from './components/dashboard-widgets-section/dashboard-widgets-section';
import { useDashboardData } from '../../hooks/use-dashboard-data';

export const DashboardPage = () => {
  useDocumentTitle('HMMH (Dashboard)');
  const { userName, signOut, deleteAccount } = useAuth();
  const navigate = useNavigate();
  const {
    range,
    handleRangeChange,
    todayLabel,
    rangeError,
    caloriesError,
    weightChartData,
    weightStats,
    weightChartStatus,
    calorieChartData,
    calorieStats,
    calorieChartStatus,
    xAxisProps,
    yAxisProps,
    refreshWeights,
    refreshCalories,
  } = useDashboardData();

  const handleLogout = () => {
    signOut();
    navigate('/');
  };

  const handleDelete = async () => {
    await deleteAccount();
    navigate('/');
  };

  return (
    <PageShell
      title="Help me manage health"
      rightSlot={<UserMenu userName={userName} onLogout={handleLogout} onDelete={handleDelete} />}
    >
      <DateRangeFilter todayLabel={todayLabel} range={range} onChange={handleRangeChange} />
      {rangeError ? (
        <Text size="sm" c="red" className={styles.filterError}>
          {rangeError}
        </Text>
      ) : null}
      {caloriesError ? (
        <Text size="sm" c="red" className={styles.filterError}>
          {caloriesError}
        </Text>
      ) : null}

      <DashboardWidgetsSection
        weightChartData={weightChartData}
        calorieChartData={calorieChartData}
        weightChartStatus={weightChartStatus}
        calorieChartStatus={calorieChartStatus}
        weightStats={weightStats}
        calorieStats={calorieStats}
        xAxisProps={xAxisProps}
        yAxisProps={yAxisProps}
        onWeightsClick={() => navigate('/weights')}
        onCaloriesClick={() => navigate('/calories')}
      />

      <PageBlock variant="section">
        <DashboardTodaySection onWeightSaved={refreshWeights} onCaloriesSaved={refreshCalories} />
      </PageBlock>
    </PageShell>
  );
};
