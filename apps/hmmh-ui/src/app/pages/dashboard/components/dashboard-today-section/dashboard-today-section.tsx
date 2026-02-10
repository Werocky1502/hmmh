import { SimpleGrid } from '@mantine/core';
import { CalorieEntryCard } from '../../../../shared/calorie-entry-card/calorie-entry-card';
import { WeightEntryCard } from '../../../../shared/weight-entry-card/weight-entry-card';

interface DashboardTodaySectionProps {
  onWeightSaved: () => void;
  onCaloriesSaved: () => void;
}

export const DashboardTodaySection = ({ onWeightSaved, onCaloriesSaved }: DashboardTodaySectionProps) => {
  return (
    <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg">
      <WeightEntryCard
        title="My weight today"
        onSaved={onWeightSaved}
        layout="stacked"
        headerAlign="center"
      />
      <CalorieEntryCard
        title="Log calories today"
        onSaved={onCaloriesSaved}
        layout="stacked"
        headerAlign="center"
        foodPlaceholder="Chicken salad"
      />
    </SimpleGrid>
  );
};
