import { createContext, useContext, useState, useEffect } from "react";
import type { ReactNode } from "react";

interface AuthUser {
  username: string;
  email: string;
}

interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  login: (token: string, user: AuthUser) => void;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Helper function to load auth state from localStorage
function loadAuthState() {
  const storedToken = localStorage.getItem("authToken");
  const storedUser = localStorage.getItem("authUser");
  
  if (storedToken && storedUser) {
    try {
      return {
        token: storedToken,
        user: JSON.parse(storedUser) as AuthUser,
      };
    } catch {
      // If parsing fails, clear invalid data
      localStorage.removeItem("authToken");
      localStorage.removeItem("authUser");
    }
  }
  
  return { token: null, user: null };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  // Initialize state from localStorage immediately (synchronous)
  const initialState = loadAuthState();
  const [user, setUser] = useState<AuthUser | null>(initialState.user);
  const [token, setToken] = useState<string | null>(initialState.token);

  useEffect(() => {
    // Verify token is still valid on mount
    const storedToken = localStorage.getItem("authToken");
    const storedUser = localStorage.getItem("authUser");
    
    if (storedToken && storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser);
        setToken(storedToken);
        setUser(parsedUser);
      } catch {
        // If parsing fails, clear invalid data
        localStorage.removeItem("authToken");
        localStorage.removeItem("authUser");
        setToken(null);
        setUser(null);
      }
    }

    // Listen for logout events from axios interceptor
    const handleLogout = () => {
      setToken(null);
      setUser(null);
    };

    window.addEventListener("auth-logout", handleLogout);
    return () => {
      window.removeEventListener("auth-logout", handleLogout);
    };
  }, []);

  const login = (newToken: string, newUser: AuthUser) => {
    // Update state immediately
    setToken(newToken);
    setUser(newUser);
    // Persist to localStorage
    localStorage.setItem("authToken", newToken);
    localStorage.setItem("authUser", JSON.stringify(newUser));
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem("authToken");
    localStorage.removeItem("authUser");
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        login,
        logout,
        isAuthenticated: !!token && !!user,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}

