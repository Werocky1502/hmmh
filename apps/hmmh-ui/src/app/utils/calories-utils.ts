import type { CalorieEntry } from '../api/contracts/calories-types';
import { toDateInputValue } from './global-utils';

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
