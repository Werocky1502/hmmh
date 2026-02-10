import { render, screen } from '@testing-library/react';
import { PageBlock } from './page-block';

test('renders children inside block', () => {
  render(
    <PageBlock variant="section">
      <div>Content</div>
    </PageBlock>,
  );

  expect(screen.getByText('Content')).toBeInTheDocument();
});