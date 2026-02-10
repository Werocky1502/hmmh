import { Button, Card, Group, NumberInput, Select, Stack, Text, TextInput, Title } from '@mantine/core';
import { useMemo, useState } from 'react';
import { useAuth } from '../auth/auth-context';
import { createCalorie } from './calories-api';
import type { CalorieEntryRequest } from './calories-types';
import { getTodayDateString, parseDateInputValue, toDateInputValue } from './calories-utils';
import styles from './calorie-entry-card.module.css';

interface CalorieEntryCardProps {
  date?: Date;
  onDateChange?: (date: Date) => void;
  allowDateChange?: boolean;
  onSaved?: () => void;
}

const partOfDayOptions = [
  { value: 'Morning', label: 'Morning' },
  { value: 'Midday', label: 'Midday' },
  { value: 'Evening', label: 'Evening' },
  { value: 'Night', label: 'Night' },
];

export const CalorieEntryCard = ({ date, onDateChange, allowDateChange, onSaved }: CalorieEntryCardProps) => {
  const { token } = useAuth();
  const todayDate = useMemo(() => getTodayDateString(), []);
  const entryDate = date ? toDateInputValue(date) : todayDate;
  const [calories, setCalories] = useState<number | string>('');
  const [foodName, setFoodName] = useState('');
  const [partOfDay, setPartOfDay] = useState<string | null>(null);
  const [note, setNote] = useState('');
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async () => {
    if (!token || typeof calories !== 'number') {
      return;
    }

    setIsSaving(true);
    setError(null);

    const request: CalorieEntryRequest = {
      date: entryDate,
      calories,
      foodName: foodName.trim() ? foodName.trim() : null,
      partOfDay: partOfDay ?? null,
      note: note.trim() ? note.trim() : null,
    };

    try {
      await createCalorie(token, request);
      setCalories('');
      setFoodName('');
      setPartOfDay(null);
      setNote('');
      onSaved?.();
    } catch (saveError) {
      const message = saveError instanceof Error ? saveError.message : 'Failed to save calories.';
      setError(message);
    } finally {
      setIsSaving(false);
    }
  };

  const handleDateChange = (value: string) => {
    const nextDate = parseDateInputValue(value);
    if (!nextDate) {
      return;
    }

    onDateChange?.(nextDate);
  };

  return (
    <Card withBorder radius="lg" className={styles.card}>
      <Stack gap="sm">
        <Title order={4}>Log calories</Title>
        <Group gap="md" className={styles.formRow}>
          <NumberInput
            label="Calories"
            value={calories}
            onChange={setCalories}
            step={1}
            disabled={isSaving}
            className={styles.calorieInput}
          />
          <TextInput
            label="Food name"
            value={foodName}
            onChange={(event) => setFoodName(event.currentTarget.value)}
            placeholder="Greek yogurt"
            disabled={isSaving}
            className={styles.textInput}
          />
          <Select
            label="Part of day"
            data={partOfDayOptions}
            value={partOfDay}
            onChange={setPartOfDay}
            placeholder="Select"
            clearable
            disabled={isSaving}
            className={styles.textInput}
          />
          <TextInput
            label="Note"
            value={note}
            onChange={(event) => setNote(event.currentTarget.value)}
            placeholder="Optional note"
            disabled={isSaving}
            className={styles.noteInput}
          />
          {allowDateChange ? (
            <TextInput
              label="Date"
              type="date"
              value={entryDate}
              onChange={(event) => handleDateChange(event.currentTarget.value)}
              className={styles.dateInput}
              disabled={isSaving}
            />
          ) : null}
        </Group>
        <div className={styles.saveRow}>
          <Button onClick={handleSave} loading={isSaving} disabled={typeof calories !== 'number'}>
            Save calories
          </Button>
        </div>
        {error ? (
          <Text size="sm" c="red">
            {error}
          </Text>
        ) : null}
      </Stack>
    </Card>
  );
};
