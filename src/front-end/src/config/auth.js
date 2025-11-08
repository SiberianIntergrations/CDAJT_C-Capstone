import { LogLevel } from '@azure/msal-browser';
import { useRouter } from "next/navigation";

export const API_SCOPE =
  process.env.NEXT_PUBLIC_API_SCOPE || "api://99ffb099-af80-41d1-9c16-e844f8ed5308/user_impersonation";

export const msalConfig = {
  auth: {
    clientId: process.env.NEXT_PUBLIC_API_CLIENTID ||  '99ffb099-af80-41d1-9c16-e844f8ed5308',
    authority: process.env.NEXT_PUBLIC_API_AUTHORITY || `https://renovationstationexsm3943.ciamlogin.com/`,
    redirectUri: '/',
  },
  cache: {
    cacheLocation: 'localStorage', // This configures where your cache will be stored
    storeAuthStateInCookie: false, // Set this to "true" if you want to use cookies to store auth state
},
  system: {
    loggerOptions: {
        /**
         * Below you can configure MSAL.js logs. For more information, visit:
         * https://docs.microsoft.com/azure/active-directory/develop/msal-logging-js
         */
        loggerCallback: (level, message, containsPii) => {
            if (containsPii) {
                return;
            }
            switch (level) {
                case LogLevel.Error:
                    console.error(message);
                    return;
                case LogLevel.Info:
                    // console.info(message);
                    return;
                case LogLevel.Verbose:
                    console.debug(message);
                    return;
                case LogLevel.Warning:
                    console.warn(message);
                    return;
                default:
                    return;
            }
        },
    },
},
};

export const createLoginRequest = (redirectUri) => ({
    ...loginRequest,
    redirectUri,
})

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit:
 * https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
export const loginRequest = {
    scopes: ["openid", "offline_access", "profile", API_SCOPE],
};

 export const silentRequest = {
     scopes: ["openid", "offline_access", "profile", API_SCOPE],
     loginHint: "example@renovationstationexsm3943.onmicrosoft.com"
 };
 

//to use: userHasAnyRole(msalInstance.getActiveAccount()?.idTokenClaims, ["admin.UpdateOTISStatus"])
export function userHasAnyRole(claims, rolesToCheck) {
    if (claims = undefined) {
        return false;
    }
    
    if (typeof claims !== 'object' || !Array.isArray(rolesToCheck)) {
        throw new Error("Invalid arguments: claims must be an object and rolesToCheck must be an array.");
    }

    const roles = claims.roles || []; // Extract the "roles" claim from the access token
    if (!Array.isArray(roles)) {
        throw new Error("Invalid access token: 'roles' claim must be an array.");
    }

    return rolesToCheck.some(role => roles.includes(role));
}