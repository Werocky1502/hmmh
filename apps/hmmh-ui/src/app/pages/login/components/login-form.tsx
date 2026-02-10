import { Button, PasswordInput, Stack, TextInput } from '@mantine/core';

interface LoginFormProps {
  login: string;
  password: string;
  isSubmitting: boolean;
  mode: 'sign-in' | 'sign-up';
  onLoginChange: (value: string) => void;
  onPasswordChange: (value: string) => void;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
}

export const LoginForm = ({
  login,
  password,
  isSubmitting,
  mode,
  onLoginChange,
  onPasswordChange,
  onSubmit,
}: LoginFormProps) => {
  return (
    <form onSubmit={onSubmit}>
      <Stack gap="md">
        <TextInput
          label="Login"
          placeholder="Your login"
          value={login}
          onChange={(event) => onLoginChange(event.currentTarget.value)}
          required
        />
        <PasswordInput
          label="Password"
          placeholder="At least 8 characters"
          value={password}
          onChange={(event) => onPasswordChange(event.currentTarget.value)}
          required
          minLength={8}
        />
        <Button type="submit" loading={isSubmitting} fullWidth>
          {mode === 'sign-in' ? 'Sign in' : 'Sign up'}
        </Button>
      </Stack>
    </form>
  );
};
