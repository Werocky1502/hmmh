import { Badge, Card, Group, Stack, Text } from '@mantine/core';
import type { ReactNode } from 'react';
import styles from './card-shell.module.css';

type CardHeaderAlign = 'space-between' | 'center';
type CardHeaderDirection = 'row' | 'column';
type CardTone = 'solid' | 'soft';

interface CardShellProps {
  title: string;
  children: ReactNode;
  actions?: ReactNode;
  count?: number;
  countLabel?: string;
  onClick?: () => void;
  headerAlign?: CardHeaderAlign;
  headerDirection?: CardHeaderDirection;
  tone?: CardTone;
}

export const CardShell = ({
  title,
  children,
  actions,
  count,
  countLabel,
  onClick,
  headerAlign = 'space-between',
  headerDirection = 'row',
  tone = 'solid',
}: CardShellProps) => {
  const isClickable = Boolean(onClick);
  const showBadge = typeof count === 'number' && countLabel;
  const headerClasses = [
    styles.header,
    headerAlign === 'center' ? styles.headerCentered : '',
    headerDirection === 'column' ? styles.headerColumn : '',
  ]
    .filter(Boolean)
    .join(' ');

  const cardClasses = [styles.card, styles[tone], isClickable ? styles.clickable : '']
    .filter(Boolean)
    .join(' ');

  return (
    <Card
      withBorder
      radius="lg"
      className={cardClasses}
      onClick={onClick}
      onKeyDown={(event) => {
        if (!onClick) {
          return;
        }

        if (event.key === 'Enter' || event.key === ' ') {
          event.preventDefault();
          onClick();
        }
      }}
      role={onClick ? 'button' : undefined}
      tabIndex={onClick ? 0 : undefined}
    >
      <Stack gap="md">
        <Group justify={headerAlign} className={headerClasses}>
          <Text fw={600} size="md">
            {title}
          </Text>
          {actions}
          {showBadge ? (
            <Badge color="teal" variant="light">
              {count} {countLabel}
            </Badge>
          ) : null}
        </Group>
        {children}
      </Stack>
    </Card>
  );
};
