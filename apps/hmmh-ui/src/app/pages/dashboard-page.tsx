import {
  Avatar,
  Badge,
  Button,
  Card,
  Container,
  Group,
  Menu,
  SimpleGrid,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import { IconChevronDown, IconLogout, IconTrash } from '@tabler/icons-react';
import { useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/auth-context';
import styles from './dashboard-page.module.css';

const weightSeries = [78.8, 78.5, 78.2, 78.4, 78.1, 77.9, 77.8];
const calorieSeries = [1780, 1905, 1720, 1840, 1760, 1690, 1810];

const buildSparkPath = (values: number[], width: number, height: number) => {
  const min = Math.min(...values);
  const max = Math.max(...values);
  const range = Math.max(max - min, 1);
  const step = width / (values.length - 1);

  return values
    .map((value, index) => {
      const x = index * step;
      const y = height - ((value - min) / range) * height;
      return `${x},${y}`;
    })
    .join(' ');
};

export const DashboardPage = () => {
  const { userName, signOut, deleteAccount } = useAuth();
  const navigate = useNavigate();

  const initial = userName?.[0]?.toUpperCase() ?? 'U';

  const sparkPath = useMemo(() => buildSparkPath(weightSeries, 360, 120), []);

  const handleLogout = () => {
    signOut();
    navigate('/');
  };

  const handleDelete = async () => {
    await deleteAccount();
    navigate('/');
  };

  return (
    <div className={styles.page}>
      <Container size="lg" className={styles.container}>
        <Group justify="space-between" className={styles.header}>
          <div className={styles.headerSpacer} />
          <Title order={2} className={styles.title}>
            Help me manage health
          </Title>
          <Menu width={200} position="bottom-end" shadow="md">
            <Menu.Target>
              <Button variant="subtle" rightSection={<IconChevronDown size={16} />}
                className={styles.userButton}
              >
                <Group gap="sm">
                  <Avatar radius="xl" color="teal">
                    {initial}
                  </Avatar>
                  <Text fw={600}>{userName ?? 'User'}</Text>
                </Group>
              </Button>
            </Menu.Target>
            <Menu.Dropdown>
              <Menu.Item leftSection={<IconLogout size={16} />} onClick={handleLogout}>
                Logout
              </Menu.Item>
              <Menu.Item
                color="red"
                leftSection={<IconTrash size={16} />}
                onClick={handleDelete}
              >
                Delete account
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>

        <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg" className={styles.grid}>
          <Card withBorder radius="lg" className={styles.card}>
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Weight trend</Text>
                <Badge color="teal" variant="light">
                  7 days
                </Badge>
              </Group>
              <div className={styles.sparkline}>
                <svg viewBox="0 0 360 120" preserveAspectRatio="none">
                  <polyline
                    fill="none"
                    stroke="#1f6f73"
                    strokeWidth="4"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    points={sparkPath}
                  />
                </svg>
              </div>
              <Group justify="space-between" className={styles.statRow}>
                <div>
                  <Text size="sm" c="dimmed">
                    Starting weight
                  </Text>
                  <Text fw={600}>78.8 kg</Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Current weight
                  </Text>
                  <Text fw={600}>77.8 kg</Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Change
                  </Text>
                  <Text fw={600}>-1.0 kg</Text>
                </div>
              </Group>
            </Stack>
          </Card>

          <Card withBorder radius="lg" className={styles.card}>
            <Stack gap="md">
              <Group justify="space-between">
                <Text fw={600}>Daily calories</Text>
                <Badge color="teal" variant="light">
                  avg 1,790
                </Badge>
              </Group>
              <div className={styles.bars}>
                {calorieSeries.map((value) => (
                  <div key={value} className={styles.bar}>
                    <span style={{ height: `${(value / 2200) * 100}%` }} />
                  </div>
                ))}
              </div>
              <Group justify="space-between" className={styles.statRow}>
                <div>
                  <Text size="sm" c="dimmed">
                    Lowest day
                  </Text>
                  <Text fw={600}>1,690 kcal</Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Highest day
                  </Text>
                  <Text fw={600}>1,905 kcal</Text>
                </div>
                <div>
                  <Text size="sm" c="dimmed">
                    Goal band
                  </Text>
                  <Text fw={600}>1,700-1,900</Text>
                </div>
              </Group>
            </Stack>
          </Card>
        </SimpleGrid>
      </Container>
    </div>
  );
};
