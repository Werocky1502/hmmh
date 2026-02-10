import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { UserMenu } from './user-menu';

test('shows user name in menu button', () => {
  render(
    <MantineProvider>
      <UserMenu userName="Sam" onLogout={() => undefined} onDelete={() => undefined} />
    </MantineProvider>,
  );

  expect(screen.getByText('Sam')).toBeInTheDocument();
});