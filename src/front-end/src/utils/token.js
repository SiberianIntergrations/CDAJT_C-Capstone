import { jwtDecode } from "jwt-decode";
import storage from "./storage";

// Token storage keys
const TOKEN_KEYS = {
  ACCESS: "access_token",
  REFRESH: "refresh_token",
};

export const AUTH_EVENT = "auth_change";

// Dispatch auth change event for same-tab listeners - used for updating auth state
// in components/hooks when tokens change
const dispatchAuthChange = () => {
  if (typeof window !== "undefined") {
    window.dispatchEvent(new Event(AUTH_EVENT));
  }
};

// Retrieve auth tokens from storage
export const getAccessToken = () => storage.get(TOKEN_KEYS.ACCESS);
export const getRefreshToken = () => storage.get(TOKEN_KEYS.REFRESH);

// Store auth tokens in storage
export const setAuthTokens = (accessToken, refreshToken) => {
  if (accessToken) {
    storage.set(TOKEN_KEYS.ACCESS, accessToken);
  }
  if (refreshToken) {
    storage.set(TOKEN_KEYS.REFRESH, refreshToken);
  }
  dispatchAuthChange();
};

// Clear auth tokens from storage
export const clearAuthTokens = () => {
  storage.remove(TOKEN_KEYS.ACCESS);
  storage.remove(TOKEN_KEYS.REFRESH);
  dispatchAuthChange();
};

// Decode JWT token
export const decodeToken = (token) => {
  if (!token) return null;
  try {
    const decoded = jwtDecode(token);
    console.log("Decoded token:", decoded);
    return decoded;
  } catch (error) {
    console.error("Error decoding token:", error?.message);
    return null;
  }
};

// Check if token is expired
export const isTokenExpired = (token) => {
  const decoded = decodeToken(token);
  if (!decoded || !decoded.exp) return true;
  const currentTime = Math.floor(Date.now() / 1000);
  return decoded.exp < currentTime;
};

// Helper function to map backend roles to frontend roles
export const normalizedUserRole = (role) => {
  if (!role) return null;
  const roleLower = role.toLowerCase();

  const roleMap = {
    'admin': 'admin',
    'employee': 'staff',
    'customer': 'customer',
  };
  const normalized = roleMap[roleLower] || 'customer';
  console.log(`Normalized role: ${role} -> ${normalized}`);
  return normalized;
};

// Get user role from token. Uses microsoft claims format if needed (extracted from previous project code).
export const getUserRole = (token) => {
  const tokenToUse = token || getAccessToken();
  const decoded = decodeToken(tokenToUse);
  if (!decoded) return null;

  let role = decoded.role;

  if (!role) {
    // Microsoft Identity Claims Format
    const msRoleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    role = decoded[msRoleClaim];
  }
  return normalizedUserRole(role);
};

export const getUserId = (token) => {
  const decoded = decodeToken(token || getAccessToken());
  return decoded?.sub || decoded?.user_id || decoded?.id || null;
}

export const getUserEmail = (token) => {
  const decoded = decodeToken(token || getAccessToken());
  return decoded?.email || null;
}

export const getUserClaims = (token) => {
  return decodeToken(token || getAccessToken());
}

// Check if user is authenticated
export const isAuthenticated = () => {
  const token = getAccessToken();
  if (!token) return false;
  return !isTokenExpired(token);
};
