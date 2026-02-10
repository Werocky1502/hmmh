import { buildApiUrl, ensureSuccess } from './api-client';
import type { CalorieEntry, CalorieEntryRequest } from './contracts/calories-types';

const buildAuthHeaders = (token: string, includeJson = false) => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${token}`,
  };

  if (includeJson) {
    headers['Content-Type'] = 'application/json';
  }

  return headers;
};

export const getCaloriesByDate = async (
  token: string,
  date: string,
): Promise<CalorieEntry[]> => {
  const response = await fetch(buildApiUrl(`/api/calories/${date}`), {
    headers: buildAuthHeaders(token),
  });

  await ensureSuccess(response);
  return (await response.json()) as CalorieEntry[];
};

export const getCalorieRange = async (
  token: string,
  startDate: string,
  endDate: string,
): Promise<CalorieEntry[]> => {
  const query = new URLSearchParams({ startDate, endDate });
  const response = await fetch(buildApiUrl(`/api/calories?${query.toString()}`), {
    headers: buildAuthHeaders(token),
  });

  await ensureSuccess(response);
  return (await response.json()) as CalorieEntry[];
};

export const createCalorie = async (
  token: string,
  request: CalorieEntryRequest,
): Promise<CalorieEntry> => {
  const response = await fetch(buildApiUrl('/api/calories'), {
    method: 'POST',
    headers: buildAuthHeaders(token, true),
    body: JSON.stringify(request),
  });

  await ensureSuccess(response);
  return (await response.json()) as CalorieEntry;
};

export const deleteCalorie = async (token: string, id: string): Promise<void> => {
  const response = await fetch(buildApiUrl(`/api/calories/${id}`), {
    method: 'DELETE',
    headers: buildAuthHeaders(token),
  });

  await ensureSuccess(response);
};
