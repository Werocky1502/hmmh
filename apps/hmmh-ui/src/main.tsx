import { StrictMode } from 'react';
import * as ReactDOM from 'react-dom/client';
import { MantineProvider, createTheme } from '@mantine/core';
import '@mantine/core/styles.css';
import App from './app/app';
import './styles.css';

const theme = createTheme({
  fontFamily: '"Space Grotesk", system-ui, sans-serif',
  headings: { fontFamily: '"Fraunces", "Space Grotesk", serif' },
  primaryColor: 'teal',
  defaultRadius: 'md',
});

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement,
);

root.render(
  <StrictMode>
    <MantineProvider theme={theme}>
      <App />
    </MantineProvider>
  </StrictMode>,
);
