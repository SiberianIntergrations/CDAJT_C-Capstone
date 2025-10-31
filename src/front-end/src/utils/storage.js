/**
 * SSR-safe localStorage utility to prevent build failures when accessing localStorage during server-side rendering.
 */

const storage = {
  // Get item from localStorage
  get: (key) => {
    if (typeof window === "undefined") return null;
    try {
      return localStorage.getItem(key);
    } catch (error) {
      console.error("Error accessing localStorage:", error);
      return null;
    }
  },

  // Set item in localStorage
  set: (key, value) => {
    if (typeof window === "undefined") return;
    try {
      localStorage.setItem(key, value);
    } catch (error) {
      console.error("Error accessing localStorage:", error);
    }
  },

  // Remove item from localStorage
  remove: (key) => {
    if (typeof window === "undefined") return;
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error("Error accessing localStorage:", error);
    }
  },
};

export default storage;
