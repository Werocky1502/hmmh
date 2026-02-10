import { StrictMode } from 'react';
import * as ReactDOM from 'react-dom/client';
import { MantineProvider, createTheme, localStorageColorSchemeManager } from '@mantine/core';
import { BrowserRouter } from 'react-router-dom';
import '@mantine/core/styles.css';
import '@mantine/charts/styles.css';
import '@mantine/dates/styles.css';
import App from './app/app';
import { AuthProvider } from './app/auth/auth-context';
import './styles.css';

const theme = createTheme({
  primaryColor: 'teal',
  defaultRadius: 'md',
});

const colorSchemeManager = localStorageColorSchemeManager({ key: 'hmmh-color-scheme' });

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement,
);

root.render(
  <StrictMode>
    <MantineProvider theme={theme} defaultColorScheme="auto" colorSchemeManager={colorSchemeManager}>
      <BrowserRouter>
        <AuthProvider>
          <App />
        </AuthProvider>
      </BrowserRouter>
    </MantineProvider>
  </StrictMode>,
);
