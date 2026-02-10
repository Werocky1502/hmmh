import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { PageShell } from './page-shell';

test('renders title and slots', () => {
  render(
    <MantineProvider>
      <PageShell title="Overview" leftSlot={<div>Left</div>} rightSlot={<div>Right</div>}>
        <div>Body</div>
      </PageShell>
    </MantineProvider>,
  );

  expect(screen.getByText('Overview')).toBeInTheDocument();
  expect(screen.getByText('Left')).toBeInTheDocument();
  expect(screen.getByText('Right')).toBeInTheDocument();
});