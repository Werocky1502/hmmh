import { Button, Center, Stack, Text, Title } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/auth-context';
import { ThemeToggle } from '../shared/theme-toggle/theme-toggle';
import styles from './landing-page.module.css';

export const LandingPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const handleClick = () => {
    navigate(isAuthenticated ? '/dashboard' : '/login');
  };

  return (
    <div className={styles.page}>
      <div className={styles.pattern} />
      <div className={styles.themeToggle}>
        <ThemeToggle />
      </div>
      <Center className={styles.content}>
        <Stack gap="lg" align="center" className={styles.stack}>
          <Title order={1} className={styles.title}>
            Help me manage health
          </Title>
          <Text size="lg" className={styles.subtitle}>
            Calm daily tracking for weight and calories.
          </Text>
          <Button size="lg" onClick={handleClick} className={styles.cta}>
            {isAuthenticated ? 'Show me the dashboard' : 'Log in to start usage'}
          </Button>
        </Stack>
      </Center>
      <div className={styles.notice} role="status" aria-live="polite">
        <Text size="sm">
          Warning: this app was built with a suspiciously low amount of human
          intelligence.
        </Text>
      </div>
    </div>
  );
};
