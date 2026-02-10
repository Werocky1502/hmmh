import type { ReactElement } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { useAuth } from './auth/auth-context';
import { CaloriesPage } from './pages/calories/calories-page';
import { DashboardPage } from './pages/dashboard/dashboard-page';
import { LandingPage } from './pages/landing-page';
import { LoginPage } from './pages/login/login-page';
import { WeightsPage } from './pages/weights/weights-page';

const RequireAuth = ({ children }: { children: ReactElement }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

export function App() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/dashboard"
        element={
          <RequireAuth>
            <DashboardPage />
          </RequireAuth>
        }
      />
      <Route
        path="/weights"
        element={
          <RequireAuth>
            <WeightsPage />
          </RequireAuth>
        }
      />
      <Route
        path="/calories"
        element={
          <RequireAuth>
            <CaloriesPage />
          </RequireAuth>
        }
      />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
