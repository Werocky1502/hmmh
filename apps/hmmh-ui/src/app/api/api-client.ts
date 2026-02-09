const trimSlash = (value: string) => value.replace(/\/+$/, '');

export const getApiBaseUrl = () => {
  const baseUrl = import.meta.env.VITE_API_BASE_URL ?? '';
  return trimSlash(baseUrl);
};

export const buildApiUrl = (path: string) => {
  const baseUrl = getApiBaseUrl();
  return baseUrl ? `${baseUrl}${path}` : path;
};

export const ensureSuccess = async (response: Response) => {
  if (response.ok) {
    return;
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
