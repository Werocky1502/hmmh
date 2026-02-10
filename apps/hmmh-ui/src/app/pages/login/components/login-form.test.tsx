import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi } from 'vitest';
import { LoginForm } from './login-form';

test('updates login field on input', async () => {
  const user = userEvent.setup();
  const onLoginChange = vi.fn();

  render(
    <MantineProvider>
      <LoginForm
        login=""
        password=""
        isSubmitting={false}
        mode="sign-in"
        onLoginChange={onLoginChange}
        onPasswordChange={vi.fn()}
        onSubmit={vi.fn()}
      />
    </MantineProvider>,
  );

  await user.type(screen.getByPlaceholderText('Your login'), 'sam');
  expect(onLoginChange).toHaveBeenCalled();
});