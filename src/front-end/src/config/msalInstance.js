import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './auth';

let msalInstance;
let initPromise = null;

if (!msalInstance) {
  msalInstance = new PublicClientApplication(msalConfig);
}

// Function to ensure MSAL is initialized
export const initializeMsal = () => {
  if (!initPromise) {
    initPromise = msalInstance.initialize();
  }
  return initPromise;
};

export default msalInstance;