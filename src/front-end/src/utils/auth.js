import api from "@/config/api";
import { setAuthTokens, clearAuthTokens } from "@/utils/token";
import { msalConfig } from "@/config/auth";
import msalInstance from "@/config/msalInstance";

// API calls
export const loginUser = async () => {
  // Interactive login using MSAL
  try {
    const loginResponse = await msalInstance.loginPopup({
      scopes: msalConfig.auth.scopes || ["openid", "profile", "email"],
    });
    // loginResponse.account contains user info
    return loginResponse;
  } catch (error) {
    console.error("MSAL login error:", error);
    throw error;
  }
};

export const logoutUser = async () => {
  try {
    await msalInstance.logoutPopup();
    if (typeof window !== "undefined") {
      window.location.href = "/";
    }
  } catch (error) {
    console.error("MSAL logout error:", error);
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