import axios from "axios";

// Transform PascalCase to camelCase for responses
const transformResponse = (data: any): any => {
  if (typeof data !== "object" || data === null) return data;
  
  if (Array.isArray(data)) {
    return data.map(transformResponse);
  }
  
  const transformed: any = {};
  for (const key in data) {
    if (Object.prototype.hasOwnProperty.call(data, key)) {
      // Convert PascalCase to camelCase
      const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
      transformed[camelKey] = transformResponse(data[key]);
    }
  }
  return transformed;
};

// Transform camelCase to PascalCase for requests
const transformRequest = (data: any): any => {
  if (typeof data !== "object" || data === null) return data;
  
  if (Array.isArray(data)) {
    return data.map(transformRequest);
  }
  
  const transformed: any = {};
  for (const key in data) {
    if (Object.prototype.hasOwnProperty.call(data, key)) {
      // Convert camelCase to PascalCase
      const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
      transformed[pascalKey] = transformRequest(data[key]);
    }
  }
  return transformed;
};

export const api = axios.create({
  baseURL: "http://localhost:5268/api", // .NET API HTTP port (use HTTP for local development)
  headers: { "Content-Type": "application/json" },
});

// Add token to requests if available (must be first)
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("authToken");
    if (token) {
      // Ensure Authorization header is set correctly
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
      // Debug: Log token presence (remove in production)
      console.log("Token attached to request:", config.url, "Token length:", token.length);
    } else {
      console.warn("No token found in localStorage for request:", config.url);
    }
    // Transform request data from camelCase to PascalCase
    if (config.data) {
      config.data = transformRequest(config.data);
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Handle 401 unauthorized responses and transform responses
api.interceptors.response.use(
  (response) => {
    // Transform response data from PascalCase to camelCase
    if (response.data) {
      response.data = transformResponse(response.data);
    }
    return response;
  },
  (error) => {
    // Transform error response if it exists
    if (error.response?.data) {
      error.response.data = transformResponse(error.response.data);
    }
    if (error.response?.status === 401) {
      // Token expired or invalid - clear storage
      // Don't redirect here, let the ProtectedRoute handle it
      // This prevents race conditions with React Router
      console.error("401 Unauthorized - Token may be invalid or expired");
      console.error("Request URL:", error.config?.url);
      console.error("Request headers:", error.config?.headers);
      localStorage.removeItem("authToken");
      localStorage.removeItem("authUser");
      // Use a custom event to notify AuthContext to update
      window.dispatchEvent(new Event("auth-logout"));
    }
    return Promise.reject(error);
  }
);