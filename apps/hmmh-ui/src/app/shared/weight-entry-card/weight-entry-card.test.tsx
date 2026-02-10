import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { vi } from 'vitest';
import { WeightEntryCard } from './weight-entry-card';

vi.mock('../../auth/auth-context', () => ({
  useAuth: () => ({
    getAccessToken: vi.fn().mockResolvedValue(null),
  }),
}));

vi.mock('../../api/weights-api', () => ({
  getWeightByDate: vi.fn(),
  upsertWeight: vi.fn(),
}));

test('renders weight entry fields', () => {
  render(
    <MantineProvider>
      <WeightEntryCard title="Enter weight" />
    </MantineProvider>,
  );

  expect(screen.getByText('Enter weight')).toBeInTheDocument();
  expect(screen.getByLabelText('Weight (kg)')).toBeInTheDocument();
});