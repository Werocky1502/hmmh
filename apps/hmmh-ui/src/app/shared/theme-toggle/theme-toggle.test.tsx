import { MantineProvider } from '@mantine/core';
import { render, screen } from '@testing-library/react';
import { ThemeToggle } from './theme-toggle';

test('renders theme toggle switch', () => {
  render(
    <MantineProvider>
      <ThemeToggle />
    </MantineProvider>,
  );

  expect(screen.getByLabelText('Toggle color scheme')).toBeInTheDocument();
});