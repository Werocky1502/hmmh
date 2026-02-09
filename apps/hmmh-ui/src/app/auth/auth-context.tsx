import { createContext, useCallback, useContext, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import { deleteAccount as deleteAccountRequest, signIn, signUp } from './auth-api';

interface AuthSession {
  token: string;
  userName: string;
}

interface AuthContextValue {
  isAuthenticated: boolean;
  userName: string | null;
  token: string | null;
  signInWithPassword: (login: string, password: string) => Promise<void>;
  signUpWithPassword: (login: string, password: string) => Promise<void>;
  signOut: () => void;
  deleteAccount: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const tokenKey = 'hmmh.auth.token';
const userKey = 'hmmh.auth.user';

const loadSession = (): AuthSession | null => {
  const token = localStorage.getItem(tokenKey);
  const userName = localStorage.getItem(userKey);

  if (!token || !userName) {
    return null;
  }

  return { token, userName };
};

const persistSession = (session: AuthSession) => {
  localStorage.setItem(tokenKey, session.token);
  localStorage.setItem(userKey, session.userName);
};

const clearSession = () => {
  localStorage.removeItem(tokenKey);
  localStorage.removeItem(userKey);
};

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [session, setSession] = useState<AuthSession | null>(loadSession());

  const signInWithPassword = useCallback(async (login: string, password: string) => {
    const response = await signIn({ login, password });
    const nextSession = { token: response.token, userName: response.userName };
    persistSession(nextSession);
    setSession(nextSession);
  }, []);

  const signUpWithPassword = useCallback(async (login: string, password: string) => {
    const response = await signUp({ login, password });
    const nextSession = { token: response.token, userName: response.userName };
    persistSession(nextSession);
    setSession(nextSession);
  }, []);

  const signOut = useCallback(() => {
    clearSession();
    setSession(null);
  }, []);

  const deleteAccount = useCallback(async () => {
    if (!session) {
      return;
    }

    await deleteAccountRequest(session.token);
    clearSession();
    setSession(null);
  }, [session]);

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: Boolean(session),
      userName: session?.userName ?? null,
      token: session?.token ?? null,
      signInWithPassword,
      signUpWithPassword,
      signOut,
      deleteAccount,
    }),
    [session, signInWithPassword, signUpWithPassword, signOut, deleteAccount],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }

  return context;
};
