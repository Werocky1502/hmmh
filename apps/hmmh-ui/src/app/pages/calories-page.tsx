import {
  Avatar,
  Badge,
  Button,
  Card,
  Container,
  Group,
  Menu,
  SegmentedControl,
  Stack,
  Table,
  Text,
  Title,
} from '@mantine/core';
import { LineChart } from '@mantine/charts';
import { DatePickerInput, type DatesRangeValue } from '@mantine/dates';
import { IconArrowLeft, IconChevronDown, IconLogout, IconTrash } from '@tabler/icons-react';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { TooltipProps } from 'recharts';
import { useAuth } from '../auth/auth-context';
import { useDocumentTitle } from '../hooks/use-document-title';
import { deleteCalorie, getCalorieRange } from '../calories/calories-api';
import { CalorieEntryCard } from '../calories/calorie-entry-card';
import type { CalorieEntry } from '../calories/calories-types';
import {
  buildDailyCalorieSeries,
  clampDateRange,
  formatAxisDate,
  formatDisplayDate,
  getDefaultTwoWeekRange,
  parseDateInputValue,
  startOfToday,
  toDateInputValue,
} from '../calories/calories-utils';
import styles from './calories-page.module.css';

export const CaloriesPage = () => {
  useDocumentTitle('HMMH (Calories)');
  const { userName, signOut, deleteAccount, getAccessToken } = useAuth();
  const navigate = useNavigate();
  const { start, end } = useMemo(() => getDefaultTwoWeekRange(), []);
  const [startDate, setStartDate] = useState(start);
  const [endDate, setEndDate] = useState(end);
  const [range, setRange] = useState<DatesRangeValue<string>>([
    toDateInputValue(start),
    toDateInputValue(end),
  ]);
  const [entryDate, setEntryDate] = useState(startOfToday());
  const [calories, setCalories] = useState<CalorieEntry[]>([]);
  const [isLoadingRange, setIsLoadingRange] = useState(false);
  const [rangeError, setRangeError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [chartMode, setChartMode] = useState<'total' | 'parts'>('total');
  const todayLabel = useMemo(
    () => formatDisplayDate(toDateInputValue(startOfToday())),
    [],
  );

  const initial = userName?.[0]?.toUpperCase() ?? 'U';
  const dailySeries = useMemo(
    () => buildDailyCalorieSeries(calories, startDate, endDate),
    [calories, startDate, endDate],
  );
  const totalsWithData = useMemo(
    () => dailySeries.filter(entry => entry.total > 0),
    [dailySeries],
  );
  const sortedEntries = useMemo(
    () =>
      [...calories].sort((left, right) => {
        if (left.date === right.date) {
          return left.id.localeCompare(right.id);
        }

        return right.date.localeCompare(left.date);
      }),
    [calories],
  );
  const calorieValues = useMemo(() => totalsWithData.map(entry => entry.total), [totalsWithData]);
  const hasCalorieData = calories.length > 0;
  const hasChartData = totalsWithData.length > 0;
  const averageCalories = useMemo(
    () => (calorieValues.length ? calorieValues.reduce((sum, value) => sum + value, 0) / calorieValues.length : null),
    [calorieValues],
  );
  const minCalories = useMemo(
    () => (calorieValues.length ? Math.min(...calorieValues) : null),
    [calorieValues],
  );
  const maxCalories = useMemo(
    () => (calorieValues.length ? Math.max(...calorieValues) : null),
    [calorieValues],
  );
  const xAxisProps = useMemo(() => ({ tickFormatter: formatAxisDate }), []);
  const yAxisProps = useMemo(() => {
    if (minCalories === null || maxCalories === null) {
      return {};
    }

    const range = maxCalories - minCalories;
    const padding = Math.max(range > 0 ? range * 0.05 : maxCalories * 0.05, 20);

    return {
      domain: [Math.max(minCalories - padding, 0), maxCalories + padding],
    };
  }, [minCalories, maxCalories]);

  const partTotals = useMemo(() => {
    return dailySeries.reduce(
      (acc, entry) => {
        acc.morning += entry.morning;
        acc.midday += entry.midday;
        acc.evening += entry.evening;
        acc.night += entry.night;
        return acc;
      },
      { morning: 0, midday: 0, evening: 0, night: 0 },
    );
  }, [dailySeries]);

  const chartSeries = useMemo(
    () =>
      chartMode === 'parts'
        ? [
            partTotals.morning > 0 ? { name: 'morning', color: 'cyan.6' } : null,
            partTotals.midday > 0 ? { name: 'midday', color: 'lime.6' } : null,
            partTotals.evening > 0 ? { name: 'evening', color: 'orange.6' } : null,
            partTotals.night > 0 ? { name: 'night', color: 'blue.6' } : null,
          ]
            .filter((entry): entry is { name: string; color: string } => entry !== null)
        : [{ name: 'total', color: 'teal.6' }],
    [chartMode, partTotals],
  );

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
      <div className={styles.tooltip}>
        {rows.map((row) => (
          <div key={row.key} className={styles.tooltipRow}>
            <span>{labels[row.key] ?? row.key}</span>
            <span>{`${row.value.toFixed(0)} kcal`}</span>
          </div>
        ))}
      </div>
    );
  };

  const loadRange = useCallback(async () => {
    const token = await getAccessToken();
    if (!token) {
      return;
    }

    setIsLoadingRange(true);
    setRangeError(null);

    try {
      const response = await getCalorieRange(token, toDateInputValue(startDate), toDateInputValue(endDate));
      setCalories(response);
    } catch (loadError) {
      const message = loadError instanceof Error ? loadError.message : 'Failed to load calories.';
      setRangeError(message);
    } finally {
      setIsLoadingRange(false);
    }
  }, [getAccessToken, startDate, endDate]);

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
    const token = await getAccessToken();
    if (!token) {
      return;
    }

    setDeletingId(id);
    setRangeError(null);

    try {
      await deleteCalorie(token, id);
      await loadRange();
    } catch (deleteError) {
      const message = deleteError instanceof Error ? deleteError.message : 'Failed to delete entry.';
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
            My calorie history
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
                <Text fw={600}>Daily calories</Text>
                <SegmentedControl
                  size="xs"
                  value={chartMode}
                  onChange={(value) => setChartMode(value as 'total' | 'parts')}
                  data={[
                    { label: 'Total', value: 'total' },
                    { label: 'By day part', value: 'parts' },
                  ]}
                />
              </Group>
              <div className={styles.sparkline}>
                {isLoadingRange ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    Loading calories...
                  </Text>
                ) : !hasCalorieData ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    No calories recorded for this range.
                  </Text>
                ) : chartSeries.length === 0 ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    No part-of-day calories in this range.
                  </Text>
                ) : !hasChartData ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    No calorie totals available for this range.
                  </Text>
                ) : (
                  <LineChart
                    h={120}
                    w="100%"
                    data={dailySeries}
                    dataKey="date"
                    series={chartSeries}
                    curveType="monotone"
                    withDots
                    dotProps={{ r: 3 }}
                    activeDotProps={{ r: 5 }}
                    strokeWidth={3}
                    gridAxis="x"
                    withXAxis
                    withYAxis
                    xAxisProps={xAxisProps}
                    yAxisProps={yAxisProps}
                    unit="kcal"
                    tooltipProps={{ content: renderCaloriesTooltip }}
                  />
                )}
              </div>
              <Group justify="space-between" className={styles.statRow}>
                <div>
                  <Text size="sm" c="dimmed">
                    Lowest day
                  </Text>
                  <Text fw={600}>{minCalories !== null ? `${minCalories.toFixed(0)} kcal` : '--'}</Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Highest day
                  </Text>
                  <Text fw={600}>{maxCalories !== null ? `${maxCalories.toFixed(0)} kcal` : '--'}</Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Avg calories
                  </Text>
                  <Text fw={600}>
                    {averageCalories !== null ? `${averageCalories.toFixed(0)} kcal` : '--'}
                  </Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Entries
                  </Text>
                  <Text fw={600}>{sortedEntries.length}</Text>
                </div>
              </Group>
            </Stack>
          </Card>
        </div>

        <div className={styles.section}>
          <CalorieEntryCard date={entryDate} onDateChange={setEntryDate} allowDateChange onSaved={loadRange} />
        </div>

        <div className={styles.section}>
          <Card withBorder radius="lg" className={styles.tableCard}>
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Entries</Text>
                <Badge color="teal" variant="light">
                  {sortedEntries.length} entries
                </Badge>
              </Group>
              {isLoadingRange ? (
                <Text size="sm" c="dimmed" className={styles.emptyState}>
                  Loading calories...
                </Text>
              ) : sortedEntries.length === 0 ? (
                <Text size="sm" c="dimmed" className={styles.emptyState}>
                  No calories recorded for this range.
                </Text>
              ) : (
                <Table striped highlightOnHover>
                  <Table.Thead>
                    <Table.Tr>
                      <Table.Th>Date</Table.Th>
                      <Table.Th>Calories</Table.Th>
                      <Table.Th>Food</Table.Th>
                      <Table.Th>Part of day</Table.Th>
                      <Table.Th>Note</Table.Th>
                      <Table.Th />
                    </Table.Tr>
                  </Table.Thead>
                  <Table.Tbody>
                    {sortedEntries.map((entry) => (
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
