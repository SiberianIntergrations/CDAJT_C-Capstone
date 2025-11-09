import {
  createContext,
  useContext,
  useState,
  useCallback,
  useEffect,
} from "react";
import { useSessionData } from "../hooks/useSessionData";
import { useSessionActions } from "../hooks/useSessionActions";
import api from "@/config/api";

const SessionContext = createContext(null);

export const useSession = () => {
  const context = useContext(SessionContext);
  if (!context) {
    throw new Error("useSession must be used within SessionProvider");
  }
  return context;
};

export const SessionProvider = ({ children }) => {
  const [updateTrigger, setUpdateTrigger] = useState(0);
  const [dialogState, setDialogState] = useState({
    newSession: false,
    addTable: false,
    addTableGroup: false,
    newBill: false,
    currentSessionId: null,
  });

  const triggerUpdate = useCallback(() => {
    setUpdateTrigger((prev) => prev + 1);
  }, []);

  const {
    sessions,
    dashboardSummary,
    isLoading: dataLoading,
    error,
    fetchSessions,
    fetchDashboardSummary,
  } = useSessionData(updateTrigger);

  // useSessionActions now handles success callbacks internally
  const {
    // State
    actionError,
    isLoading: actionLoading,

    // Dining session operations
    createDiningSession,
    listDiningSessions,
    getDiningSessionDetail,
    closeDiningSession,
    addTableToSession,
    addTableGroupToSession,
    removeTableFromSession,
    removeTableGroupFromSession,
    getActiveSessionId,
    getSessionMenuId,

    // Session operations
    fetchActiveSessions,
    fetchSessionById,
    fetchSessionByTable,

    // Table operations
    listEmptyTables,

    // Table group operations
    fetchAvailableTableGroups,

    // Bill operations
    createBill: baseCreateBill,
    getBills,
    getBillSummary,
    closeBill: baseCloseBill,

    // Utilities
    clearActionError,
  } = useSessionActions(async () => {
    // This callback runs after successful actions
    await Promise.all([fetchSessions(), fetchDashboardSummary()]);
    triggerUpdate();
  });

  // Combined loading state
  const isLoading = dataLoading || actionLoading;

  // Wrap each action to trigger updates after completion
  const createSession = async (
    menuId,
    locationId,
    tableId,
    tableGroupId,
    tableAssignmentType
  ) => {
    try {
      if (tableAssignmentType === "table") {
        await api.post(
          `/DiningSession/Create_Dinning_Session?assignmentType=table`,
          {
            menu_Id: menuId,
            location_Id: locationId,
            table_Id: tableId,
          }
        );
      } else if (tableAssignmentType === "tableGroup") {
        await api.post(
          `/DiningSession/Create_Dinning_Session?assignmentType=table_group`,
          {
            menu_Id: menuId,
            location_Id: locationId,
            tableGroup_Id: tableGroupId,
          }
        );
      }
    } catch (error) {
      console.error("Error creating session:", error);
      return false;
    }
    triggerUpdate();
  };

  const addTable = async (sessionId, tableId) => {
    const result = await addTableToSession(sessionId, tableId);
    return result !== null && result !== undefined;
  };

  const addTableGroup = async (sessionId, tableGroupId) => {
    const result = await addTableGroupToSession(sessionId, tableGroupId);
    return result !== null && result !== undefined;
  };

  const removeTable = async (sessionId, tableId) => {
    const result = await removeTableFromSession(sessionId, tableId);
    return result !== null && result !== undefined;
  };

  const removeTableGroup = async (sessionId, tableGroupId) => {
    const result = await removeTableGroupFromSession(sessionId, tableGroupId);
    return result !== null && result !== undefined;
  };

  const createBill = async (sessionId, billData) => {
    const result = await baseCreateBill(sessionId, billData);
    return result !== null && result !== undefined;
  };

  const closeBill = async (sessionId, billId) => {
    const result = await baseCloseBill(sessionId, billId);
    return result !== null && result !== undefined;
  };

  const endSession = async (sessionId) => {
    const result = await closeDiningSession(sessionId);
    return result !== null && result !== undefined;
  };

  const openDialog = useCallback((dialogName, sessionId = null) => {
    console.log("Dialog session: ", sessionId);
    setDialogState((prev) => ({
      ...prev,
      [dialogName]: true,
      currentSessionId: sessionId,
    }));
  }, []);

  const closeDialog = useCallback(
    (dialogName) => {
      setDialogState((prev) => ({
        ...prev,
        [dialogName]: false,
        currentSessionId: null,
      }));
      clearActionError();
    },
    [clearActionError]
  );

  const value = {
    // Data state
    sessions,
    dashboardSummary,
    isLoading,
    error,
    actionError,

    // Dialog state
    dialogState,
    openDialog,
    closeDialog,

    // Data fetching
    fetchSessions,
    fetchDashboardSummary,

    // Dining session operations (backward compatible wrappers)
    createSession,
    addTable,
    addTableGroup,
    removeTable,
    removeTableGroup,
    endSession,

    // Direct access to all useSessionActions functions
    createDiningSession,
    listDiningSessions,
    getDiningSessionDetail,
    closeDiningSession,
    addTableToSession,
    addTableGroupToSession,
    removeTableFromSession,
    removeTableGroupFromSession,
    getActiveSessionId,
    getSessionMenuId,

    // Session operations
    fetchActiveSessions,
    fetchSessionById,
    fetchSessionByTable,

    // Table operations
    listEmptyTables,

    // Table group operations
    fetchAvailableTableGroups,

    // Bill operations
    createBill,
    getBills,
    getBillSummary,
    closeBill,

    // Utilities
    clearActionError,
    triggerUpdate,
  };

console.log("SessionProvider initialized – createBill:", typeof createBill);

  return (
    <SessionContext.Provider value={value}>{children}</SessionContext.Provider>
  );
};
