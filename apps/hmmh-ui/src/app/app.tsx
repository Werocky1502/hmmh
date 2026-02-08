import {
  Badge,
  Button,
  Card,
  Container,
  Divider,
  Group,
  Progress,
  SimpleGrid,
  Stack,
  Text,
  ThemeIcon,
  Title,
} from '@mantine/core';
import {
  IconActivity,
  IconApple,
  IconScale,
  IconSparkles,
  IconTargetArrow,
} from '@tabler/icons-react';
import styles from './app.module.css';

const metrics = [
  {
    title: 'Weekly kcal avg',
    value: '1,842',
    change: '+3%',
    detail: 'Balanced intake',
    icon: IconApple,
  },
  {
    title: 'Weight trend',
    value: '-0.6 kg',
    change: '14 days',
    detail: 'Steady progress',
    icon: IconScale,
  },
  {
    title: 'Active days',
    value: '5',
    change: 'this week',
    detail: 'Movement streak',
    icon: IconActivity,
  },
  {
    title: 'Goal focus',
    value: '82%',
    change: '+6%',
    detail: 'Nutrition alignment',
    icon: IconTargetArrow,
  },
];

const habits = [
  {
    title: 'Protein rhythm',
    subtitle: 'Keep 25-30g per meal',
    progress: 72,
    color: 'teal',
  },
  {
    title: 'Hydration cadence',
    subtitle: '2.4L in the last 7 days',
    progress: 64,
    color: 'cyan',
  },
  {
    title: 'Evening reset',
    subtitle: 'Light dinners 4 of 7 days',
    progress: 57,
    color: 'lime',
  },
];

export function App() {
  return (
    <div className={styles.page}>
      <div className={styles.glow} />
      <Container size="lg" className={styles.content}>
        <Group justify="space-between" className={styles.header}>
          <Group gap="xs">
            <ThemeIcon variant="light" color="teal" size="lg" radius="md">
              <IconSparkles size={18} />
            </ThemeIcon>
            <Text className={styles.brand}>HMMH</Text>
          </Group>
          <Badge variant="light" color="teal">
            Beta workspace
          </Badge>
        </Group>

        <div className={styles.hero}>
          <Stack gap="md" className={styles.heroCopy}>
            <Badge color="teal" variant="outline">
              Daily wellness dashboard
            </Badge>
            <Title order={1} className={styles.heroTitle}>
              See how daily calories shape long-term weight trends.
            </Title>
            <Text size="lg" c="dimmed">
              HMMH turns meals, weigh-ins, and habit signals into a calm view of
              progress. Track what matters and spot momentum before it fades.
            </Text>
            <Group>
              <Button size="md">Start tracking</Button>
              <Button size="md" variant="light" color="teal">
                View sample data
              </Button>
            </Group>
            <Group className={styles.heroStats}>
              <div>
                <Text className={styles.heroValue}>1,842</Text>
                <Text size="sm" c="dimmed">
                  avg kcal/day
                </Text>
              </div>
              <div>
                <Text className={styles.heroValue}>-0.6 kg</Text>
                <Text size="sm" c="dimmed">
                  2-week trend
                </Text>
              </div>
              <div>
                <Text className={styles.heroValue}>82%</Text>
                <Text size="sm" c="dimmed">
                  goal alignment
                </Text>
              </div>
            </Group>
          </Stack>

          <Card withBorder radius="lg" className={styles.heroCard}>
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Today</Text>
                <Badge color="teal" variant="light">
                  2 entries
                </Badge>
              </Group>
              <Group align="flex-start" className={styles.cardRow}>
                <ThemeIcon variant="light" color="teal" size="lg">
                  <IconApple size={18} />
                </ThemeIcon>
                <div>
                  <Text fw={600}>1,620 kcal logged</Text>
                  <Text size="sm" c="dimmed">
                    80% of daily target
                  </Text>
                </div>
              </Group>
              <Progress value={80} color="teal" size="md" radius="xl" />
              <Group align="flex-start" className={styles.cardRow}>
                <ThemeIcon variant="light" color="cyan" size="lg">
                  <IconScale size={18} />
                </ThemeIcon>
                <div>
                  <Text fw={600}>78.4 kg</Text>
                  <Text size="sm" c="dimmed">
                    Evening weigh-in scheduled
                  </Text>
                </div>
              </Group>
              <Divider />
              <Text size="sm" c="dimmed">
                Next nudge: prioritize protein at dinner to keep the weekly
                average steady.
              </Text>
            </Stack>
          </Card>
        </div>

        <Divider my="xl" />

        <Title order={2} className={styles.sectionTitle}>
          Your week at a glance
        </Title>
        <SimpleGrid cols={{ base: 1, sm: 2, md: 4 }} spacing="lg">
          {metrics.map((metric) => (
            <Card key={metric.title} withBorder radius="lg" className={styles.metricCard}>
              <Group justify="space-between" align="flex-start">
                <div>
                  <Text size="sm" c="dimmed">
                    {metric.title}
                  </Text>
                  <Text className={styles.metricValue}>{metric.value}</Text>
                </div>
                <ThemeIcon variant="light" color="teal" size="lg">
                  <metric.icon size={18} />
                </ThemeIcon>
              </Group>
              <Group gap="xs" className={styles.metricMeta}>
                <Badge variant="light" color="teal">
                  {metric.change}
                </Badge>
                <Text size="sm" c="dimmed">
                  {metric.detail}
                </Text>
              </Group>
            </Card>
          ))}
        </SimpleGrid>

        <Group justify="space-between" align="flex-end" className={styles.sectionHeader}>
          <Title order={2}>Habit rhythm</Title>
          <Text size="sm" c="dimmed">
            Sample insights from the last 7 days
          </Text>
        </Group>
        <SimpleGrid cols={{ base: 1, md: 3 }} spacing="lg">
          {habits.map((habit) => (
            <Card key={habit.title} withBorder radius="lg" className={styles.planCard}>
              <Stack gap="sm">
                <Text fw={600}>{habit.title}</Text>
                <Text size="sm" c="dimmed">
                  {habit.subtitle}
                </Text>
                <Progress value={habit.progress} color={habit.color} radius="xl" />
                <Text size="xs" c="dimmed">
                  {habit.progress}% on track
                </Text>
              </Stack>
            </Card>
          ))}
        </SimpleGrid>
      </Container>
    </div>
  );
}

export default App;
