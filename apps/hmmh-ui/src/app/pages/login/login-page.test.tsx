import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';
import { LoginPage } from './login-page';

const signInWithPassword = vi.fn();
const signUpWithPassword = vi.fn();

vi.mock('../../auth/auth-context', () => ({
  useAuth: () => ({
    isAuthenticated: false,
    signInWithPassword,
    signUpWithPassword,
  }),
}));

test('renders the sign-in view by default', () => {
  render(
    <MantineProvider>
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>
    </MantineProvider>,
  );

  expect(screen.getByText('Welcome back')).toBeInTheDocument();
  expect(screen.getByRole('button', { name: 'Sign in' })).toBeInTheDocument();
});