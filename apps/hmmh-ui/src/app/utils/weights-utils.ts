import type { WeightEntry } from '../api/contracts/weights-types';

export const sortWeightsByDate = (weights: WeightEntry[]) => {
  return [...weights].sort((left, right) => left.date.localeCompare(right.date));
};
