import { Text } from '@mantine/core';
import { DatePickerInput, type DatesRangeValue } from '@mantine/dates';
import styles from './date-range-filter.module.css';

interface DateRangeFilterProps {
  todayLabel: string;
  range: DatesRangeValue<string>;
  onChange: (value: DatesRangeValue<string>) => void;
}

export const DateRangeFilter = ({ todayLabel, range, onChange }: DateRangeFilterProps) => {
  return (
    <div className={styles.filterRow}>
      <Text size="sm" c="dimmed" className={styles.currentDate}>
        Today: {todayLabel}
      </Text>
      <div className={styles.filterControls}>
        <Text size="sm" c="dimmed" className={styles.filterLabel}>
          Selected period
        </Text>
        <DatePickerInput
          type="range"
          value={range}
          onChange={onChange}
          className={styles.filterInput}
          placeholder="Pick dates"
          allowSingleDateInRange
          label=""
          valueFormat="YYYY-MM-DD"
        />
      </div>
    </div>
  );
};
