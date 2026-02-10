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

export const getTodayDateString = () => toDateInputValue(startOfToday());
