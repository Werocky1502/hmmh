import { Text, Title } from '@mantine/core';
import styles from '../login-page.module.css';

interface LoginHeaderProps {
  mode: 'sign-in' | 'sign-up';
}

export const LoginHeader = ({ mode }: LoginHeaderProps) => {
  return (
    <div>
      <Text className={styles.kicker}>HelpMeManageHealth</Text>
      <Title order={2} className={styles.title}>
        {mode === 'sign-in' ? 'Welcome back' : 'Create your account'}
      </Title>
      <Text c="dimmed">
        {mode === 'sign-in'
          ? 'Sign in with your login and password.'
          : 'Set a login and password to start tracking.'}
      </Text>
    </div>
  );
};
