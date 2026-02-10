import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';
import { WeightsPage } from './weights-page';

vi.mock('../../auth/auth-context', () => ({
  useAuth: () => ({
    userName: 'Sam',
    signOut: vi.fn(),
    deleteAccount: vi.fn(),
  }),
}));

vi.mock('../../hooks/use-weights-page-data', () => ({
  useWeightsPageData: () => ({
    range: ['2026-02-01', '2026-02-10'],
    handleRangeChange: vi.fn(),
    todayLabel: 'Feb 10',
    rangeError: null,
    isLoadingRange: false,
    deletingId: null,
    entryDate: new Date('2026-02-10T00:00:00Z'),
    setEntryDate: vi.fn(),
    chartData: [],
    xAxisProps: {},
    yAxisProps: {},
    weightStats: [{ label: 'Avg', value: '70.0' }],
    weightChartStatus: { status: 'empty', message: 'No data' },
    sortedWeights: [],
    handleDeleteEntry: vi.fn(),
    refreshWeights: vi.fn(),
  }),
}));

vi.mock('../../shared/weight-entry-card/weight-entry-card', () => ({
  WeightEntryCard: ({ title }: { title: string }) => <div>{title}</div>,
}));

test('renders weight page shell content', () => {
  render(
    <MantineProvider>
      <MemoryRouter>
        <WeightsPage />
      </MemoryRouter>
    </MantineProvider>,
  );

  expect(screen.getByText('My weight history')).toBeInTheDocument();
  expect(screen.getByText('Back to dashboard')).toBeInTheDocument();
});