import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { DashboardWidgetsSection } from './dashboard-widgets-section';

test('renders dashboard widget titles', () => {
  render(
    <MantineProvider>
      <DashboardWidgetsSection
        weightChartData={[]}
        calorieChartData={[]}
        weightChartStatus={{ status: 'empty', message: 'No data' }}
        calorieChartStatus={{ status: 'empty', message: 'No data' }}
        weightStats={[{ label: 'Avg', value: '70.0' }]}
        calorieStats={[{ label: 'Avg', value: '1900' }]}
        xAxisProps={{}}
        yAxisProps={{}}
        onWeightsClick={() => undefined}
        onCaloriesClick={() => undefined}
      />
    </MantineProvider>,
  );

  expect(screen.getByText('Weight trend')).toBeInTheDocument();
  expect(screen.getByText('Daily calories')).toBeInTheDocument();
});