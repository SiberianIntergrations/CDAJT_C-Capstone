import React, { createContext, useState, useContext, useCallback } from "react";

export const NotificationContext = createContext(null);

export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error("useNotification must be used within NotificationProvider");
  }

  const notifySuccess = useCallback(
    (message, duration = 6000) => {
      context.showNotification(message, "success", duration);
    },
    [context]
  );

  const notifyError = useCallback(
    (message, duration = 6000) => {
      context.showNotification(message, "error", duration);
    },
    [context]
  );

  const notifyWarning = useCallback(
    (message, duration = 6000) => {
      context.showNotification(message, "warning", duration);
    },
    [context]
  );

  const notifyInfo = useCallback(
    (message, duration = 6000) => {
      context.showNotification(message, "info", duration);
    },
    [context]
  );

  return {
    ...context,
    notifySuccess,
    notifyError,
    notifyWarning,
    notifyInfo,
  };
};

export const NotificationProvider = ({ children }) => {
  const [open, setOpen] = useState(false);
  const [message, setMessage] = useState("");
  const [severity, setSeverity] = useState("info");
  const [autoHideDuration, setAutoHideDuration] = useState(6000);

  const showNotification = (message, severity = "info", duration = 6000) => {
    setMessage(message);
    setSeverity(severity);
    setAutoHideDuration(duration);
    setOpen(true);
  };

  const hideNotification = () => {
    setOpen(false);
  };

  const value = {
    open,
    message,
    severity,
    autoHideDuration,
    showNotification,
    hideNotification,
  };

  return (
    <NotificationContext.Provider value={value}>
      {children}
    </NotificationContext.Provider>
  );
};
