import {
  Avatar,
  Badge,
  Button,
  Card,
  Container,
  Group,
  Menu,
  SimpleGrid,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import { AreaChart } from '@mantine/charts';
import { DatePickerInput, type DatesRangeValue } from '@mantine/dates';
import { IconChevronDown, IconLogout, IconTrash } from '@tabler/icons-react';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { TooltipProps } from 'recharts';
import { useAuth } from '../auth/auth-context';
import { getCalorieRange } from '../calories/calories-api';
import { CalorieTodayCard } from '../calories/calorie-today-card';
import type { CalorieEntry } from '../calories/calories-types';
import { buildDailyCalorieTotals } from '../calories/calories-utils';
import type { WeightEntry } from '../weights/weights-types';
import { getWeightRange } from '../weights/weights-api';
import { WeightTodayCard } from '../weights/weight-today-card';
import {
  clampDateRange,
  formatAxisDate,
  formatRangeLabel,
  getDefaultDashboardRange,
  parseDateInputValue,
  sortWeightsByDate,
  toDateInputValue,
} from '../weights/weights-utils';
import styles from './dashboard-page.module.css';

const renderWeightTooltip = ({ payload }: TooltipProps<number, string>) => {
  const rawValue = payload?.[0]?.value;
  const numericValue = Number(rawValue);

  if (!Number.isFinite(numericValue)) {
    return null;
  }

  return <div className={styles.tooltip}>{numericValue.toFixed(1)} kg</div>;
};

export const DashboardPage = () => {
  const { userName, signOut, deleteAccount, token } = useAuth();
  const navigate = useNavigate();
  const { start, end } = useMemo(() => getDefaultDashboardRange(), []);
  const [startDate, setStartDate] = useState(start);
  const [endDate, setEndDate] = useState(end);
  const [range, setRange] = useState<DatesRangeValue<string>>([
    toDateInputValue(start),
    toDateInputValue(end),
  ]);
  const [weights, setWeights] = useState<WeightEntry[]>([]);
  const [calories, setCalories] = useState<CalorieEntry[]>([]);
  const [isLoadingRange, setIsLoadingRange] = useState(false);
  const [isLoadingCalories, setIsLoadingCalories] = useState(false);
  const [rangeError, setRangeError] = useState<string | null>(null);
  const [caloriesError, setCaloriesError] = useState<string | null>(null);

  const initial = userName?.[0]?.toUpperCase() ?? 'U';

  const sortedWeights = useMemo(() => sortWeightsByDate(weights), [weights]);
  const weightValues = useMemo(() => sortedWeights.map(entry => entry.weightKg), [sortedWeights]);
  const chartData = useMemo(
    () => sortedWeights.map(entry => ({ date: entry.date, weight: entry.weightKg })),
    [sortedWeights],
  );
  const hasWeightData = sortedWeights.length > 0;
  const hasTrendData = weightValues.length > 1;
  const rangeLabel = useMemo(() => formatRangeLabel(startDate, endDate), [startDate, endDate]);
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

  const dailyCalories = useMemo(
    () => buildDailyCalorieTotals(calories, startDate, endDate),
    [calories, startDate, endDate],
  );
  const calorieTotals = useMemo(() => dailyCalories.map(entry => entry.calories), [dailyCalories]);
  const calorieTotalsWithData = useMemo(
    () => calorieTotals.filter(total => total > 0),
    [calorieTotals],
  );
  const hasCalorieData = calories.length > 0;
  const hasCalorieTrend = calorieTotalsWithData.length > 1;
  const totalCalories = useMemo(
    () => calorieTotals.reduce((sum, value) => sum + value, 0),
    [calorieTotals],
  );
  const averageCalories = useMemo(
    () => (hasCalorieData && dailyCalories.length > 0 ? totalCalories / dailyCalories.length : null),
    [dailyCalories.length, hasCalorieData, totalCalories],
  );
  const minCalories = useMemo(
    () => (calorieTotalsWithData.length ? Math.min(...calorieTotalsWithData) : null),
    [calorieTotalsWithData],
  );
  const maxCalories = useMemo(
    () => (calorieTotalsWithData.length ? Math.max(...calorieTotalsWithData) : null),
    [calorieTotalsWithData],
  );

  const renderCaloriesTooltip = ({ payload }: TooltipProps<number, string>) => {
    const rawValue = payload?.[0]?.value;
    const numericValue = Number(rawValue);

    if (!Number.isFinite(numericValue)) {
      return null;
    }

    return <div className={styles.tooltip}>{`${numericValue.toFixed(0)} kcal`}</div>;
  };

  const loadRange = useCallback(async () => {
    if (!token) {
      return;
    }

    setIsLoadingRange(true);
    setRangeError(null);

    try {
      const startValue = toDateInputValue(startDate);
      const endValue = toDateInputValue(endDate);
      const response = await getWeightRange(token, startValue, endValue);
      setWeights(response);
    } catch (loadError) {
      const message = loadError instanceof Error ? loadError.message : 'Failed to load weights.';
      setRangeError(message);
    } finally {
      setIsLoadingRange(false);
    }
  }, [token, startDate, endDate]);

  const loadCaloriesRange = useCallback(async () => {
    if (!token) {
      return;
    }

    setIsLoadingCalories(true);
    setCaloriesError(null);

    try {
      const startValue = toDateInputValue(startDate);
      const endValue = toDateInputValue(endDate);
      const response = await getCalorieRange(token, startValue, endValue);
      setCalories(response);
    } catch (loadError) {
      const message = loadError instanceof Error ? loadError.message : 'Failed to load calories.';
      setCaloriesError(message);
    } finally {
      setIsLoadingCalories(false);
    }
  }, [token, startDate, endDate]);

  useEffect(() => {
    void loadRange();
  }, [loadRange]);

  useEffect(() => {
    void loadCaloriesRange();
  }, [loadCaloriesRange]);

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

  const handleWeightsNavigate = () => {
    navigate('/weights');
  };

  const handleCaloriesNavigate = () => {
    navigate('/calories');
  };

  return (
    <div className={styles.page}>
      <Container size="lg" className={styles.container}>
        <Group justify="space-between" className={styles.header}>
          <div className={styles.headerSpacer} />
          <Title order={2} className={styles.title}>
            Help me manage health
          </Title>
          <Menu width={200} position="bottom-end" shadow="md">
            <Menu.Target>
              <Button variant="subtle" rightSection={<IconChevronDown size={16} />}
                className={styles.userButton}
              >
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
              <Menu.Item
                color="red"
                leftSection={<IconTrash size={16} />}
                onClick={handleDelete}
              >
                Delete account
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>

        <div className={styles.filterRow}>
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

        <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg" className={styles.grid}>
          <Card
            withBorder
            radius="lg"
            className={`${styles.card} ${styles.clickableCard}`}
            onClick={handleWeightsNavigate}
            onKeyDown={(event) => {
              if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                handleWeightsNavigate();
              }
            }}
            role="button"
            tabIndex={0}
          >
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Weight trend</Text>
                <Badge color="teal" variant="light">
                  {rangeLabel}
                </Badge>
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
                    Entries
                  </Text>
                  <Text fw={600}>{sortedWeights.length}</Text>
                </div>
              </Group>
            </Stack>
          </Card>

          <Card
            withBorder
            radius="lg"
            className={`${styles.card} ${styles.clickableCard}`}
            onClick={handleCaloriesNavigate}
            onKeyDown={(event) => {
              if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                handleCaloriesNavigate();
              }
            }}
            role="button"
            tabIndex={0}
          >
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Daily calories</Text>
                <Badge color="teal" variant="light">
                  {averageCalories !== null ? `avg ${averageCalories.toFixed(0)}` : '--'}
                </Badge>
              </Group>
              <div className={styles.sparkline}>
                {isLoadingCalories ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    Loading calories...
                  </Text>
                ) : !hasCalorieData ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    No calories recorded for this range.
                  </Text>
                ) : !hasCalorieTrend ? (
                  <Text size="sm" c="dimmed" className={styles.sparklineEmpty}>
                    Add another day to see the trend line.
                  </Text>
                ) : (
                  <AreaChart
                    h={120}
                    w="100%"
                    data={dailyCalories}
                    dataKey="date"
                    series={[{ name: 'calories', color: 'teal.6' }]}
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
                    Entries
                  </Text>
                  <Text fw={600}>{calories.length}</Text>
                </div>
              </Group>
            </Stack>
          </Card>
        </SimpleGrid>

        <div className={styles.section}>
          <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg">
            <WeightTodayCard onSaved={loadRange} />
            <CalorieTodayCard onSaved={loadCaloriesRange} />
          </SimpleGrid>
        </div>
      </Container>
    </div>
  );
};
