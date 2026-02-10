import {
  Avatar,
  Badge,
  Button,
  Card,
  Container,
  Group,
  Menu,
  Stack,
  Table,
  Text,
  Title,
} from '@mantine/core';
import { AreaChart } from '@mantine/charts';
import { DatePickerInput, type DatesRangeValue } from '@mantine/dates';
import { IconArrowLeft, IconChevronDown, IconLogout, IconTrash } from '@tabler/icons-react';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { TooltipProps } from 'recharts';
import { useAuth } from '../auth/auth-context';
import { useDocumentTitle } from '../hooks/use-document-title';
import { deleteWeight, getWeightRange } from '../weights/weights-api';
import { WeightEntryCard } from '../weights/weight-entry-card';
import type { WeightEntry } from '../weights/weights-types';
import {
  clampDateRange,
  formatAxisDate,
  formatDisplayDate,
  getDefaultTwoWeekRange,
  parseDateInputValue,
  startOfToday,
  sortWeightsByDate,
  toDateInputValue,
} from '../weights/weights-utils';
import styles from './weights-page.module.css';

export const WeightsPage = () => {
  useDocumentTitle('HMMH (Weights)');
  const { userName, signOut, deleteAccount, token } = useAuth();
  const navigate = useNavigate();
  const { start, end } = useMemo(() => getDefaultTwoWeekRange(), []);
  const [startDate, setStartDate] = useState(start);
  const [endDate, setEndDate] = useState(end);
  const [range, setRange] = useState<DatesRangeValue<string>>([
    toDateInputValue(start),
    toDateInputValue(end),
  ]);
  const [entryDate, setEntryDate] = useState(startOfToday());
  const [weights, setWeights] = useState<WeightEntry[]>([]);
  const [isLoadingRange, setIsLoadingRange] = useState(false);
  const [rangeError, setRangeError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const todayLabel = useMemo(
    () => formatDisplayDate(toDateInputValue(startOfToday())),
    [],
  );

  const initial = userName?.[0]?.toUpperCase() ?? 'U';
  const chartWeights = useMemo(() => sortWeightsByDate(weights), [weights]);
  const sortedWeights = useMemo<WeightEntry[]>(
    () => sortWeightsByDate(weights).slice().reverse(),
    [weights],
  );
  const weightValues = useMemo(() => chartWeights.map(entry => entry.weightKg), [chartWeights]);
  const chartData = useMemo(
    () => chartWeights.map(entry => ({ date: entry.date, weight: entry.weightKg })),
    [chartWeights],
  );
  const hasWeightData = chartWeights.length > 0;
  const hasTrendData = weightValues.length > 1;
  const averageWeight = useMemo(
    () => (hasWeightData ? weightValues.reduce((sum, value) => sum + value, 0) / weightValues.length : null),
    [hasWeightData, weightValues],
  );
  const minWeight = useMemo(() => (hasWeightData ? Math.min(...weightValues) : null), [hasWeightData, weightValues]);
  const maxWeight = useMemo(() => (hasWeightData ? Math.max(...weightValues) : null), [hasWeightData, weightValues]);
  const xAxisProps = useMemo(() => ({ tickFormatter: formatAxisDate }), []);
  const yAxisProps = useMemo(() => {
    if (minWeight === null || maxWeight === null) {
      return {};
    }

    const range = maxWeight - minWeight;
    const padding = Math.max(range > 0 ? range * 0.05 : minWeight * 0.01, 0.5);

    return {
      domain: [minWeight - padding, maxWeight + padding],
    };
  }, [minWeight, maxWeight]);

  const renderWeightTooltip = ({ payload }: TooltipProps<number, string>) => {
    const rawValue = payload?.[0]?.value;
    const numericValue = Number(rawValue);

    if (!Number.isFinite(numericValue)) {
      return null;
    }

    return <div className={styles.tooltip}>{numericValue.toFixed(1)} kg</div>;
  };

  const loadRange = useCallback(async () => {
    if (!token) {
      return;
    }

    setIsLoadingRange(true);
    setRangeError(null);

    try {
      const response = await getWeightRange(token, toDateInputValue(startDate), toDateInputValue(endDate));
      setWeights(response);
    } catch (loadError) {
      const message = loadError instanceof Error ? loadError.message : 'Failed to load weights.';
      setRangeError(message);
    } finally {
      setIsLoadingRange(false);
    }
  }, [token, startDate, endDate]);

  useEffect(() => {
    void loadRange();
  }, [loadRange]);

  const handleRangeChange = (value: DatesRangeValue<string>) => {
    setRange(value);

    const startValue = value[0] ? parseDateInputValue(value[0]) : null;
    const endValue = value[1] ? parseDateInputValue(value[1]) : null;

    if (startValue && endValue) {
      const normalized = clampDateRange(startValue, endValue);
      setStartDate(normalized.start);
      setEndDate(normalized.end);
    }
  };

  const handleLogout = () => {
    signOut();
    navigate('/');
  };

  const handleDelete = async () => {
    await deleteAccount();
    navigate('/');
  };

  const handleDeleteEntry = async (id: string) => {
    if (!token) {
      return;
    }

    setDeletingId(id);
    setRangeError(null);

    try {
      await deleteWeight(token, id);
      await loadRange();
    } catch (deleteError) {
      const message = deleteError instanceof Error ? deleteError.message : 'Failed to delete weight.';
      setRangeError(message);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className={styles.page}>
      <Container size="lg" className={styles.container}>
        <Group justify="space-between" className={styles.header}>
          <Button
            variant="subtle"
            onClick={() => navigate('/dashboard')}
            className={styles.backButton}
            leftSection={<IconArrowLeft size={16} />}
          >
            Back to dashboard
          </Button>
          <Title order={2} className={styles.title}>
            My weight history
          </Title>
          <Menu width={200} position="bottom-end" shadow="md">
            <Menu.Target>
              <Button variant="subtle" rightSection={<IconChevronDown size={16} />} className={styles.userButton}>
                <Group gap="sm">
                  <Avatar radius="xl" color="teal">
                    {initial}
                  </Avatar>
                  <Text fw={600}>{userName ?? 'User'}</Text>
                </Group>
              </Button>
            </Menu.Target>
            <Menu.Dropdown>
              <Menu.Item leftSection={<IconLogout size={16} />} onClick={handleLogout}>
                Logout
              </Menu.Item>
              <Menu.Item color="red" leftSection={<IconTrash size={16} />} onClick={handleDelete}>
                Delete account
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>

        <div className={styles.filterRow}>
          <Text size="sm" c="dimmed" className={styles.currentDate}>
            Today: {todayLabel}
          </Text>
          <div className={styles.filterControls}>
            <Text size="sm" c="dimmed" className={styles.filterLabel}>
              Selected period
            </Text>
            <DatePickerInput
              type="range"
              value={range}
              onChange={handleRangeChange}
              className={styles.filterInput}
              placeholder="Pick dates"
              allowSingleDateInRange
              label=""
              valueFormat="YYYY-MM-DD"
            />
          </div>
        </div>
        {rangeError ? (
          <Text size="sm" c="red" className={styles.filterError}>
            {rangeError}
          </Text>
        ) : null}

        <div className={styles.section}>
          <Card withBorder radius="lg" className={styles.chartCard}>
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Weight trend</Text>
              </Group>
              <div className={styles.sparkline}>
                {isLoadingRange ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    Loading weights...
                  </Text>
                ) : !hasWeightData ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    No weights recorded for this range.
                  </Text>
                ) : !hasTrendData ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    Add another entry to see the trend line.
                  </Text>
                ) : (
                  <AreaChart
                    h={120}
                    w="100%"
                    data={chartData}
                    dataKey="date"
                    series={[{ name: 'weight', color: 'teal.6' }]}
                    curveType="monotone"
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
                    unit="kg"
                    tooltipProps={{ content: renderWeightTooltip }}
                  />
                )}
              </div>
              <Group justify="space-between" className={styles.statRow}>
                <div>
                  <Text size="sm" c="dimmed">
                    Min weight
                  </Text>
                  <Text fw={600}>
                    {minWeight !== null ? `${minWeight.toFixed(1)} kg` : '--'}
                  </Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Max weight
                  </Text>
                  <Text fw={600}>
                    {maxWeight !== null ? `${maxWeight.toFixed(1)} kg` : '--'}
                  </Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Avg weight
                  </Text>
                  <Text fw={600}>
                    {averageWeight !== null ? `${averageWeight.toFixed(1)} kg` : '--'}
                  </Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Entries
                  </Text>
                  <Text fw={600}>{sortedWeights.length}</Text>
                </div>
              </Group>
            </Stack>
          </Card>
        </div>

        <div className={styles.section}>
          <WeightEntryCard
            date={entryDate}
            onDateChange={setEntryDate}
            allowDateChange
            onSaved={loadRange}
          />
        </div>

        <div className={styles.section}>
          <Card withBorder radius="lg" className={styles.tableCard}>
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Weights</Text>
                <Badge color="teal" variant="light">
                  {sortedWeights.length} entries
                </Badge>
              </Group>
              {isLoadingRange ? (
                <Text size="sm" c="dimmed" className={styles.emptyState}>
                  Loading weights...
                </Text>
              ) : sortedWeights.length === 0 ? (
                <Text size="sm" c="dimmed" className={styles.emptyState}>
                  No weights recorded for this range.
                </Text>
              ) : (
                <Table striped highlightOnHover>
                  <Table.Thead>
                    <Table.Tr>
                      <Table.Th>Date</Table.Th>
                      <Table.Th>Weight (kg)</Table.Th>
                      <Table.Th />
                    </Table.Tr>
                  </Table.Thead>
                  <Table.Tbody>
                    {sortedWeights.map((entry) => (
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
                    ))}
                  </Table.Tbody>
                </Table>
              )}
            </Stack>
          </Card>
        </div>
      </Container>
    </div>
  );
};
