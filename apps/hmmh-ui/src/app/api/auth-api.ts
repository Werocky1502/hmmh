import { buildApiUrl, ensureSuccess } from './api-client';

export interface AuthRequest {
  login: string;
  password: string;
}

export interface SignUpResponse {
  userName: string;
}

export const signUp = async (request: AuthRequest): Promise<SignUpResponse> => {
  const response = await fetch(buildApiUrl('/api/auth/sign-up'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  await ensureSuccess(response);
  return (await response.json()) as SignUpResponse;
};

export const deleteAccount = async (token: string): Promise<void> => {
  const response = await fetch(buildApiUrl('/api/auth/delete'), {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` },
  });

  await ensureSuccess(response);
};
