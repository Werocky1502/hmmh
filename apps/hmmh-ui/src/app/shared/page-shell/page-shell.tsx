import { Container, Group, Title } from '@mantine/core';
import type { ReactNode } from 'react';
import { PageBlock } from '../page-block/page-block';
import styles from './page-shell.module.css';

interface PageShellProps {
  title: string;
  leftSlot?: ReactNode;
  rightSlot?: ReactNode;
  children: ReactNode;
}

export const PageShell = ({ title, leftSlot, rightSlot, children }: PageShellProps) => {
  return (
    <PageBlock variant="surface">
      <Container size="lg" className={styles.container}>
        <Group justify="space-between" className={styles.header}>
          <div className={styles.headerSlot}>{leftSlot ?? <div className={styles.headerSpacer} />}</div>
          <Title order={2} className={styles.title}>
            {title}
          </Title>
          <div className={styles.headerSlot}>{rightSlot}</div>
        </Group>
        {children}
      </Container>
    </PageBlock>
  );
};
