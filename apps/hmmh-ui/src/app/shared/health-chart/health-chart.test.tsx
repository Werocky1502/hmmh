import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { HealthChart } from './health-chart';

test('renders status message when chart is not ready', () => {
  render(
    <MantineProvider>
      <HealthChart
        kind="area"
        data={[]}
        dataKey="date"
        series={[]}
        status="empty"
        statusMessage="No data"
      />
    </MantineProvider>,
  );

  expect(screen.getByText('No data')).toBeInTheDocument();
});