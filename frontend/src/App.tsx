import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import { AnimatePresence } from "framer-motion";
import { AppShell, ThemeInitializer, ThemeToaster } from "./components";
import { LoginPage, RegisterPage, TasksPage, TodayPage, TeamPage } from "./pages";
import { useMeQuery } from "./api/hooks";

function AuthGuard() {
  const session = useMeQuery();
  if (session.isPending) return <main className="grid min-h-screen place-items-center text-sm text-stone-500">Загрузка...</main>;
  if (session.isError) return <Navigate to="/login" replace />;
  return <AppShell />;
}

function PublicRoute({ children }: { children: React.ReactNode }) { const session = useMeQuery(); if (session.isPending) return <main className="grid min-h-screen place-items-center text-sm text-stone-500">Загрузка...</main>; return session.isSuccess ? <Navigate to="/today" replace /> : <>{children}</>; }

export function App() {
  const location = useLocation();
  return <><ThemeInitializer /><ThemeToaster /><AnimatePresence mode="wait"><Routes location={location} key={location.pathname}>
    <Route path="/login" element={<PublicRoute><LoginPage /></PublicRoute>} /><Route path="/register" element={<PublicRoute><RegisterPage /></PublicRoute>} />
    <Route element={<AuthGuard />}><Route index element={<Navigate to="/today" replace />} /><Route path="/today" element={<TodayPage />} /><Route path="/tasks" element={<TasksPage />} /><Route path="/team" element={<TeamPage />} /></Route>
  </Routes></AnimatePresence></>;
}
