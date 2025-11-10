import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './auth';

const msalInstance = new PublicClientApplication(msalConfig);

msalInstance.initialize().catch((error) => {
  console.error("MSAL initialization failed:", error);
});

export default msalInstance;
