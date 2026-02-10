import { Button, Table, Text } from '@mantine/core';
import { IconArrowLeft, IconTrash } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/auth-context';
import { useDocumentTitle } from '../../hooks/use-document-title';
import { DateRangeFilter } from '../../shared/date-range-filter/date-range-filter';
import { EntriesTable } from '../../shared/entries-table/entries-table';
import { PageBlock } from '../../shared/page-block/page-block';
import { PageShell } from '../../shared/page-shell/page-shell';
import { UserMenu } from '../../shared/user-menu/user-menu';
import { CalorieEntryCard } from '../../shared/calorie-entry-card/calorie-entry-card';
import { formatDisplayDate } from '../../utils/global-utils';
import styles from './calories-page.module.css';
import { CaloriesChartSection } from './components/calories-chart-section/calories-chart-section';
import { useCaloriesPageData } from '../../hooks/use-calories-page-data';

export const CaloriesPage = () => {
  useDocumentTitle('HMMH (Calories)');
  const { userName, signOut, deleteAccount } = useAuth();
  const navigate = useNavigate();
  const {
    range,
    handleRangeChange,
    todayLabel,
    rangeError,
    isLoadingRange,
    deletingId,
    entryDate,
    setEntryDate,
    chartMode,
    setChartMode,
    dailySeries,
    chartSeries,
    xAxisProps,
    yAxisProps,
    calorieStats,
    calorieChartStatus,
    sortedEntries,
    handleDeleteEntry,
    refreshCalories,
  } = useCaloriesPageData();

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
      title="My calorie history"
      leftSlot={(
        <Button
          variant="subtle"
          onClick={() => navigate('/dashboard')}
          className={styles.backButton}
          leftSection={<IconArrowLeft size={16} />}
        >
          Back to dashboard
        </Button>
      )}
      rightSlot={<UserMenu userName={userName} onLogout={handleLogout} onDelete={handleDelete} />}
    >
      <DateRangeFilter todayLabel={todayLabel} range={range} onChange={handleRangeChange} />
      {rangeError ? (
        <Text size="sm" c="red" className={styles.filterError}>
          {rangeError}
        </Text>
      ) : null}

      <PageBlock variant="section">
        <CaloriesChartSection
          chartMode={chartMode}
          onChartModeChange={setChartMode}
          dailySeries={dailySeries}
          chartSeries={chartSeries}
          xAxisProps={xAxisProps}
          yAxisProps={yAxisProps}
          calorieStats={calorieStats}
          calorieChartStatus={calorieChartStatus}
        />
      </PageBlock>

      <PageBlock variant="section">
        <CalorieEntryCard
          title="Log calories"
          date={entryDate}
          onDateChange={setEntryDate}
          allowDateChange
          onSaved={refreshCalories}
        />
      </PageBlock>

      <PageBlock variant="section">
        <EntriesTable
          title="Entries"
          countLabel="entries"
          entries={sortedEntries}
          isLoading={isLoadingRange}
          loadingMessage="Loading calories..."
          emptyMessage="No calories recorded for this range."
          columns={['Date', 'Calories', 'Food', 'Part of day', 'Note', '']}
          renderRow={(entry) => (
            <Table.Tr key={entry.id}>
              <Table.Td>{formatDisplayDate(entry.date)}</Table.Td>
              <Table.Td>{entry.calories.toFixed(0)}</Table.Td>
              <Table.Td>{entry.foodName ?? '--'}</Table.Td>
              <Table.Td>{entry.partOfDay ?? '--'}</Table.Td>
              <Table.Td>{entry.note ?? '--'}</Table.Td>
              <Table.Td align="right">
                <Button
                  size="xs"
                  variant="subtle"
                  color="red"
                  leftSection={<IconTrash size={14} />}
                  onClick={() => handleDeleteEntry(entry.id)}
                  loading={deletingId === entry.id}
                >
                  Delete
                </Button>
              </Table.Td>
            </Table.Tr>
          )}
        />
      </PageBlock>
    </PageShell>
  );
};
