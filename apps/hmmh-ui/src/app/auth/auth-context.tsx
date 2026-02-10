import { createContext, useCallback, useContext, useMemo } from 'react';
import type { ReactNode } from 'react';
import { AuthProvider as OidcAuthProvider, useAuth as useOidcAuth } from 'react-oidc-context';
import { getUserManager } from './oidc-client';
import { deleteAccount as deleteAccountRequest, signUp } from '../api/auth-api';

interface AuthContextValue {
  isAuthenticated: boolean;
  userName: string | null;
  getAccessToken: () => Promise<string | null>;
  signInWithPassword: (login: string, password: string) => Promise<void>;
  signUpWithPassword: (login: string, password: string) => Promise<void>;
  signOut: () => void;
  deleteAccount: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const parseJwtPayload = (token?: string | null) => {
  if (!token) {
    return null;
  }

  const parts = token.split('.');
  if (parts.length < 2) {
    return null;
  }

  try {
    const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = payload.padEnd(payload.length + ((4 - (payload.length % 4)) % 4), '=');
    const decoded = atob(padded);
    return JSON.parse(decoded) as Record<string, unknown>;
  } catch {
    return null;
  }
};

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const userManager = getUserManager();

  return (
    <OidcAuthProvider userManager={userManager}>
      <AuthContextProvider>{children}</AuthContextProvider>
    </OidcAuthProvider>
  );
};

const AuthContextProvider = ({ children }: { children: ReactNode }) => {
  const oidc = useOidcAuth();
  const userManager = getUserManager();

  const signInWithPassword = useCallback(async (login: string, password: string) => {
    await userManager.signinResourceOwnerCredentials({
      username: login,
      password,
    });
  }, [userManager]);

  const signUpWithPassword = useCallback(async (login: string, password: string) => {
    await signUp({ login, password });
    await signInWithPassword(login, password);
  }, [signInWithPassword]);

  const signOut = useCallback(() => {
    void oidc.removeUser();
  }, [oidc]);

  const getAccessToken = useCallback(async () => {
    if (oidc.user && !oidc.user.expired) {
      return oidc.user.access_token;
    }

    try {
      const refreshed = await userManager.signinSilent();
      return refreshed?.access_token ?? null;
    } catch {
      await oidc.removeUser();
      return null;
    }
  }, [oidc, userManager]);

  const deleteAccount = useCallback(async () => {
    const token = await getAccessToken();
    if (!token) {
      return;
    }

    await deleteAccountRequest(token);
    await oidc.removeUser();
  }, [getAccessToken, oidc]);

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: Boolean(oidc.user) && !oidc.user?.expired,
      userName: (() => {
        const profileName = oidc.user?.profile?.name
          ?? oidc.user?.profile?.preferred_username
          ?? oidc.user?.profile?.sub;
        if (profileName) {
          return profileName;
        }

        const claims = parseJwtPayload(oidc.user?.access_token);
        if (!claims) {
          return null;
        }

        return (claims.name
          ?? claims.preferred_username
          ?? claims.sub
          ?? null) as string | null;
      })(),
      getAccessToken,
      signInWithPassword,
      signUpWithPassword,
      signOut,
      deleteAccount,
    }),
    [oidc.user, getAccessToken, signInWithPassword, signUpWithPassword, signOut, deleteAccount],
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
