import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { CardShell } from './card-shell';

test('renders title and badge', () => {
  render(
    <MantineProvider>
      <CardShell title="Snapshot" count={3} countLabel="entries">
        <div>Body</div>
      </CardShell>
    </MantineProvider>,
  );

  expect(screen.getByText('Snapshot')).toBeInTheDocument();
  expect(screen.getByText('3 entries')).toBeInTheDocument();
});