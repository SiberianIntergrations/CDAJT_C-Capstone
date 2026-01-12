import api from "@/config/api";
import { decodeToken, getUserRoles } from "@/config/auth";

// Import dispatchAuthChange from useAuth hook
let dispatchAuthChange;
if (typeof window !== 'undefined') {
  import("@/hooks/useAuth").then(module => {
    dispatchAuthChange = module.dispatchAuthChange;
  });
}

/**
 * Get stored auth token
 */
export const getAuthToken = () => {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem("authToken");
};

/**
 * Login user with email and password
 */
export const loginUser = async (email, password) => {
  const response = await api.post("/auth/login", {
    email,
    password,
  });
  
  console.log(`Auth.js Response:`, response.data);
  
  if (response.data.access_token) {
    localStorage.setItem("authToken", response.data.access_token);
    localStorage.setItem("access_token", response.data.access_token);
    
    if (response.data.expires_in) {
      const expiresAt = Date.now() + (response.data.expires_in * 1000);
      localStorage.setItem("tokenExpiresAt", expiresAt.toString());
    }
    
    if (response.data.user) {
      localStorage.setItem("user", JSON.stringify(response.data.user));
    }
    
    // Dispatch auth change event
    if (typeof window !== 'undefined' && dispatchAuthChange) {
      dispatchAuthChange();
    }
  }

  return response.data;
};

/**
 * Logout user
 */
export const logoutUser = () => {
  localStorage.removeItem("authToken");
  localStorage.removeItem("access_token");
  localStorage.removeItem("tokenExpiresAt");
  localStorage.removeItem("user");
  localStorage.removeItem("guest");
  
  // Dispatch auth change event
  if (typeof window !== 'undefined' && dispatchAuthChange) {
    dispatchAuthChange();
  }
  
  if (typeof window !== 'undefined') {
    window.location.href = "/";
  }
};

/**
 * Get current authenticated user
 */
export const getCurrentUser = async () => {
  const response = await api.get("/auth/me");
  return response.data;
};

/**
 * Check if user is authenticated
 */
export const isAuthenticated = () => {
  if (typeof window === 'undefined') return false;
  
  const token = localStorage.getItem("authToken");
  if (!token) return false;
  
  const claims = decodeToken(token);
  if (!claims || !claims.exp) return false;
  
  return claims.exp * 1000 > Date.now();
};

/**
 * Get current user's roles
 */
export const getCurrentUserRoles = () => {
  const token = getAuthToken();
  return getUserRoles(token);
};