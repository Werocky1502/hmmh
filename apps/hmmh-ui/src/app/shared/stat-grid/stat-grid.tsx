import { Group, Text } from '@mantine/core';
import styles from './stat-grid.module.css';

interface StatItem {
  label: string;
  value: string | number;
}

interface StatGridProps {
  items: StatItem[];
}

export const StatGrid = ({ items }: StatGridProps) => {
  return (
    <Group justify="space-between" className={styles.statRow}>
      {items.map((item) => (
        <div key={item.label}>
          <Text size="sm" c="dimmed">
            {item.label}
          </Text>
          <Text fw={600}>{item.value}</Text>
        </div>
      ))}
    </Group>
  );
};
