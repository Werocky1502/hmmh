import { buildApiUrl, ensureSuccess } from './api-client';
import type { WeightEntry, WeightEntryRequest } from './contracts/weights-types';

const buildAuthHeaders = (token: string, includeJson = false) => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${token}`,
  };

  if (includeJson) {
    headers['Content-Type'] = 'application/json';
  }

  return headers;
};

export const getWeightByDate = async (
  token: string,
  date: string,
): Promise<WeightEntry | null> => {
  const response = await fetch(buildApiUrl(`/api/weights/${date}`), {
    headers: buildAuthHeaders(token),
  });

  await ensureSuccess(response);
  return (await response.json()) as WeightEntry;
};

export const getWeightRange = async (
  token: string,
  startDate: string,
  endDate: string,
): Promise<WeightEntry[]> => {
  const query = new URLSearchParams({ startDate, endDate });
  const response = await fetch(buildApiUrl(`/api/weights?${query.toString()}`), {
    headers: buildAuthHeaders(token),
  });

  await ensureSuccess(response);
  return (await response.json()) as WeightEntry[];
};

export const upsertWeight = async (
  token: string,
  request: WeightEntryRequest,
): Promise<WeightEntry> => {
  const response = await fetch(buildApiUrl('/api/weights'), {
    method: 'PUT',
    headers: buildAuthHeaders(token, true),
    body: JSON.stringify(request),
  });

  await ensureSuccess(response);
  return (await response.json()) as WeightEntry;
};

export const deleteWeight = async (token: string, id: string): Promise<void> => {
  const response = await fetch(buildApiUrl(`/api/weights/${id}`), {
    method: 'DELETE',
    headers: buildAuthHeaders(token),
  });

  await ensureSuccess(response);
};
