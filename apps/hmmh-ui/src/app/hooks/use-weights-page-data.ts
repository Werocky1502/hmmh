import { useCallback, useEffect, useMemo, useState } from 'react';
import type { DatesRangeValue } from '@mantine/dates';
import { useAuth } from '../auth/auth-context';
import { deleteWeight, getWeightRange } from '../api/weights-api';
import type { WeightEntry } from '../api/contracts/weights-types';
import {
  clampDateRange,
  formatAxisDate,
  formatDisplayDate,
  getDefaultTwoWeekRange,
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

export const useWeightsPageData = () => {
  const { getAccessToken } = useAuth();
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

  const weightChartStatus: ChartStatusInfo = isLoadingRange
    ? { status: 'loading', message: 'Loading weights...' }
    : !hasWeightData
      ? { status: 'empty', message: 'No weights recorded for this range.' }
      : !hasTrendData
        ? { status: 'no-data', message: 'Add another entry to see the trend line.' }
        : { status: 'ready', message: '' };

  const weightStats: StatItem[] = [
    { label: 'Min weight', value: minWeight !== null ? `${minWeight.toFixed(1)} kg` : '--' },
    { label: 'Max weight', value: maxWeight !== null ? `${maxWeight.toFixed(1)} kg` : '--' },
    { label: 'Avg weight', value: averageWeight !== null ? `${averageWeight.toFixed(1)} kg` : '--' },
    { label: 'Entries', value: sortedWeights.length },
  ];

  const loadRange = useCallback(async () => {
    const token = await getAccessToken();
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
      await deleteWeight(token, id);
      await loadRange();
    } catch (deleteError) {
      const message = deleteError instanceof Error ? deleteError.message : 'Failed to delete weight.';
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
    chartData,
    xAxisProps,
    yAxisProps,
    weightStats,
    weightChartStatus,
    sortedWeights,
    handleDeleteEntry,
    refreshWeights: loadRange,
  };
};
