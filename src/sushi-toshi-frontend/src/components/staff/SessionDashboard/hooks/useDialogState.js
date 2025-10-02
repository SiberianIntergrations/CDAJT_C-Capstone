// File: sushi-toshi-frontend/components/staff/SessionDashboard/hooks/useDialogState.js
import { useState } from 'react';

export const useDialogState = () => {
  const [dialogState, setDialogState] = useState({
    newSession: false,
    addTable: false,
    newBill: false,
    currentSessionId: null
  });

  const openDialog = (dialogName, sessionId = null) => {
    setDialogState(prev => ({
      ...prev,
      [dialogName]: true,
      currentSessionId: sessionId
    }));
  };

  const closeDialog = (dialogName) => {
    setDialogState(prev => ({
      ...prev,
      [dialogName]: false,
      currentSessionId: null
    }));
  };

  return {
    dialogState,
    openDialog,
    closeDialog
  };
};
