import type { ReactNode } from 'react';
import { render, screen } from '@testing-library/react';
import { vi } from 'vitest';
import { AuthProvider, useAuth } from './auth-context';

vi.mock('react-oidc-context', () => ({
  AuthProvider: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  useAuth: () => ({
    user: null,
    removeUser: vi.fn(),
  }),
}));

vi.mock('./oidc-client', () => ({
  getUserManager: () => ({
    signinResourceOwnerCredentials: vi.fn(),
    signinSilent: vi.fn(),
  }),
}));

vi.mock('../api/auth-api', () => ({
  deleteAccount: vi.fn(),
  signUp: vi.fn(),
}));

test('renders children through AuthProvider', () => {
  render(
    <AuthProvider>
      <div>content</div>
    </AuthProvider>,
  );

  expect(screen.getByText('content')).toBeInTheDocument();
});

test('useAuth throws when used outside provider', () => {
  const TestComponent = () => {
    useAuth();
    return null;
  };

  expect(() => render(<TestComponent />)).toThrow('useAuth must be used within AuthProvider');
});