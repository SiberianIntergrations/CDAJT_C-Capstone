import axios from "axios";
import { getAccessToken, getRefreshToken, setAuthTokens, clearAuthTokens } from "@/utils/token";

export const API_BASE_URL =
  (typeof process !== "undefined" && process.env.NEXT_PUBLIC_API_URL) || "http://localhost:5264";

export const api = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
});

// Add auth header interceptor
api.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
  },
  (error) => Promise.reject(error)
);

// Refresh token logic to prevent multiple refresh attempts/api spam
let isRefreshing = false;
let refreshSubscribers = [];

const subscribeTokenRefresh = (cb) => refreshSubscribers.push(cb);

const onRefreshed = (token) => {
  refreshSubscribers.forEach((cb) => cb(token));
  refreshSubscribers = [];
};

const redirectToLogin = () => {
  clearAuthTokens();
  if (typeof window !== "undefined") {
    window.location.href = "/auth/login";
  }
};

// Add response interceptor for token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error?.config;

    // Early return for non-auth errors or missing original request
    if (!error?.response || !originalRequest) {
      return Promise.reject(error);
    }

    const status = error.response.status;

    // Prevent infinite loops
    const isRefreshEndpoint = originalRequest.url?.includes("/auth/refresh");

    // Check if the error is 401 Unauthorized and the request is not retried
    if (status === 401 && !originalRequest._retry && !isRefreshEndpoint) {
      originalRequest._retry = true;

      const refreshToken = getRefreshToken();
      if (!refreshToken) {
        redirectToLogin();
        return Promise.reject(error);
      }

      // Queue request if a refresh is already in progress
      if (isRefreshing) {
        return new Promise((resolve,  reject) => {
          subscribeTokenRefresh((newToken) => {
            if (!newToken) return reject(error);
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
            resolve(api(originalRequest));
          });
        });
      }

      isRefreshing = true;

      try {
        // Request a new access token using the refresh token
        const response = await axios.post(
          `${API_BASE_URL}/api/auth/refresh`,
          { refresh_token: refreshToken },
          { headers: { "Content-Type": "application/json" } }
        );

        const { access_token } = response.data;

        if (!access_token) {
          throw new Error("No access token in refresh response");
        }

        // Store the new access token and notify subscribers
        setAuthTokens("access_token", access_token);
        onRefreshed(access_token);

        // Update the Authorization header and retry the original request
        originalRequest.headers.Authorization = `Bearer ${access_token}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Clear tokens and redirect to login if refresh fails
        console.error("Token refresh error:", refreshError);
        onRefreshed(null);
        redirectToLogin();
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    // Reject other errors
    return Promise.reject(error);
  }
);

export default api;