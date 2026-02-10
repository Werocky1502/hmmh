import { useCallback, useEffect, useMemo, useState } from 'react';
import type { DatesRangeValue } from '@mantine/dates';
import { useAuth } from '../auth/auth-context';
import { deleteCalorie, getCalorieRange } from '../api/calories-api';
import type { CalorieEntry } from '../api/contracts/calories-types';
import { buildDailyCalorieSeries } from '../utils/calories-utils';
import {
  clampDateRange,
  formatAxisDate,
  formatDisplayDate,
  getDefaultTwoWeekRange,
  parseDateInputValue,
  startOfToday,
  toDateInputValue,
} from '../utils/global-utils';

type ChartStatus = 'loading' | 'empty' | 'no-data' | 'ready';

type ChartStatusInfo = {
  status: ChartStatus;
  message: string;
};

export type StatItem = {
  label: string;
  value: string | number;
};

export const useCaloriesPageData = () => {
  const { getAccessToken } = useAuth();
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
          ].filter((entry): entry is { name: string; color: string } => entry !== null)
        : [{ name: 'total', color: 'teal.6' }],
    [chartMode, partTotals],
  );

  const calorieChartStatus: ChartStatusInfo = isLoadingRange
    ? { status: 'loading', message: 'Loading calories...' }
    : !hasCalorieData
      ? { status: 'empty', message: 'No calories recorded for this range.' }
      : chartSeries.length === 0
        ? { status: 'no-data', message: 'No part-of-day calories in this range.' }
        : !hasChartData
          ? { status: 'no-data', message: 'No calorie totals available for this range.' }
          : { status: 'ready', message: '' };

  const calorieStats: StatItem[] = [
    { label: 'Lowest day', value: minCalories !== null ? `${minCalories.toFixed(0)} kcal` : '--' },
    { label: 'Highest day', value: maxCalories !== null ? `${maxCalories.toFixed(0)} kcal` : '--' },
    { label: 'Avg calories', value: averageCalories !== null ? `${averageCalories.toFixed(0)} kcal` : '--' },
    { label: 'Entries', value: sortedEntries.length },
  ];

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

  return {
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
    refreshCalories: loadRange,
  };
};
