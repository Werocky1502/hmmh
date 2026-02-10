import { Alert, Card, Container, Stack } from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/auth-context';
import { LoginForm } from './components/login-form';
import { LoginHeader } from './components/login-header';
import { LoginModeToggle } from './components/login-mode-toggle';
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
            <LoginHeader mode={mode} />

            {error ? (
              <Alert icon={<IconAlertCircle size={16} />} color="red" variant="light">
                {error}
              </Alert>
            ) : null}

            <LoginForm
              login={login}
              password={password}
              isSubmitting={isSubmitting}
              mode={mode}
              onLoginChange={setLogin}
              onPasswordChange={setPassword}
              onSubmit={handleSubmit}
            />

            <LoginModeToggle mode={mode} onToggle={toggleMode} />
          </Stack>
        </Card>
      </Container>
    </div>
  );
};
