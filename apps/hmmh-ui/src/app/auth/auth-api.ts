import { buildApiUrl, ensureSuccess } from '../api/api-client';

export interface AuthResponse {
  token: string;
  userName: string;
}

export interface AuthRequest {
  login: string;
  password: string;
}

export const signIn = async (request: AuthRequest): Promise<AuthResponse> => {
  const response = await fetch(buildApiUrl('/api/auth/sign-in'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  await ensureSuccess(response);
  return (await response.json()) as AuthResponse;
};

export const signUp = async (request: AuthRequest): Promise<AuthResponse> => {
  const response = await fetch(buildApiUrl('/api/auth/sign-up'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  await ensureSuccess(response);
  return (await response.json()) as AuthResponse;
};

export const deleteAccount = async (token: string): Promise<void> => {
  const response = await fetch(buildApiUrl('/api/auth/delete'), {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` },
  });

  await ensureSuccess(response);
};
