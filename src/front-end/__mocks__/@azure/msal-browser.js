// Mock @azure/msal-browser
export class PublicClientApplication {
  constructor(config) {
    this.config = config;
  }

  async initialize() {
    return Promise.resolve();
  }

  getAllAccounts() {
    return [];
  }

  async loginPopup(request) {
    return Promise.resolve({
      account: {
        username: 'test@example.com',
        name: 'Test User',
        localAccountId: '123',
        homeAccountId: '456',
      },
      idToken: 'mock-id-token',
      accessToken: 'mock-access-token',
    });
  }

  async logoutPopup() {
    return Promise.resolve();
  }

  async acquireTokenSilent() {
    return Promise.resolve({
      accessToken: 'mock-access-token',
    });
  }
}

export const InteractionStatus = {
  None: 'none',
  Login: 'login',
  Logout: 'logout',
  AcquireToken: 'acquireToken',
};

export const InteractionType = {
  Redirect: 'redirect',
  Popup: 'popup',
  Silent: 'silent',
};
