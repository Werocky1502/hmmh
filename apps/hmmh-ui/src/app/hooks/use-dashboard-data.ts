import { useCallback, useEffect, useMemo, useState } from 'react';
import type { DatesRangeValue } from '@mantine/dates';
import { useAuth } from '../auth/auth-context';
import { getCalorieRange } from '../api/calories-api';
import type { CalorieEntry } from '../api/contracts/calories-types';
import { buildDailyCalorieTotals } from '../utils/calories-utils';
import { getWeightRange } from '../api/weights-api';
import type { WeightEntry } from '../api/contracts/weights-types';
import {
  clampDateRange,
  formatAxisDate,
  formatDisplayDate,
  getDefaultDashboardRange,
  parseDateInputValue,
  startOfToday,
  toDateInputValue,
} from '../utils/global-utils';
import { sortWeightsByDate } from '../utils/weights-utils';

type ChartStatus = 'loading' | 'empty' | 'no-data' | 'ready';

type ChartStatusInfo = {
  status: ChartStatus;
  message: string;
};

export type StatItem = {
  label: string;
  value: string | number;
};

export const useDashboardData = () => {
  const { getAccessToken } = useAuth();
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
  const todayLabel = useMemo(
    () => formatDisplayDate(toDateInputValue(startOfToday())),
    [],
  );

  const sortedWeights = useMemo(() => sortWeightsByDate(weights), [weights]);
  const weightValues = useMemo(() => sortedWeights.map(entry => entry.weightKg), [sortedWeights]);
  const weightChartData = useMemo(
    () => sortedWeights.map(entry => ({ date: entry.date, weight: entry.weightKg })),
    [sortedWeights],
  );
  const hasWeightData = sortedWeights.length > 0;
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

  const weightChartStatus: ChartStatusInfo = isLoadingRange
    ? { status: 'loading', message: 'Loading weights...' }
    : !hasWeightData
      ? { status: 'empty', message: 'No weights recorded for this range.' }
      : !hasTrendData
        ? { status: 'no-data', message: 'Add another entry to see the trend line.' }
        : { status: 'ready', message: '' };

  const calorieChartStatus: ChartStatusInfo = isLoadingCalories
    ? { status: 'loading', message: 'Loading calories...' }
    : !hasCalorieData
      ? { status: 'empty', message: 'No calories recorded for this range.' }
      : !hasCalorieTrend
        ? { status: 'no-data', message: 'Add another day to see the trend line.' }
        : { status: 'ready', message: '' };

  const weightStats: StatItem[] = [
    { label: 'Min weight', value: minWeight !== null ? `${minWeight.toFixed(1)} kg` : '--' },
    { label: 'Max weight', value: maxWeight !== null ? `${maxWeight.toFixed(1)} kg` : '--' },
    { label: 'Avg weight', value: averageWeight !== null ? `${averageWeight.toFixed(1)} kg` : '--' },
    { label: 'Entries', value: sortedWeights.length },
  ];

  const calorieStats: StatItem[] = [
    { label: 'Lowest day', value: minCalories !== null ? `${minCalories.toFixed(0)} kcal` : '--' },
    { label: 'Highest day', value: maxCalories !== null ? `${maxCalories.toFixed(0)} kcal` : '--' },
    { label: 'Avg calories', value: averageCalories !== null ? `${averageCalories.toFixed(0)} kcal` : '--' },
    { label: 'Entries', value: calories.length },
  ];

  const loadRange = useCallback(async () => {
    const token = await getAccessToken();
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
  }, [getAccessToken, startDate, endDate]);

  const loadCaloriesRange = useCallback(async () => {
    const token = await getAccessToken();
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
  }, [getAccessToken, startDate, endDate]);

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

  return {
    range,
    handleRangeChange,
    todayLabel,
    rangeError,
    caloriesError,
    weightChartData,
    weightStats,
    weightChartStatus,
    calorieChartData: dailyCalories,
    calorieStats,
    calorieChartStatus,
    xAxisProps,
    yAxisProps,
    refreshWeights: loadRange,
    refreshCalories: loadCaloriesRange,
  };
};
