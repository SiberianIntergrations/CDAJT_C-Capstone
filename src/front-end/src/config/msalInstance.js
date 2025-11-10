import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './auth';

const msalInstance = new PublicClientApplication(msalConfig);

msalInstance.initialize().catch((error) => {
  console.error("MSAL initialization failed:", error);
});

// Function to ensure MSAL is initialized
export const initializeMsal = () => {
  if (!initPromise) {
    initPromise = msalInstance.initialize();
  }
  return initPromise;
};

export default msalInstance;