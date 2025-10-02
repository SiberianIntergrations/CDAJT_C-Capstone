// File: sushi-toshi-frontend/components/staff/SessionDashboard/hooks/useSessionActions.js
import { useState } from 'react';
import { axiosInstance, createApiUrl } from '../../../../config/api';

export const useSessionActions = (onSuccess) => {
  const [actionError, setActionError] = useState(null);

  const handleSuccess = async () => {
    if (onSuccess) {
      await onSuccess();
    }
  };

  const createSession = async (menuId) => {
    try {
      const response = await axiosInstance.post(createApiUrl('/dining-sessions'), {
        menu_id: parseInt(menuId)
      });
      if (!response.statusText === 'OK') {
        const errorData = response.data;
        throw new Error(errorData.detail || 'Failed to create session');
      }
      
      await handleSuccess();
      return true;
    } catch (err) {
      setActionError(err.message);
      return false;
    }
  };

  const addTable = async (sessionId, tableId) => {
    try {
      const response = await axiosInstance.post(createApiUrl(`/dining-sessions/${sessionId}/tables`), {
        table_id: parseInt(tableId)
      });

      if (!response.statusText === 'OK') {
        const errorData = response.data;
        throw new Error(errorData.detail || 'Failed to add table');
      }
      
      await handleSuccess();
      return true;
    } catch (err) {
      setActionError(err.message);
      return false;
    }
  };

  const createBill = async (sessionId, billData) => {
    try {
      const response = await axiosInstance.post(createApiUrl(`/bills/${sessionId}`), {
        bill_name: billData.billName,
        adult_count: parseInt(billData.adultCount),
        child_count: parseInt(billData.childCount),
        senior_count: parseInt(billData.seniorCount),
        tot_count: parseInt(billData.totCount)
      });

      if (!response.statusText === 'OK') {
        const errorData = response.data;
        throw new Error(errorData.detail || 'Failed to create bill');
      }
      
      await handleSuccess();
      return true;
    } catch (err) {
      setActionError(err.message);
      return false;
    }
  };

  const closeBill = async (sessionId, billId) => {
    try {
      const response = await axiosInstance.post(createApiUrl(`/bills/${billId}/close?session_id=${sessionId}`));
      if (!response.statusText === 'OK') {
        const errorData = response.data;
        throw new Error(errorData.detail || 'Failed to close bill');
      }
      
      await handleSuccess();
      return true;
    } catch (err) {
      setActionError(err.message);
      return false;
    }
  };

  const endSession = async (sessionId) => {
    try {
      const response = await axiosInstance.post(createApiUrl(`/dining-sessions/${sessionId}/end`));
      
      if (!response.statusText === 'OK') {
        const errorData = response.data;
        throw new Error(errorData.detail || 'Failed to end session');
      }
      
      await handleSuccess();
      return true;
    } catch (err) {
      setActionError(err.message);
      return false;
    }
  };

  const removeTable = async (sessionId, tableId) => {
    try {
      const response = await axiosInstance.delete(createApiUrl(`/dining-sessions/${sessionId}/tables/${tableId}`));
      if (!response.statusText === 'OK') {
        const errorData = response.data;
        throw new Error(errorData.detail || 'Failed to remove table');
      }
      
      await handleSuccess();
      return true;
    } catch (err) {
      setActionError(err.message);
      return false;
    }
  };

  return {
    createSession,
    addTable,
    removeTable,
    createBill,
    closeBill,
    endSession,
    actionError,
    clearActionError: () => setActionError(null)
  };
};