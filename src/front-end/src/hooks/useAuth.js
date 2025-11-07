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
    const [authChanged, setAuthChanged] = useState(0); // Add this state

    useEffect(() => {
        // Only run on client
        if (typeof window === "undefined") return;

        let isMounted = true;

        const initAndCheckAuth = async () => {
            setLoading(true); 
            await msalInstance.initialize();
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
        };

        initAndCheckAuth();

        // Listen for MSAL account changes and custom auth changed event
        const handleMsalChange = () => {
            setAuthChanged((prev) => prev + 1);
        };
        window.addEventListener("msal:accountChanged", handleMsalChange);
        window.addEventListener("auth:changed", handleMsalChange);

        return () => {
            isMounted = false;
            window.removeEventListener("msal:accountChanged", handleMsalChange);
            window.removeEventListener("auth:changed", handleMsalChange);
        };
    // Add authChanged to dependency array to rerun effect
    }, [msalInstance, authChanged]);

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
