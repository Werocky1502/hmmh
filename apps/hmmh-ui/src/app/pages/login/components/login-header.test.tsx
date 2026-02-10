import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { LoginHeader } from './login-header';

test('renders sign-up messaging', () => {
  render(
    <MantineProvider>
      <LoginHeader mode="sign-up" />
    </MantineProvider>,
  );

  expect(screen.getByText('Create your account')).toBeInTheDocument();
});