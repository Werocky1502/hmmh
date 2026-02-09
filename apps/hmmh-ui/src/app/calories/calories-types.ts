export interface CalorieEntry {
  id: string;
  date: string;
  calories: number;
  foodName?: string | null;
  partOfDay?: string | null;
  note?: string | null;
}

export interface CalorieEntryRequest {
  date: string;
  calories: number;
  foodName?: string | null;
  partOfDay?: string | null;
  note?: string | null;
}
