import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./contexts/AuthContext";
import { NotificationProvider } from "./contexts/NotificationContext";
import Layout from "./components/Layout/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import Login from "./pages/Login.tsx";
import Register from "./pages/Register.tsx";
import Dashboard from "./pages/Dashboard.tsx";
import IncidentList from "./pages/IncidentList.tsx";
import IncidentCreate from "./pages/IncidentCreate.tsx";
import IncidentEdit from "./pages/IncidentEdit.tsx";

export default function App() {
  return (
    <AuthProvider>
      <NotificationProvider>
    <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/*"
            element={
              <ProtectedRoute>
                <Layout>
                  <Routes>
                    <Route path="/" element={<Dashboard />} />
          <Route path="/incidents" element={<IncidentList />} />
          <Route path="/create" element={<IncidentCreate />} />
          <Route path="/edit/:id" element={<IncidentEdit />} />
        </Routes>
      </Layout>
              </ProtectedRoute>
            }
          />
        </Routes>
    </BrowserRouter>
      </NotificationProvider>
    </AuthProvider>
  );
}
