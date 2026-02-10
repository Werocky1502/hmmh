import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { StatGrid } from './stat-grid';

test('renders stat items', () => {
  render(
    <MantineProvider>
      <StatGrid items={[{ label: 'Avg', value: '70.0' }]} />
    </MantineProvider>,
  );

  expect(screen.getByText('Avg')).toBeInTheDocument();
  expect(screen.getByText('70.0')).toBeInTheDocument();
});