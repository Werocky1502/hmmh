import type { CalorieEntry } from './calories-types';

const pad = (value: number) => value.toString().padStart(2, '0');

export const startOfToday = () => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return today;
};

export const toDateInputValue = (date: Date) => {
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

export const parseDateInputValue = (value: string) => {
  if (!value) {
    return null;
  }

  const [year, month, day] = value.split('-').map(Number);
  if (!year || !month || !day) {
    return null;
  }

  return new Date(year, month - 1, day);
};

export const getDefaultDashboardRange = () => {
  const end = startOfToday();
  const start = new Date(end);
  start.setDate(end.getDate() - 6);
  return { start, end };
};

export const getDefaultMonthlyRange = () => {
  const end = startOfToday();
  const start = new Date(end);
  start.setMonth(end.getMonth() - 1);
  return { start, end };
};

export const getDefaultTwoWeekRange = () => {
  const end = startOfToday();
  const start = new Date(end);
  start.setDate(end.getDate() - 13);
  return { start, end };
};

export const clampDateRange = (start: Date, end: Date) => {
  if (start > end) {
    return { start: new Date(end), end: new Date(end) };
  }

  return { start, end };
};

export const formatRangeLabel = (start: Date, end: Date) => {
  const format = (value: Date) => value.toLocaleDateString(undefined, {
    month: 'short',
    day: 'numeric',
  });

  return `${format(start)} - ${format(end)}`;
};

export const formatDisplayDate = (value: string) => {
  const parsed = parseDateInputValue(value);
  if (!parsed) {
    return value;
  }

  return parsed.toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
};

export const formatAxisDate = (value: string) => {
  const parsed = parseDateInputValue(value);
  if (!parsed) {
    return value;
  }

  return parsed.toLocaleDateString(undefined, {
    month: 'short',
    day: 'numeric',
  });
};

export const sortCaloriesByDate = (entries: CalorieEntry[]) => {
  return [...entries].sort((left, right) => left.date.localeCompare(right.date));
};

export const buildDailyCalorieTotals = (entries: CalorieEntry[], start: Date, end: Date) => {
  const totals = new Map<string, number>();

  for (const entry of entries) {
    totals.set(entry.date, (totals.get(entry.date) ?? 0) + entry.calories);
  }

  const cursor = new Date(start);
  cursor.setHours(0, 0, 0, 0);
  const endDate = new Date(end);
  endDate.setHours(0, 0, 0, 0);

  const series: Array<{ date: string; calories: number }> = [];
  while (cursor <= endDate) {
    const date = toDateInputValue(cursor);
    series.push({ date, calories: totals.get(date) ?? 0 });
    cursor.setDate(cursor.getDate() + 1);
  }

  return series;
};

const normalizePartOfDay = (value?: string | null) => {
  if (!value) {
    return null;
  }

  const trimmed = value.trim().toLowerCase();
  if (!trimmed) {
    return null;
  }

  if (trimmed === 'morning') {
    return 'morning';
  }

  if (trimmed === 'midday') {
    return 'midday';
  }

  if (trimmed === 'evening') {
    return 'evening';
  }

  if (trimmed === 'night') {
    return 'night';
  }

  return null;
};

export const buildDailyCalorieSeries = (entries: CalorieEntry[], start: Date, end: Date) => {
  const totals = new Map<
    string,
    {
      total: number;
      morning: number;
      midday: number;
      evening: number;
      night: number;
    }
  >();

  for (const entry of entries) {
    const normalized = normalizePartOfDay(entry.partOfDay);
    const existing = totals.get(entry.date) ?? {
      total: 0,
      morning: 0,
      midday: 0,
      evening: 0,
      night: 0,
    };

    existing.total += entry.calories;
    if (normalized) {
      existing[normalized] += entry.calories;
    }

    totals.set(entry.date, existing);
  }

  const cursor = new Date(start);
  cursor.setHours(0, 0, 0, 0);
  const endDate = new Date(end);
  endDate.setHours(0, 0, 0, 0);

  const series: Array<{
    date: string;
    total: number;
    morning: number;
    midday: number;
    evening: number;
    night: number;
  }> = [];

  while (cursor <= endDate) {
    const date = toDateInputValue(cursor);
    const values = totals.get(date) ?? {
      total: 0,
      morning: 0,
      midday: 0,
      evening: 0,
      night: 0,
    };
    series.push({ date, ...values });
    cursor.setDate(cursor.getDate() + 1);
  }

  return series;
};

export const getTodayDateString = () => toDateInputValue(startOfToday());
