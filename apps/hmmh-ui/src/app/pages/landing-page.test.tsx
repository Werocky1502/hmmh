import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';
import { LandingPage } from './landing-page';

vi.mock('../auth/auth-context', () => ({
  useAuth: () => ({
    isAuthenticated: false,
  }),
}));

test('renders the landing page call to action', () => {
  render(
    <MantineProvider>
      <MemoryRouter>
        <LandingPage />
      </MemoryRouter>
    </MantineProvider>,
  );

  expect(screen.getByText('Log in to start usage')).toBeInTheDocument();
});