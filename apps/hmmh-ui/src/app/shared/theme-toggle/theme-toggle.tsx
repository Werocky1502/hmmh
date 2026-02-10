import { Switch, useComputedColorScheme, useMantineColorScheme } from '@mantine/core';
import { IconMoonStars, IconSun } from '@tabler/icons-react';

export const ThemeToggle = () => {
  const computedColorScheme = useComputedColorScheme('light');
  const { setColorScheme } = useMantineColorScheme();
  const isDark = computedColorScheme === 'dark';

  return (
    <Switch
      checked={isDark}
      onChange={(event) => setColorScheme(event.currentTarget.checked ? 'dark' : 'light')}
      size="lg"
      color="teal"
      aria-label="Toggle color scheme"
      onLabel={<IconSun size={18} />}
      offLabel={<IconMoonStars size={18} />}
    />
  );
};
