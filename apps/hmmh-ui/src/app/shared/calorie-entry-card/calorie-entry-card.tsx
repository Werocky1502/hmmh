import { Button, Card, Group, NumberInput, Select, Stack, Text, TextInput } from '@mantine/core';
import { useMemo, useState } from 'react';
import { useAuth } from '../../auth/auth-context';
import { createCalorie } from '../../api/calories-api';
import type { CalorieEntryRequest } from '../../api/contracts/calories-types';
import { getTodayDateString, parseDateInputValue, toDateInputValue } from '../../utils/global-utils';
import styles from './calorie-entry-card.module.css';

type CalorieEntryLayout = 'inline' | 'stacked';
type HeaderAlign = 'left' | 'center';

interface CalorieEntryCardProps {
  title: string;
  date?: Date;
  onDateChange?: (date: Date) => void;
  allowDateChange?: boolean;
  onSaved?: () => void;
  buttonLabel?: string;
  foodPlaceholder?: string;
  layout?: CalorieEntryLayout;
  headerAlign?: HeaderAlign;
}

const partOfDayOptions = [
  { value: 'Morning', label: 'Morning' },
  { value: 'Midday', label: 'Midday' },
  { value: 'Evening', label: 'Evening' },
  { value: 'Night', label: 'Night' },
];

export const CalorieEntryCard = ({
  title,
  date,
  onDateChange,
  allowDateChange,
  onSaved,
  buttonLabel = 'Save calories',
  foodPlaceholder = 'Greek yogurt',
  layout = 'inline',
  headerAlign = 'left',
}: CalorieEntryCardProps) => {
  const { getAccessToken } = useAuth();
  const todayDate = useMemo(() => getTodayDateString(), []);
  const entryDate = date ? toDateInputValue(date) : todayDate;
  const [calories, setCalories] = useState<number | string>('');
  const [foodName, setFoodName] = useState('');
  const [partOfDay, setPartOfDay] = useState<string | null>(null);
  const [note, setNote] = useState('');
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isStacked = layout === 'stacked';
  const headerClasses = [styles.header, headerAlign === 'center' ? styles.headerCentered : '']
    .filter(Boolean)
    .join(' ');
  const cardClasses = [styles.card, isStacked ? styles.stackedCard : ''].filter(Boolean).join(' ');
  const contentClasses = [styles.content, isStacked ? styles.stackedContent : ''].filter(Boolean).join(' ');
  const formRowClasses = [styles.formRow, isStacked ? styles.formRowCentered : ''].filter(Boolean).join(' ');

  const handleSave = async () => {
    if (typeof calories !== 'number') {
      return;
    }

    const token = await getAccessToken();
    if (!token) {
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

  const saveButton = (
    <Button onClick={handleSave} loading={isSaving} disabled={typeof calories !== 'number'}>
      {buttonLabel}
    </Button>
  );

  return (
    <Card withBorder radius="lg" className={cardClasses}>
      <Stack gap={isStacked ? 'xs' : 'sm'} className={contentClasses}>
        <div className={headerClasses}>
          <Text fw={600} size="md">
            {title}
          </Text>
        </div>
        <div className={isStacked ? styles.stackedBody : undefined}>
          <Group gap="md" className={formRowClasses}>
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
              placeholder={foodPlaceholder}
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
            {!isStacked ? saveButton : null}
          </Group>
          {error ? (
            <Text size="sm" c="red">
              {error}
            </Text>
          ) : null}
        </div>
        {isStacked ? <div className={styles.saveRow}>{saveButton}</div> : null}
      </Stack>
    </Card>
  );
};
