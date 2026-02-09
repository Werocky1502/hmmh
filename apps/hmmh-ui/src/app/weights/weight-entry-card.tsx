import { Button, Card, Group, NumberInput, Stack, Text, TextInput, Title } from '@mantine/core';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useAuth } from '../auth/auth-context';
import { getWeightByDate, upsertWeight } from './weights-api';
import { getTodayDateString, parseDateInputValue, toDateInputValue } from './weights-utils';
import styles from './weight-entry-card.module.css';

interface WeightEntryCardProps {
  date?: Date;
  onDateChange?: (date: Date) => void;
  allowDateChange?: boolean;
  onSaved?: () => void;
}

export const WeightEntryCard = ({ date, onDateChange, allowDateChange, onSaved }: WeightEntryCardProps) => {
  const { token } = useAuth();
  const todayDate = useMemo(() => getTodayDateString(), []);
  const entryDate = date ? toDateInputValue(date) : todayDate;
  const [value, setValue] = useState<number | string>('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadWeight = useCallback(async () => {
    if (!token) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const entry = await getWeightByDate(token, entryDate);
      if (entry && entry.weightKg > 0) {
        setValue(entry.weightKg);
      }
    } catch (loadError) {
      const message = loadError instanceof Error ? loadError.message : 'Failed to load weight.';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }, [token, entryDate]);

  useEffect(() => {
    void loadWeight();
  }, [loadWeight]);

  const handleSave = async () => {
    if (!token || typeof value !== 'number') {
      return;
    }

    setIsSaving(true);
    setError(null);

    try {
      const response = await upsertWeight(token, { date: entryDate, weightKg: value });
      setValue(response.weightKg);
      onSaved?.();
    } catch (saveError) {
      const message = saveError instanceof Error ? saveError.message : 'Failed to save weight.';
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
        <Title order={4}>Enter weight</Title>
        <Group gap="md" className={styles.formRow}>
          <NumberInput
            label="Weight (kg)"
            value={value}
            onChange={setValue}
            min={20}
            max={500}
            step={0.1}
            decimalScale={1}
            fixedDecimalScale
            disabled={isLoading}
            className={styles.weightInput}
          />
          {allowDateChange ? (
            <TextInput
              label="Date"
              type="date"
              value={entryDate}
              onChange={(event) => handleDateChange(event.currentTarget.value)}
              className={styles.dateInput}
            />
          ) : null}
          <Button onClick={handleSave} loading={isSaving} disabled={isLoading || typeof value !== 'number'}>
            Save weight
          </Button>
        </Group>
        {error ? (
          <Text size="sm" c="red">
            {error}
          </Text>
        ) : null}
      </Stack>
    </Card>
  );
};
