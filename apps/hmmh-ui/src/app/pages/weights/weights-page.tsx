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
import { WeightEntryCard } from '../../shared/weight-entry-card/weight-entry-card';
import { formatDisplayDate } from '../../utils/global-utils';
import styles from './weights-page.module.css';
import { useWeightsPageData } from '../../hooks/use-weights-page-data';
import { WeightsChartSection } from './components/weights-chart-section/weights-chart-section';

export const WeightsPage = () => {
  useDocumentTitle('HMMH (Weights)');
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
    chartData,
    xAxisProps,
    yAxisProps,
    weightStats,
    weightChartStatus,
    sortedWeights,
    handleDeleteEntry,
    refreshWeights,
  } = useWeightsPageData();

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
      title="My weight history"
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
        <WeightsChartSection
          chartData={chartData}
          xAxisProps={xAxisProps}
          yAxisProps={yAxisProps}
          weightStats={weightStats}
          weightChartStatus={weightChartStatus}
        />
      </PageBlock>

      <PageBlock variant="section">
        <WeightEntryCard
          title="Enter weight"
          date={entryDate}
          onDateChange={setEntryDate}
          allowDateChange
          onSaved={refreshWeights}
        />
      </PageBlock>

      <PageBlock variant="section">
        <EntriesTable
          title="Weights"
          countLabel="entries"
          entries={sortedWeights}
          isLoading={isLoadingRange}
          loadingMessage="Loading weights..."
          emptyMessage="No weights recorded for this range."
          columns={['Date', 'Weight (kg)', '']}
          renderRow={(entry) => (
            <Table.Tr key={entry.id}>
              <Table.Td>{formatDisplayDate(entry.date)}</Table.Td>
              <Table.Td>{entry.weightKg.toFixed(1)}</Table.Td>
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
