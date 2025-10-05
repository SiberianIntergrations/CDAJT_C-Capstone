// // File: sushi-toshi-frontend/components/staff/SessionDashboard/context/SessionContext.jsx
// import { createContext, useContext, useState, useCallback } from 'react';
// import { useSessionData } from '../hooks/useSessionData';
// import { useSessionActions } from '../hooks/useSessionActions';

// const SessionContext = createContext(null);

// export const useSession = () => {
//   const context = useContext(SessionContext);
//   if (!context) {
//     throw new Error('useSession must be used within SessionProvider');
//   }
//   return context;
// };

// export const SessionProvider = ({ children }) => {
//   const [updateCounter, setUpdateCounter] = useState(0);
//   const [dialogState, setDialogState] = useState({
//     newSession: false,
//     addTable: false,
//     newBill: false,
//     currentSessionId: null
//   });

//   const {
//     sessions,
//     dashboardSummary,
//     isLoading,
//     error,
//     fetchSessions
//   } = useSessionData(updateCounter);

//   const {
//     createSession,
//     addTable,
//     createBill,
//     closeBill,
//     endSession,
//     actionError,
//     clearActionError
//   } = useSessionActions(() => {
//     fetchSessions();
//   });

//   const openDialog = useCallback((dialogName, sessionId = null) => {
//     setDialogState(prev => ({
//       ...prev,
//       [dialogName]: true,
//       currentSessionId: sessionId
//     }));
//   }, []);

//   const closeDialog = useCallback((dialogName) => {
//     setDialogState(prev => ({
//       ...prev,
//       [dialogName]: false,
//       currentSessionId: null
//     }));
//   }, []);

//   const value = {
//     sessions,
//     dashboardSummary,
//     isLoading,
//     error,
//     dialogState,
//     openDialog,
//     closeDialog,
//     fetchSessions,
//     createSession,
//     addTable,
//     createBill,
//     closeBill,
//     endSession,
//     actionError,
//     clearActionError
//   };

//   return (
//     <SessionContext.Provider value={value}>
//       {children}
//     </SessionContext.Provider>
//   );
// };

import { createContext, useContext, useState, useCallback, useEffect } from 'react';
import { useSessionData } from '../hooks/useSessionData';
import { useSessionActions } from '../hooks/useSessionActions';


const SessionContext = createContext(null);

export const useSession = () => {
  const context = useContext(SessionContext);
  if (!context) {
    throw new Error('useSession must be used within SessionProvider');
  }
  return context;
};

export const SessionProvider = ({ children }) => {
  const [updateTrigger, setUpdateTrigger] = useState(0);
  const [actionError, setActionError] = useState(null);
  const [dialogState, setDialogState] = useState({
    newSession: false,
    addTable: false,
    newBill: false,
    currentSessionId: null
  });

  const {
    sessions,
    dashboardSummary,
    isLoading,
    error,
    fetchSessions,
    fetchDashboardSummary
  } = useSessionData(updateTrigger);

  const triggerUpdate = useCallback(() => {
    setUpdateTrigger(prev => prev + 1);
  }, []);

  const {
    createSession: baseCreateSession,
    addTable: baseAddTable,
    createBill: baseCreateBill,
    closeBill: baseCloseBill,
    endSession: baseEndSession,
    actionError: hookActionError,
    clearActionError
  } = useSessionActions();
  
  useEffect(() => {
    if (hookActionError) {
      console.log('Action error in context:', hookActionError);
    }
  }, [hookActionError]);

  // Wrap each action to trigger updates after completion
  const createSession = async (menuId) => {
    const success = await baseCreateSession(menuId);
    if (success) {
      await Promise.all([fetchSessions(), fetchDashboardSummary()]);
      triggerUpdate();
    }
    return success;
  };

  const addTable = async (sessionId, tableId) => {
    const success = await baseAddTable(sessionId, tableId);
    if (success) {
      await Promise.all([fetchSessions(), fetchDashboardSummary()]);
      triggerUpdate();
    }
    return success;
  };

  const createBill = async (sessionId, billData) => {
    const success = await baseCreateBill(sessionId, billData);
    if (success) {
      await Promise.all([fetchSessions(), fetchDashboardSummary()]);
      triggerUpdate();
    }
    return success;
  };

  const closeBill = async (sessionId, billId) => {
    const success = await baseCloseBill(sessionId, billId);
    if (success) {
      await Promise.all([fetchSessions(), fetchDashboardSummary()]);
      triggerUpdate();
    }
    return success;
  };

  const endSession = async (sessionId) => {
    const success = await baseEndSession(sessionId);
    if (success) {
      await Promise.all([fetchSessions(), fetchDashboardSummary()]);
      triggerUpdate();
    }
    return success;
  };

  const openDialog = useCallback((dialogName, sessionId = null) => {
    setDialogState(prev => ({
      ...prev,
      [dialogName]: true,
      currentSessionId: sessionId
    }));
  }, []);

  const closeDialog = useCallback((dialogName) => {
    setDialogState(prev => ({
      ...prev,
      [dialogName]: false,
      currentSessionId: null
    }));
  }, []);

  const value = {
    sessions,
    dashboardSummary,
    isLoading,
    error,
    dialogState,
    openDialog,
    closeDialog,
    fetchSessions,
    fetchDashboardSummary,
    createSession,
    addTable,
    createBill,
    closeBill,
    endSession,
    actionError: hookActionError,
    clearActionError,
    triggerUpdate
  };

  return (
    <SessionContext.Provider value={value}>
      {children}
    </SessionContext.Provider>
  );
};