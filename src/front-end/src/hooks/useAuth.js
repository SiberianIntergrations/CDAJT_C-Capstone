import { useState, useEffect } from "react";
import {
    getAccessToken,
    clearAuthTokens,
    getUserRole,
    getUserId,
    getUserEmail,
    isTokenExpired,
    AUTH_EVENT
} from "@/utils/token";

export const useAuth = () => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [userId, setUserId] = useState(null);
    const [userEmail, setUserEmail] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkAuth = () => {
            const token = getAccessToken();

            if (!token) {
                console.log("useAuth: No token found.");
                setIsAuthenticated(false);
                setUserRole(null);
                setUserId(null);
                setUserEmail(null);
                setLoading(false);
                return;
            }

            // Check if token is expired
            if (isTokenExpired(token)) {
                clearAuthTokens();
                setIsAuthenticated(false);
                setUserRole(null);
                setUserId(null);
                setUserEmail(null);
                setLoading(false);
                return;
            }

            // Decode and set user info
            try {
                console.log(token);
                const role = getUserRole(token);
                const id = getUserId(token);
                const email = getUserEmail(token);

                if (role) {
                    setUserRole(role);
                    setUserId(id);
                    setUserEmail(email);
                    setIsAuthenticated(true);
                } else {
                    throw new Error("Invalid token");
                }
            } catch (error) {
                console.error("Error decoding token:", error);
                clearAuthTokens();
                setIsAuthenticated(false);
                setUserRole(null);
                setUserId(null);
                setUserEmail(null);
            } finally {
                setLoading(false);
            }
        };

        checkAuth();

        // Listen for storage changes (e.g., login/logout in other tabs)
        const handleStorageChange = (event) => {
            if (event.key === "access_token") {
                checkAuth();
            }
        };
        window.addEventListener("storage", handleStorageChange);

        // same-tab updates
        const handleAuthChange = () => {
            checkAuth();
        };
        window.addEventListener(AUTH_EVENT, handleAuthChange);

        return () => {
            window.removeEventListener("storage", handleStorageChange);
            window.removeEventListener(AUTH_EVENT, handleAuthChange);
        };
    }, []);

    // Check if user has a specific role
    const hasRole = (role) => {
        return userRole === role;
    };

    // Check if user has any of the specified roles
    const hasAnyRole = (roles) => {
        return roles.includes(userRole);
    };

    return {
        isAuthenticated,
        userRole,
        userId,
        userEmail,
        loading,
        hasRole,
        hasAnyRole,
    };
};

export default useAuth;