import axios from "axios";
import { useState, useEffect } from "react";
import { axiosInstance, createApiUrl } from "@/config/api";

// Token management
export const getAuthToken = () => {
  if (typeof window !== "undefined") {
    return localStorage.getItem("access_token");
  }
  return null;
};

export const getRefreshToken = () => {
  if (typeof window !== "undefined") {
    return localStorage.getItem("refresh_token");
  }
  return null;
};

export const setAuthTokens = (accessToken, refreshToken) => {
  localStorage.setItem("access_token", accessToken);
  if (refreshToken) {
    localStorage.setItem("refresh_token", refreshToken);
  }
};

export const clearAuthTokens = () => {
  localStorage.removeItem("access_token");
  localStorage.removeItem("refresh_token");
};

// API calls
export const loginUser = async (email, password) => {
  const response = await axiosInstance.post(createApiUrl("/auth/login"), {
    email,
    password,
  });
  return response.data;
};

export const registerUser = async (userData) => {
  const response = await axiosInstance.post(
    createApiUrl("/auth/register"),
    userData
  );
  return response.data;
};

export const requestPasswordReset = async (email) => {
  const response = await axiosInstance.post(
    createApiUrl("/auth/forgot-password"),
    { email }
  );
  return response.data;
};

export const resetPassword = async (token, newPassword) => {
  const response = await axiosInstance.post(
    createApiUrl("/auth/reset-password"),
    { token, new_password: newPassword }
  );
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
