import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { CaloriesChartSection } from './calories-chart-section';

test('renders chart header and segmented control', () => {
  render(
    <MantineProvider>
      <CaloriesChartSection
        chartMode="total"
        onChartModeChange={() => undefined}
        dailySeries={[]}
        chartSeries={[]}
        xAxisProps={{}}
        yAxisProps={{}}
        calorieStats={[{ label: 'Avg', value: '1900' }]}
        calorieChartStatus={{ status: 'empty', message: 'No data' }}
      />
    </MantineProvider>,
  );

  expect(screen.getByText('Daily calories')).toBeInTheDocument();
  expect(screen.getByText('By day part')).toBeInTheDocument();
});