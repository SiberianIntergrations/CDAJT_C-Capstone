import axios from "axios";
import msalInstance from "@/config/msalInstance";
import { loginRequest } from "@/config/auth"; // use loginRequest, not silentRequest

export const API_BASE_URL =
  (typeof process !== "undefined" && process.env.NEXT_PUBLIC_API_URL) ||
  "http://localhost:5264";

export const api = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(async (config) => {
  try {
    // Get the currently signed-in account
    const activeAccount = msalInstance.getActiveAccount();
    if (!activeAccount) {
      throw new Error("No active account! Please sign in before making API calls.");
    }

    // Acquire token silently for the active account
    const tokenResponse = await msalInstance.acquireTokenSilent({
      ...loginRequest,
      account: activeAccount,
    });

    config.headers.Authorization = `Bearer ${tokenResponse.accessToken}`;
    return config;

  } catch (error) {
    console.warn("⚠️ Silent token acquisition failed:", error);

    // Optional fallback to popup if the token is expired or missing
    try {
      const popupResponse = await msalInstance.acquireTokenPopup(loginRequest);
      config.headers.Authorization = `Bearer ${popupResponse.accessToken}`;
      return config;
    } catch (popupError) {
      console.error("Token acquisition failed completely:", popupError);
      return Promise.reject(popupError);
    }
  }
});

export default api;
