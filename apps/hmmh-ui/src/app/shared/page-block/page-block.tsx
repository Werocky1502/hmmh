import type { ReactNode } from 'react';
import styles from './page-block.module.css';

type PageBlockVariant = 'surface' | 'section';

interface PageBlockProps {
  children: ReactNode;
  variant: PageBlockVariant;
  className?: string;
}

export const PageBlock = ({ children, variant, className }: PageBlockProps) => {
  const classes = [styles[variant], className].filter(Boolean).join(' ');
  return <div className={classes}>{children}</div>;
};
