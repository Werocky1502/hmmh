import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { DateRangeFilter } from './date-range-filter';

test('renders the current date label', () => {
  render(
    <MantineProvider>
      <DateRangeFilter
        todayLabel="Feb 10"
        range={['2026-02-01', '2026-02-10']}
        onChange={() => undefined}
      />
    </MantineProvider>,
  );

  expect(screen.getByText('Today: Feb 10')).toBeInTheDocument();
});