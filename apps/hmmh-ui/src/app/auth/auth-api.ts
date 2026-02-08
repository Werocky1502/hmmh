export interface AuthResponse {
  token: string;
  userName: string;
}

export interface AuthRequest {
  login: string;
  password: string;
}

const trimSlash = (value: string) => value.replace(/\/+$/, '');

const getApiBaseUrl = () => {
  const baseUrl = import.meta.env.VITE_API_BASE_URL ?? '';
  return trimSlash(baseUrl);
};

const buildUrl = (path: string) => {
  const baseUrl = getApiBaseUrl();
  return baseUrl ? `${baseUrl}${path}` : path;
};

const handleResponse = async (response: Response) => {
  if (response.ok) {
    return response;
  }

  let message = `Request failed (${response.status})`;

  try {
    const body = (await response.json()) as { message?: string };
    if (body?.message) {
      message = body.message;
    }
  } catch {
    // Ignore parse errors and keep the default message.
  }

  throw new Error(message);
};

export const signIn = async (request: AuthRequest): Promise<AuthResponse> => {
  const response = await fetch(buildUrl('/api/auth/sign-in'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  await handleResponse(response);
  return (await response.json()) as AuthResponse;
};

export const signUp = async (request: AuthRequest): Promise<AuthResponse> => {
  const response = await fetch(buildUrl('/api/auth/sign-up'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  await handleResponse(response);
  return (await response.json()) as AuthResponse;
};

export const deleteAccount = async (token: string): Promise<void> => {
  const response = await fetch(buildUrl('/api/auth/delete'), {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` },
  });

  await handleResponse(response);
};
