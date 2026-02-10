import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi } from 'vitest';
import { LoginModeToggle } from './login-mode-toggle';

test('toggles when the action is clicked', async () => {
  const user = userEvent.setup();
  const onToggle = vi.fn();

  render(
    <MantineProvider>
      <LoginModeToggle mode="sign-in" onToggle={onToggle} />
    </MantineProvider>,
  );

  await user.click(screen.getByRole('button', { name: 'Create one' }));
  expect(onToggle).toHaveBeenCalledTimes(1);
});