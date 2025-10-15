import axios from "axios";

export const API_BASE_URL =
  (typeof process !== "undefined" && process.env.NEXT_PUBLIC_API_URL) || "http://localhost:5264";

// Added for SSR safety with localStorage to prevent build failures
const getLocal = (key) =>
  typeof window !== "undefined" ? localStorage.getItem(key) : null;
const setLocal = (key, value) =>
  typeof window !== "undefined" ? localStorage.setItem(key, value) : undefined;
const removeLocal = (key) =>
  typeof window !== "undefined" ? localStorage.removeItem(key) : undefined;

export const api = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  timeout: 10000,
});

// Add auth header interceptor
api.interceptors.request.use((config) => {
  const token = getLocal("access_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Refresh token logic to prevent multiple refresh attempts/api spam
let isRefreshing = false;
let refreshSubscribers = [];

const subscribeTokenRefresh = (cb) => refreshSubscribers.push(cb);
const onRefreshed = (token) => {
  refreshSubscribers.forEach((cb) => cb(token));
  refreshSubscribers = [];
};

const clearAndRedirectToLogin = () => {
  removeLocal("access_token");
  removeLocal("refresh_token");
  if (typeof window !== "undefined") {
    window.location.href = "/auth/login";
  }
};

// Add response interceptor for token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error?.config;

    if (!error?.response || !originalRequest) {
      return Promise.reject(error);
    }

    const status = error.response.status;

    // Prevent infinite loops
    const isRefreshEndpoint = originalRequest.url?.includes("/auth/refresh");

    // Check if the error is 401 Unauthorized and the request is not retried
    if (status === 401 && !originalRequest._retry && !isRefreshEndpoint) {
      originalRequest._retry = true;

      const refreshToken = getLocal("refresh_token");
      if (!refreshToken) {
        clearAndRedirectToLogin();
        return Promise.reject(error);
      }

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
        // Store the new access token
        setLocal("access_token", access_token);
        onRefreshed(access_token);

        // Update the Authorization header and retry the original request
        originalRequest.headers.Authorization = `Bearer ${access_token}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Clear tokens and redirect to login if refresh fails
        onRefreshed(null);
        clearAndRedirectToLogin();
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
