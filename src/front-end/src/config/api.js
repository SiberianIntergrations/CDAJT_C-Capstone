import axios from "axios";
import msalInstance from "@/config/msalInstance";
import { silentRequest } from "@/config/auth";
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
api.interceptors.request.use(async (config) => {
    try {
      // Get active account (the currently signed in user)
      const activeAccount = msalInstance.getActiveAccount()
      
      if (!activeAccount) {
        throw new Error('No active account! Please sign in before making API calls.');
      }

      // Attempt to acquire token silently
      const response = await msalInstance.acquireTokenSilent(silentRequest);
      
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

export default api;