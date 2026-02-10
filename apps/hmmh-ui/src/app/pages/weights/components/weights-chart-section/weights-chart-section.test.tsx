import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { WeightsChartSection } from './weights-chart-section';

test('renders weight chart title', () => {
  render(
    <MantineProvider>
      <WeightsChartSection
        chartData={[]}
        xAxisProps={{}}
        yAxisProps={{}}
        weightStats={[{ label: 'Avg', value: '70.0' }]}
        weightChartStatus={{ status: 'empty', message: 'No data' }}
      />
    </MantineProvider>,
  );

  expect(screen.getByText('Weight trend')).toBeInTheDocument();
});