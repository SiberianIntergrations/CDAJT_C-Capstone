// Mock @azure/msal-react
export const useMsal = jest.fn(() => ({
  instance: {
    initialize: jest.fn().mockResolvedValue(undefined),
    getAllAccounts: jest.fn(() => []),
    loginPopup: jest.fn().mockResolvedValue({
      account: {
        username: 'test@example.com',
        name: 'Test User',
      },
    }),
    logoutPopup: jest.fn().mockResolvedValue(undefined),
  },
  accounts: [],
  inProgress: 'none',
}));

export const MsalProvider = ({ children }) => children;

export const useMsalAuthentication = jest.fn(() => ({
  login: jest.fn(),
  result: null,
  error: null,
}));

export const useIsAuthenticated = jest.fn(() => false);
