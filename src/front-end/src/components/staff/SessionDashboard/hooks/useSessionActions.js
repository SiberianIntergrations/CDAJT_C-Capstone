import { useState } from "react";
import api from "@/config/api";

export const useSessionActions = (onSuccess) => {
  const [actionError, setActionError] = useState(null);

  const handleSuccess = async () => {
    if (onSuccess) {
      await onSuccess();
    }
  };

  const createSession = async (menuId) => {
    try {
      setActionError(null);
      // TODO: Update endpoint (api from old project: /dining-sessions POST)
      const response = await api.post("/DiningSession/Create_Dining_Session", {
          menu_id: parseInt(menuId),
      });
      if (response.status !== 200 && response.status !== 201) {
        throw new Error(response.data?.detail || "Failed to create session");
      }

      await handleSuccess();
      return true;
    } catch (err) {
      const errorMessage = err.response?.data?.detail || err.message;
      setActionError(errorMessage);
      console.error("Create session error:", errorMessage);
      return false;
    }
  };

  const addTable = async (session_id, tableId) => {
    try {
      setActionError(null);

      // api from old project: /dining-sessions/{sessionId}/tables POST
      const response = await api.post(`/DiningSession/${session_id}/tables`, {
        table_id: parseInt(tableId),
      });

      if (response.status !== 200 && response.status !== 201) {
        throw new Error(response.data?.detail || "Failed to add table");
      }

      await handleSuccess();
      return true;
    } catch (err) {
      const errorMessage = err.response?.data?.detail || err.message;
      setActionError(errorMessage);
      console.error("Add table error:", errorMessage);
      return false;
    }
  };

  const createBill = async (session_id, billData) => {
    try {
      setActionError(null);

      // api from old project: /bills/${sessionId} POST
      const response = await api.post(`/Bill/create_Bill/${session_id}`,
        {
          bill_name: billData.billName,
          adult_count: parseInt(billData.adultCount),
          child_count: parseInt(billData.childCount),
          senior_count: parseInt(billData.seniorCount),
          tot_count: parseInt(billData.totCount),
        }
      );

      if (response.status !== 200 && response.status !== 201) {
        throw new Error(response.data?.detail || "Failed to create bill");
      }

      await handleSuccess();
      return true;
    } catch (err) {
      const errorMessage = err.response?.data?.detail || err.message;
      setActionError(errorMessage);
      console.error("Create bill error:", errorMessage);
      return false;
    }
  };

  const closeBill = async (_bill_id) => {
    try {
      setActionError(null);
      // api from old project: /bills/{billId}/close?session_id=${sessionId} POST
      const response = await api.put(`/Bill/close_Bill/${_bill_id}`);
      if (response.status !== 200 && response.status !== 201) {
        throw new Error(response.data?.detail || "Failed to close bill");
      }

      await handleSuccess();
      return true;
    } catch (err) {
      const errorMessage = err.response?.data?.detail || err.message;
      setActionError(errorMessage);
      console.error("Close bill error:", errorMessage);
      return false;
    }
  };

  const endSession = async (sessionId) => {
    try {
      setActionError(null);

      // TODO: Update endpoint (api from old project: /dining-sessions/{sessionId}/end POST)
      const response = await api.post(`/dining-sessions/${sessionId}/end`);

      if (response.status !== 200 && response.status !== 201) {
        throw new Error(response.data?.detail || "Failed to end session");
      }

      await handleSuccess();
      return true;
    } catch (err) {
      const errorMessage = err.response?.data?.detail || err.message;
      setActionError(errorMessage);
      console.error("End session error:", errorMessage);
      return false;
    }
  };

  const removeTable = async (session_id, table_id) => {
    try {
      setActionError(null);

      // api from old project: /dining-sessions/{sessionId}/tables/{tableId} DELETE
      const response = await api.delete(`/DiningSession/${session_id}/Tables/${table_id}`);
      if (response.status !== 200 && response.status !== 204) {
        throw new Error(response.data?.detail || "Failed to remove table");
      }

      await handleSuccess();
      return true;
    } catch (err) {
      const errorMessage = err.response?.data?.detail || err.message;
      setActionError(errorMessage);
      console.error("Remove table error:", errorMessage);
      return false;
    }
  };

  const clearActionError = () => {
    setActionError(null);
  };

  return {
    createSession,
    addTable,
    removeTable,
    createBill,
    closeBill,
    endSession,
    actionError,
    clearActionError,
  };
};
