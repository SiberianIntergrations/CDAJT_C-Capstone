import { useState, useEffect } from "react";
import api from "@/config/api";
import { setAuthTokens, clearAuthTokens } from "@/utils/token";

// // Token management
// export const getAuthToken = () => {
//   if (typeof window !== "undefined") {
//     return localStorage.getItem("access_token");
//   }
//   return null;
// };

// export const getRefreshToken = () => {
//   if (typeof window !== "undefined") {
//     return localStorage.getItem("refresh_token");
//   }
//   return null;
// };

// export const setAuthTokens = (accessToken, refreshToken) => {
//   if (typeof window !== "undefined") {
//     localStorage.setItem("access_token", accessToken);
//     if (refreshToken) {
//       localStorage.setItem("refresh_token", refreshToken);
//     }
//   }
// };

// export const clearAuthTokens = () => {
//   if (typeof window !== "undefined") {
//       localStorage.removeItem("access_token");
//       localStorage.removeItem("refresh_token");
//     }
// };

// API calls
export const loginUser = async (email, password) => {
  const response = await api.post("/auth/login", {
    email,
    password,
  });

  // Automatically store tokens on login
  const { access_token, refresh_token } = response.data || {};
  if (!access_token) {
    throw new Error("No access token returned from login");
  };
    setAuthTokens(access_token, refresh_token);
  console.log("Login response data:", response.data);

  return response.data;
};

export const logoutUser = async () => {
  try {
    clearAuthTokens();
    if (typeof window !== "undefined") {
      window.location.href = "/";
    }
  } catch (error) {
    console.error("Error during logout:", error);
    clearAuthTokens();
  }
};

export const registerUser = async (userData) => {
  const response = await api.post("/auth/register", userData);
  return response.data;
};

export const requestPasswordReset = async (email) => {
  const response = await api.post("/auth/forgot-password", { email });
  return response.data;
};

export const resetPassword = async (token, newPassword) => {
  const response = await api.post("/auth/reset-password", { 
    token,
    new_password: newPassword
  });
  return response.data;
};

// Custom hook for authentication state
export const useAuth = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userRole, setUserRole] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = getAuthToken();
    if (token) {
      try {
        const decodedToken = JSON.parse(atob(token.split(".")[1]));
        setUserRole(decodedToken.role);
        setIsAuthenticated(true);
      } catch (error) {
        clearAuthTokens();
        setIsAuthenticated(false);
        setUserRole(null);
      }
    } else {
      setIsAuthenticated(false);
      setUserRole(null);
    }
    setLoading(false);
  }, []);

  return { isAuthenticated, userRole, loading };
};
