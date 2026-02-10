import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';
import { DashboardPage } from './dashboard-page';

vi.mock('../../auth/auth-context', () => ({
  useAuth: () => ({
    userName: 'Sam',
    signOut: vi.fn(),
    deleteAccount: vi.fn(),
  }),
}));

vi.mock('../../hooks/use-dashboard-data', () => ({
  useDashboardData: () => ({
    range: ['2026-02-01', '2026-02-10'],
    handleRangeChange: vi.fn(),
    todayLabel: 'Feb 10',
    rangeError: null,
    caloriesError: null,
    weightChartData: [],
    weightStats: [{ label: 'Avg', value: '70.0' }],
    weightChartStatus: { status: 'empty', message: 'No data' },
    calorieChartData: [],
    calorieStats: [{ label: 'Avg', value: '1900' }],
    calorieChartStatus: { status: 'empty', message: 'No data' },
    xAxisProps: {},
    yAxisProps: {},
    refreshWeights: vi.fn(),
    refreshCalories: vi.fn(),
  }),
}));

vi.mock('../../shared/weight-entry-card/weight-entry-card', () => ({
  WeightEntryCard: ({ title }: { title: string }) => <div>{title}</div>,
}));

vi.mock('../../shared/calorie-entry-card/calorie-entry-card', () => ({
  CalorieEntryCard: ({ title }: { title: string }) => <div>{title}</div>,
}));

test('renders dashboard shell content', () => {
  render(
    <MantineProvider>
      <MemoryRouter>
        <DashboardPage />
      </MemoryRouter>
    </MantineProvider>,
  );

  expect(screen.getByText('Help me manage health')).toBeInTheDocument();
  expect(screen.getByText('Weight trend')).toBeInTheDocument();
});