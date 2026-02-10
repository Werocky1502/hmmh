import { Anchor, Group, Text } from '@mantine/core';
import styles from '../login-page.module.css';

interface LoginModeToggleProps {
  mode: 'sign-in' | 'sign-up';
  onToggle: () => void;
}

export const LoginModeToggle = ({ mode, onToggle }: LoginModeToggleProps) => {
  return (
    <Group justify="space-between" className={styles.switchRow}>
      <Text size="sm" c="dimmed">
        {mode === 'sign-in' ? "Don't have an account?" : 'Already have an account?'}
      </Text>
      <Anchor component="button" type="button" onClick={onToggle} size="sm">
        {mode === 'sign-in' ? 'Create one' : 'Sign in'}
      </Anchor>
    </Group>
  );
};
