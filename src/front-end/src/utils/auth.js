import api from "@/config/api";
import { setAuthTokens, clearAuthTokens } from "@/utils/token";

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
  }
  setAuthTokens(access_token, refresh_token);

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
    new_password: newPassword,
  });
  return response.data;
};
