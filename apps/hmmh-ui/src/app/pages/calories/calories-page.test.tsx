import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';
import { CaloriesPage } from './calories-page';

vi.mock('../../auth/auth-context', () => ({
  useAuth: () => ({
    userName: 'Sam',
    signOut: vi.fn(),
    deleteAccount: vi.fn(),
  }),
}));

vi.mock('../../hooks/use-calories-page-data', () => ({
  useCaloriesPageData: () => ({
    range: ['2026-02-01', '2026-02-10'],
    handleRangeChange: vi.fn(),
    todayLabel: 'Feb 10',
    rangeError: null,
    isLoadingRange: false,
    deletingId: null,
    entryDate: new Date('2026-02-10T00:00:00Z'),
    setEntryDate: vi.fn(),
    chartMode: 'total',
    setChartMode: vi.fn(),
    dailySeries: [],
    chartSeries: [],
    xAxisProps: {},
    yAxisProps: {},
    calorieStats: [{ label: 'Avg', value: '1900' }],
    calorieChartStatus: { status: 'empty', message: 'No data' },
    sortedEntries: [],
    handleDeleteEntry: vi.fn(),
    refreshCalories: vi.fn(),
  }),
}));

vi.mock('../../shared/calorie-entry-card/calorie-entry-card', () => ({
  CalorieEntryCard: ({ title }: { title: string }) => <div>{title}</div>,
}));

test('renders calorie page shell content', () => {
  render(
    <MantineProvider>
      <MemoryRouter>
        <CaloriesPage />
      </MemoryRouter>
    </MantineProvider>,
  );

  expect(screen.getByText('My calorie history')).toBeInTheDocument();
  expect(screen.getByText('Back to dashboard')).toBeInTheDocument();
});