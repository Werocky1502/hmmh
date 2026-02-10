import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';
import App from './app';

vi.mock('./auth/auth-context', () => ({
  useAuth: () => ({
    isAuthenticated: false,
  }),
}));

const renderApp = (initialEntries: string[]) => {
  render(
    <MantineProvider>
      <MemoryRouter initialEntries={initialEntries}>
        <App />
      </MemoryRouter>
    </MantineProvider>,
  );
};

test('redirects unauthenticated users to login', () => {
  renderApp(['/dashboard']);
  expect(screen.getByText('Welcome back')).toBeInTheDocument();
});

test('shows the landing page on root route', () => {
  renderApp(['/']);
  expect(screen.getByText('Help me manage health')).toBeInTheDocument();
});