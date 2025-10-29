import { useState, useEffect } from "react";
import { useMsal } from "@azure/msal-react";
import { silentRequest } from "@/config/auth";


export const useAuth = () => {
    const { instance: msalInstance } = useMsal();
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [userId, setUserId] = useState(null);
    const [userEmail, setUserEmail] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Only run on client
        if (typeof window === "undefined") return;

        let isMounted = true;
        setLoading(true);

        const accounts = msalInstance.getAllAccounts();
        const account = accounts && accounts.length > 0 ? accounts[0] : null;
        if (account) {
            msalInstance.setActiveAccount(account);
            setIsAuthenticated(true);
            setUserId(account.localAccountId || account.homeAccountId || null);
            setUserEmail(account.username || null);

            msalInstance.acquireTokenSilent({ ...silentRequest, account }).then((response) => {
                let rawRoles = response.idTokenClaims?.roles || response.idTokenClaims?.role || [];
                const roleList = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
                // Map Azure AD roles to app roles
                if (roleList.includes("user.Admin")) setUserRole("admin");
                else if (roleList.includes("user.Staff")) setUserRole("staff");
                else setUserRole("customer");
                setLoading(false);
            }).catch(() => {
                setIsAuthenticated(false);
                setUserRole(null);
                setUserId(null);
                setUserEmail(null);
                setLoading(false);
            });
        } else {
            setIsAuthenticated(false);
            setUserRole(null);
            setUserId(null);
            setUserEmail(null);
            setLoading(false);
        }

        // Listen for MSAL account changes (e.g., login/logout in other tabs)
        const handleMsalChange = () => {
            const accounts = msalInstance.getAllAccounts();
            const account = accounts && accounts.length > 0 ? accounts[0] : null;
            if (account) {
                msalInstance.setActiveAccount(account);
                setIsAuthenticated(true);
                setUserId(account.localAccountId || account.homeAccountId || null);
                setUserEmail(account.username || null);

                msalInstance.acquireTokenSilent({ ...silentRequest, account }).then((response) => {
                    let rawRoles = response.idTokenClaims?.roles || response.idTokenClaims?.role || [];
                    const roleList = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
                    if (roleList.includes("user.Admin")) setUserRole("admin");
                    else if (roleList.includes("user.Staff")) setUserRole("staff");
                    else setUserRole("customer");
                    setLoading(false);
                }).catch(() => {
                    setIsAuthenticated(false);
                    setUserRole(null);
                    setUserId(null);
                    setUserEmail(null);
                    setLoading(false);
                });
            } else {
                setIsAuthenticated(false);
                setUserRole(null);
                setUserId(null);
                setUserEmail(null);
                setLoading(false);
            }
        };

        window.addEventListener("msal:accountChanged", handleMsalChange);

        return () => {
            isMounted = false;
            window.removeEventListener("msal:accountChanged", handleMsalChange);
        };
    }, [msalInstance]);

    // Check if user has a specific role
    const hasRole = (role) => {
        return userRole === role;
    };

    // Check if user has any of the specified roles
    const hasAnyRole = (roles) => {
        return roles.includes(userRole);
    };

    console.log("auth hook - isAuthenticated:", isAuthenticated, "userRole:", userRole, "loading:", loading, "userId:", userId, "userEmail:", userEmail);

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