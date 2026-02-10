import { Table, Text } from '@mantine/core';
import type { ReactNode } from 'react';
import { CardShell } from '../card-shell/card-shell';
import styles from './entries-table.module.css';

interface EntriesTableProps<T> {
  title: string;
  countLabel: string;
  entries: T[];
  isLoading: boolean;
  loadingMessage: string;
  emptyMessage: string;
  columns: ReactNode[];
  renderRow: (entry: T) => ReactNode;
}

export const EntriesTable = <T,>({
  title,
  countLabel,
  entries,
  isLoading,
  loadingMessage,
  emptyMessage,
  columns,
  renderRow,
}: EntriesTableProps<T>) => {
  return (
    <CardShell title={title} count={entries.length} countLabel={countLabel}>
      {isLoading ? (
        <Text size="sm" c="dimmed" className={styles.emptyState}>
          {loadingMessage}
        </Text>
      ) : entries.length === 0 ? (
        <Text size="sm" c="dimmed" className={styles.emptyState}>
          {emptyMessage}
        </Text>
      ) : (
        <Table striped highlightOnHover>
          <Table.Thead>
            <Table.Tr>
              {columns.map((column, index) => (
                <Table.Th key={`${title}-column-${index}`}>{column}</Table.Th>
              ))}
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{entries.map((entry) => renderRow(entry))}</Table.Tbody>
        </Table>
      )}
    </CardShell>
  );
};
