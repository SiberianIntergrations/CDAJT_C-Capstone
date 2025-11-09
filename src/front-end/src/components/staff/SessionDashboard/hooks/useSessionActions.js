import { useState } from "react";
import api from "@/config/api";
import { createApiWrapper } from "@/utils/apiWrapper";

/**
 * Safely converts value to integer with fallback.
 * @param {*} v - Value to convert
 * @param {number} d - Default value if conversion fails (default: 0)
 * @returns {number}
 */
const toInt = (v, d = 0) => (Number.isFinite(+v) ? parseInt(v, 10) : d);

// Validates that required IDs are present
const validateIds = (ids) => {
  for (const [name, value] of Object.entries(ids)) {
    if (!value) throw new Error(`Valid ${name} is required`);
  }
};

export const useSessionActions = (onSuccess) => {
  const [actionError, setActionError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleError = (err, context) => {
    console.log(err);

    // Ensure msg is always a string
    let msg;

    if (typeof err.response?.data === "string") {
      msg = err.response.data;
    } else if (err.response?.data?.message) {
      msg = err.response.data.message;
    } else if (err.response?.data?.detail) {
      msg = err.response.data.detail;
    } else if (typeof err.response?.message === "string") {
      msg = err.response.message;
    } else if (typeof err.message === "string") {
      msg = err.message;
    } else {
      msg = `Unknown error in ${context}`;
    }

    console.error(`[useSessionActions] ${context}:`, err);
    setActionError(msg);
    return msg;
  };

  // Wrapper that handles all the repetitive loading/error logic
  const apiWrapper = createApiWrapper({
    setIsLoading,
    setActionError,
    handleError,
    onSuccess,
  });

  // DINING SESSION OPERATION

  const createSession = (sessionData) =>
    apiWrapper(
      "createSession",
      async () => {
        const payload = {
          Menu_Id: toInt(sessionData?.Menu_Id),
          Location_Id: toInt(sessionData?.Location_Id),
          Table_Id: sessionData?.Table_Id ? toInt(sessionData?.Table_Id) : null,
          TableGroup_Id: sessionData?.TableGroup_Id
            ? toInt(sessionData?.TableGroup_Id)
            : null,
        };

        if (!payload.Menu_Id) throw new Error("Menu ID is required");
        if (!payload.Location_Id) throw new Error("Location ID is required");

        console.log("Creating session with payload:", payload);

        const response = await api.post(
          "/DiningSession/Create_Dinning_Session",
          payload
        );

        if (response.status !== 200 && response.status !== 201) {
          throw new Error(response.data?.detail || "Failed to create session");
        }

        await handleSuccess();
        return response.data;
      },
      { triggerSuccess: true }
    );

  /**
   * List of dining sessions filterd by active status
   */
  const listDiningSessions = (activeOnly = true) =>
    apiWrapper(
      "listDiningSessions",
      async () => {
        const res = await api.get("/DiningSession/get_list_dining_sessions", {
          params: { ActiveOnly: activeOnly },
        });
        return res.data || [];
      },
      { defaultReturn: [] }
    );

  /**
   * Get detailed information about a specific session
   */
  const getDiningSessionDetail = (sessionId) =>
    apiWrapper("getDiningSessionDetail", async () => {
      const sid = toInt(sessionId);
      validateIds({ "session ID": sid });

      const res = await api.get(`/DiningSession/get_location/${sid}`);
      return res.data;
    });

  /**
   * Closes dining session after all bills are paid
   */
  const closeSession = async (sessionId) =>
    apiWrapper(
      "closeSession",
      async () => {
        const sid = toInt(sessionId);
        validateIds({ "session ID": sid });

        const res = await api.put(`/DiningSession/${sid}/close`);
        return res.data;
      },
      { triggerSuccess: true }
    );

  /**
   * Add table to session
   */
  const addTable = (sessionId, tableId) =>
    apiWrapper(
      "addTable",
      async () => {
        const sid = toInt(sessionId);
        const tid = toInt(tableId);
        validateIds({ "session ID": sid, "table ID": tid });

        const res = await api.post(`/DiningSession/${sid}/tables`, {
          table_Id: tid,
        });
        return res.data;
      },
      { triggerSuccess: true }
    );

  /**
   * Add table group to a session
   */
  const addTableGroupToSession = (sessionId, tableGroupId) =>
    apiWrapper(
      "addTableGroupToSession",
      async () => {
        const sid = toInt(sessionId);
        const tgid = toInt(tableGroupId);
        validateIds({ "session ID": sid, "table group ID": tgid });

        const res = await api.post(`/DiningSession/${sid}/table-groups`, {
          tableGroup_Id: tgid,
        });
        return res.data;
      },
      { triggerSuccess: true }
    );

  /**
   * Remove table from a session
   */
  const removeTableFromSession = (sessionId, tableId) =>
    apiWrapper(
      "removeTableFromSession",
      async () => {
        const sid = toInt(sessionId);
        const tid = toInt(tableId);
        validateIds({ "session ID": sid, "table ID": tid });

        const res = await api.delete(`/DiningSession/${sid}/Tables/${tid}`);
        return res.data;
      },
      { triggerSuccess: true }
    );

  /**
   * Remove table group from a session
   */
  const removeTableGroupFromSession = (sessionId, tableGroupId) =>
    apiWrapper(
      "removeTableGroupFromSession",
      async () => {
        const sid = toInt(sessionId);
        const tgid = toInt(tableGroupId);
        validateIds({ "session ID": sid, "table group ID": tgid });

        const res = await api.delete(
          `/DiningSession/${sid}/table-groups/${tgid}`
        );
        return res.data;
      },
      { triggerSuccess: true }
    );

  /**
   * Get active session ID for current user
   */
  const getActiveSessionId = () =>
    apiWrapper("getActiveSessionId", async () => {
      const res = await api.get(
        "/DiningSession/participants/active-session-id"
      );
      return res.data?.Session_Id || res.data?.session_id || null;
    });

  /**
   * Get menu ID for a session
   */
  const getSessionMenuId = (sessionId) =>
    apiWrapper("getSessionMenuId", async () => {
      const sid = toInt(sessionId);
      validateIds({ "session ID": sid });

      const res = await api.get(`/DiningSession/session-menu/${sid}`);
      return res.data?.Menu_Id || res.data?.menu_id || null;
    });

  // SESSION OPERATIONS

  /**
   * Fetches all active sessions
   */
  const fetchActiveSessions = () =>
    apiWrapper(
      "fetchActiveSessions",
      async () => {
        const res = await api.get("/session/active");
        return res.data || [];
      },
      { defaultReturn: [] }
    );

  /**
   * Fetch a session by ID
   */
  const fetchSessionById = (sessionId) =>
    apiWrapper("fetchSessionById", async () => {
      const sid = toInt(sessionId);
      validateIds({ "session ID": sid });

      const res = await api.get(`/session/${sid}`);
      return res.data;
    });

  /**
   * Get session for a table
   */

  const fetchSessionByTable = (tableId) =>
    apiWrapper("fetchSessionByTable", async () => {
      const tid = toInt(tableId);
      validateIds({ "table ID": tid });

      const res = await api.get(`/session/table/${tid}`);
      return res.data;
    });

  // TABLE OPERATIONS

  /**
   * List empty tables
   */
  const listEmptyTables = (locationId) =>
    apiWrapper(
      "listEmptyTables",
      async () => {
        const params = {};
        if (locationId) params.locationId = toInt(locationId);

        const res = await api.get("/TableEntity/empty", { params });
        return res.data || [];
      },
      { defaultReturn: [] }
    );

  // TABLE GROUP OPERATIONS

  /**
   * Fetches available table groups
   */
  const fetchAvailableTableGroups = (locationId) =>
    apiWrapper(
      "fetchAvailableTableGroups",
      async () => {
        const params = {};
        if (locationId) params.locationId = toInt(locationId);

        const res = await api.get("/TableGroup/available", { params });
        return res.data || [];
      },
      { defaultReturn: [] }
    );

  // BILL OPERATIONS

  /**
   * Create a bill for a session
   */
  const createBill = (sessionId, billData) =>
    apiWrapper(
      "createBill",
      async () => {
        const sid = toInt(sessionId);
        if (!sid) throw new Error("Session ID is required");

      const payload = {
        bill_name: billData.billName,
        adult_count: parseInt(billData.adultCount) || 0,
        child_count: parseInt(billData.childCount) || 0,
        senior_count: parseInt(billData.seniorCount) || 0,
        tot_count: parseInt(billData.totCount) || 0,
      };

        if (!payload.bill_name) throw new Error("Bill name is required");

        const res = await api.post(`/Bill/create_Bill/${sid}`, payload);
        return res.data;
      },
      { triggerSuccess: true }
    );

  /**
   * Get all bills for a session
   */
  const getBills = (sessionId, tableId) =>
    apiWrapper(
      "getBills",
      async () => {
        const sid = toInt(sessionId);
        if (!sid) throw new Error("Session ID is required");

        const params = {};
        if (tableId) params._table_id = toInt(tableId);

        const res = await api.get(`/Bill/get_bills/${sid}`, { params });
        return res.data || [];
      },
      { defaultReturn: [] }
    );
  
  /**
   * Get detailed bill summary with pricing breakdown
  */
  const getBillSummary = (sessionId, billId) =>
    apiWrapper("getBillSummary", async () => {
      const sid = toInt(sessionId);
      const bid = toInt(billId);
      validateIds({ "session ID": sid, "bill ID": bid });

      const res = await api.get(`/Bill/summary/${bid}`, {
        params: { _session_id: sid },
      });
      return res.data;
    });

  /**
   * Closes a bill
   */
  const closeBill = (sessionId, billId) =>
    apiWrapper(
      "closeBill",
      async () => {
        const sid = toInt(sessionId);
        const bid = toInt(billId);
        validateIds({ "session ID": sid, "bill ID": bid });

        const res = await api.put(`/Bill/close_bill/${bid}`, null, {
          params: { _session_id: sid },
        });
        return res.data;
      },
      { triggerSuccess: true }
    );

  const clearActionError = () => setActionError(null);

  return {
    // State
    actionError,
    isLoading,

    // Dining session operations
    createDiningSession: createSession,
    listDiningSessions,
    getDiningSessionDetail,
    closeDiningSession: closeSession,
    addTableToSession: addTable,
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
  };
};
