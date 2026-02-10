import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { EntriesTable } from './entries-table';

test('renders empty state when there are no entries', () => {
  render(
    <MantineProvider>
      <EntriesTable
        title="Entries"
        countLabel="entries"
        entries={[]}
        isLoading={false}
        loadingMessage="Loading"
        emptyMessage="Nothing here"
        columns={['Date']}
        renderRow={() => null}
      />
    </MantineProvider>,
  );

  expect(screen.getByText('Nothing here')).toBeInTheDocument();
});