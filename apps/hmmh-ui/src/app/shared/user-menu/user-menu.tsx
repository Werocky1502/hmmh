import { Avatar, Button, Group, Menu, Text } from '@mantine/core';
import { IconChevronDown, IconLogout, IconTrash } from '@tabler/icons-react';
import styles from './user-menu.module.css';

interface UserMenuProps {
  userName?: string | null;
  onLogout: () => void;
  onDelete: () => void;
}

export const UserMenu = ({ userName, onLogout, onDelete }: UserMenuProps) => {
  const initial = userName?.[0]?.toUpperCase() ?? 'U';

  return (
    <Menu width={200} position="bottom-end" shadow="md">
      <Menu.Target>
        <Button variant="subtle" rightSection={<IconChevronDown size={16} />} className={styles.userButton}>
          <Group gap="sm">
            <Avatar radius="xl" color="teal">
              {initial}
            </Avatar>
            <Text fw={600}>{userName ?? 'User'}</Text>
          </Group>
        </Button>
      </Menu.Target>
      <Menu.Dropdown>
        <Menu.Item leftSection={<IconLogout size={16} />} onClick={onLogout}>
          Logout
        </Menu.Item>
        <Menu.Item color="red" leftSection={<IconTrash size={16} />} onClick={onDelete}>
          Delete account
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
};
