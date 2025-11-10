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

    //Skip MSAL for guest users
  if (typeof window !== "undefined" && localStorage.getItem("guest") === "true") {
    return config;
  }

  try {
    // Ensure MSAL is initialized
    await msalInstance.initialize();

    // Get the currently signed-in account
    let activeAccount = msalInstance.getActiveAccount();

    // If no active account is set, try to get all accounts and set the first one
    if (!activeAccount) {
      const accounts = msalInstance.getAllAccounts();
      if (accounts && accounts.length > 0) {
        activeAccount = accounts[0];
        msalInstance.setActiveAccount(activeAccount);
        console.log("Set active account:", activeAccount.username);
      } else {
        throw new Error("No active account! Please sign in before making API calls.");
      }
    }

    // Acquire token silently for the active account
    const tokenResponse = await msalInstance.acquireTokenSilent({
      ...loginRequest,
      account: activeAccount,
    });

    console.log("Token acquired successfully. Scopes:", tokenResponse.scopes);
    console.log("Token claims:", tokenResponse.idTokenClaims);
    console.log("Access Token (first 50 chars):", tokenResponse.accessToken.substring(0, 50) + "...");
    console.log("Token audience (aud):", tokenResponse.idTokenClaims?.aud);
    console.log("Token issuer (iss):", tokenResponse.idTokenClaims?.iss);
    config.headers.Authorization = `Bearer ${tokenResponse.accessToken}`;
    return config;

  } catch (error) {
    console.warn("Silent token acquisition failed:", error);

    // Optional fallback to popup if the token is expired or missing
    try {
      const popupResponse = await msalInstance.acquireTokenPopup(loginRequest);
      console.log("Token acquired via popup. Scopes:", popupResponse.scopes);
      config.headers.Authorization = `Bearer ${popupResponse.accessToken}`;
      return config;
    } catch (popupError) {
      console.error("Token acquisition failed completely:", popupError);
      return Promise.reject(popupError);
    }
  }
});

export default api;
