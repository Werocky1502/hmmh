import { Button, Card, Group, NumberInput, Stack, Text, TextInput, Title } from '@mantine/core';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useAuth } from '../../auth/auth-context';
import { getWeightByDate, upsertWeight } from '../../api/weights-api';
import { getTodayDateString, parseDateInputValue, toDateInputValue } from '../../utils/global-utils';
import styles from './weight-entry-card.module.css';

type WeightEntryLayout = 'inline' | 'stacked';
type HeaderAlign = 'left' | 'center';

interface WeightEntryCardProps {
  title: string;
  date?: Date;
  onDateChange?: (date: Date) => void;
  allowDateChange?: boolean;
  onSaved?: () => void;
  buttonLabel?: string;
  layout?: WeightEntryLayout;
  headerAlign?: HeaderAlign;
}

export const WeightEntryCard = ({
  title,
  date,
  onDateChange,
  allowDateChange,
  onSaved,
  buttonLabel = 'Save weight',
  layout = 'inline',
  headerAlign = 'left',
}: WeightEntryCardProps) => {
  const { getAccessToken } = useAuth();
  const todayDate = useMemo(() => getTodayDateString(), []);
  const entryDate = date ? toDateInputValue(date) : todayDate;
  const [value, setValue] = useState<number | string>('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isStacked = layout === 'stacked';
  const headerClasses = [styles.header, headerAlign === 'center' ? styles.headerCentered : '']
    .filter(Boolean)
    .join(' ');
  const cardClasses = [styles.card, isStacked ? styles.stackedCard : ''].filter(Boolean).join(' ');
  const contentClasses = [styles.content, isStacked ? styles.stackedContent : ''].filter(Boolean).join(' ');
  const formRowClasses = [styles.formRow, isStacked ? styles.formRowCentered : ''].filter(Boolean).join(' ');

  const loadWeight = useCallback(async () => {
    const token = await getAccessToken();
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
  }, [getAccessToken, entryDate]);

  useEffect(() => {
    void loadWeight();
  }, [loadWeight]);

  const handleSave = async () => {
    if (typeof value !== 'number') {
      return;
    }

    const token = await getAccessToken();
    if (!token) {
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

  const saveButton = (
    <Button onClick={handleSave} loading={isSaving} disabled={isLoading || typeof value !== 'number'}>
      {buttonLabel}
    </Button>
  );

  return (
    <Card withBorder radius="lg" className={cardClasses}>
      <Stack gap={isStacked ? 'xs' : 'sm'} className={contentClasses}>
        <div className={headerClasses}>
          <Title order={4}>{title}</Title>
        </div>
        <div className={isStacked ? styles.stackedBody : undefined}>
          <Group gap="md" className={formRowClasses}>
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
