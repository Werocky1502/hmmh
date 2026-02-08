import {
  Alert,
  Anchor,
  Button,
  Card,
  Container,
  Group,
  PasswordInput,
  Stack,
  Text,
  TextInput,
  Title,
} from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/auth-context';
import styles from './login-page.module.css';

type AuthMode = 'sign-in' | 'sign-up';

export const LoginPage = () => {
  const { isAuthenticated, signInWithPassword, signUpWithPassword } = useAuth();
  const [mode, setMode] = useState<AuthMode>('sign-in');
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      if (mode === 'sign-in') {
        await signInWithPassword(login, password);
      } else {
        await signUpWithPassword(login, password);
      }

      navigate('/dashboard');
    } catch (submitError) {
      const message = submitError instanceof Error
        ? submitError.message
        : 'Something went wrong. Please try again.';
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const toggleMode = () => {
    setMode((current) => (current === 'sign-in' ? 'sign-up' : 'sign-in'));
  };

  return (
    <div className={styles.page}>
      <Container size={720} className={styles.container}>
        <Card withBorder radius="lg" className={styles.card}>
          <Stack gap="lg">
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

            {error ? (
              <Alert icon={<IconAlertCircle size={16} />} color="red" variant="light">
                {error}
              </Alert>
            ) : null}

            <form onSubmit={handleSubmit}>
              <Stack gap="md">
                <TextInput
                  label="Login"
                  placeholder="Your login"
                  value={login}
                  onChange={(event) => setLogin(event.currentTarget.value)}
                  required
                />
                <PasswordInput
                  label="Password"
                  placeholder="At least 8 characters"
                  value={password}
                  onChange={(event) => setPassword(event.currentTarget.value)}
                  required
                  minLength={8}
                />
                <Button type="submit" loading={isSubmitting} fullWidth>
                  {mode === 'sign-in' ? 'Sign in' : 'Sign up'}
                </Button>
              </Stack>
            </form>

            <Group justify="space-between" className={styles.switchRow}>
              <Text size="sm" c="dimmed">
                {mode === 'sign-in'
                  ? "Don't have an account?"
                  : 'Already have an account?'}
              </Text>
              <Anchor component="button" type="button" onClick={toggleMode} size="sm">
                {mode === 'sign-in' ? 'Create one' : 'Sign in'}
              </Anchor>
            </Group>
          </Stack>
        </Card>
      </Container>
    </div>
  );
};
