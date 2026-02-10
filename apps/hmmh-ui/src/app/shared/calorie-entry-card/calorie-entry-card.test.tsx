import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { vi } from 'vitest';
import { CalorieEntryCard } from './calorie-entry-card';

vi.mock('../../auth/auth-context', () => ({
  useAuth: () => ({
    getAccessToken: vi.fn().mockResolvedValue(null),
  }),
}));

vi.mock('../../api/calories-api', () => ({
  createCalorie: vi.fn(),
}));

test('renders calorie entry fields', () => {
  render(
    <MantineProvider>
      <CalorieEntryCard title="Log calories" />
    </MantineProvider>,
  );

  expect(screen.getByText('Log calories')).toBeInTheDocument();
  expect(screen.getByLabelText('Calories')).toBeInTheDocument();
});