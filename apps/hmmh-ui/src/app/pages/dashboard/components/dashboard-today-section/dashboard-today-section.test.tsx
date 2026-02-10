import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { vi } from 'vitest';
import { DashboardTodaySection } from './dashboard-today-section';

vi.mock('../../../../shared/weight-entry-card/weight-entry-card', () => ({
  WeightEntryCard: ({ title }: { title: string }) => <div>{title}</div>,
}));

vi.mock('../../../../shared/calorie-entry-card/calorie-entry-card', () => ({
  CalorieEntryCard: ({ title }: { title: string }) => <div>{title}</div>,
}));

test('renders entry cards for today', () => {
  render(
    <MantineProvider>
      <DashboardTodaySection onWeightSaved={() => undefined} onCaloriesSaved={() => undefined} />
    </MantineProvider>,
  );

  expect(screen.getByText('My weight today')).toBeInTheDocument();
  expect(screen.getByText('Log calories today')).toBeInTheDocument();
});