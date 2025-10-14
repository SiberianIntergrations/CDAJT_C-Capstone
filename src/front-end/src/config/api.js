import { API_SCOPE } from "@/config/authconfig";



export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5264";

// Utility function to create URLs
export const createApiUrl = (path) => {
  return `${API_BASE_URL}/api${path}`;
};

// Common headers for API requests
export const getAuthHeaders = () => {
  const token = localStorage.getItem("access_token");
  return {
    Authorization: token ? `Bearer ${token}` : "",
    "Content-Type": "application/json",
  };
};

// Axios instance configuration
import axios from "axios";

export const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
});

// Add auth header interceptor
axiosInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem("access_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Add response interceptor for token refresh
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Check if the error is 401 Unauthorized and the request is not retried
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem("refresh_token");
        if (!refreshToken) {
          throw new Error("No refresh token available");
        }

        // Request a new access token using the refresh token
        const response = await axios.post(
          createApiUrl("api/auth/refresh"),
          { refresh_token: refreshToken },
          { headers: { "Content-Type": "application/json" } }
        );

        const { access_token } = response.data;

        // Store the new access token
        localStorage.setItem("access_token", access_token);

        // Update the Authorization header and retry the original request
        originalRequest.headers.Authorization = `Bearer ${access_token}`;
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        // Clear tokens and redirect to login if refresh fails
        localStorage.removeItem("access_token");
        localStorage.removeItem("refresh_token");
        window.location.href = "/api/auth/login";
        return Promise.reject(refreshError);
      }
    }

    // Reject other errors
    return Promise.reject(error);
  }
);


export const msalAxiosClient = (msalInstance) => {
  const axiosInstance = axios.create({
    baseURL: API_BASE_URL,
    headers: {},
  });

  // Add a request interceptor to handle authentication
  axiosInstance.interceptors.request.use(async (config) => {
    try {
      // Get active account (the currently signed in user)
      const activeAccount = msalInstance.getActiveAccount()
      
      if (!activeAccount) {
        throw new Error('No active account! Please sign in before making API calls.');
      }

      // Attempt to acquire token silently
      const response = await msalInstance.acquireTokenSilent({
        scopes: [API_SCOPE],
        account: activeAccount
      });
      
      // Add the token to the Authorization header
      config.headers.Authorization = `Bearer ${response.accessToken}`;
      
      return config;
    } catch (error) {
      console.error("Error acquiring token silently:", error);

      // If silent token acquisition fails, attempt to acquire token via popup
      try {
        const popupResponse = await msalInstance.acquireTokenPopup({ scopes: apiScopes });
        config.headers.Authorization = `Bearer ${popupResponse.accessToken}`;
        return config;
      } catch (popupError) {
        console.error("Error acquiring token via popup:", popupError);
        return Promise.reject(popupError);
      }
    }
  });
  return axiosInstance;
};