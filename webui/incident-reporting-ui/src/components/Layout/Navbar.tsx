import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

export default function Navbar() {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="h-16 flex items-center justify-between px-6 bg-white border-b border-gray-200 shadow-sm">
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 bg-gradient-to-br from-blue-600 to-blue-700 rounded-lg flex items-center justify-center">
          <span className="text-white font-bold text-sm">IR</span>
        </div>
        <h1 className="text-xl font-bold text-gray-900">Incident Reporting</h1>
      </div>

      <div className="flex items-center gap-3">
        {isAuthenticated && user && (
          <>
            <div className="flex items-center gap-2 px-3 py-1 bg-gray-100 rounded-lg">
              <span className="text-sm font-medium text-gray-700">
                👤 {user.username}
              </span>
            </div>
            <button
              onClick={handleLogout}
              className="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors font-medium"
            >
              Logout
            </button>
          </>
        )}
      </div>
    </header>
  );
}
  