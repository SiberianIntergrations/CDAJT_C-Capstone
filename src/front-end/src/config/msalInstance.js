import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './auth';

let msalInstance;

if (!msalInstance) {
  msalInstance = new PublicClientApplication(msalConfig);
}

export default msalInstance;
