import React from "react";
import { Snackbar, Alert, Slide } from "@mui/material";
import { useNotification } from "@/contexts/NotificationContext";

const NotificationSystem = () => {
  const { open, message, severity, autoHideDuration, hideNotification } =
    useNotification();

  function SlideTransition(props) {
    return <Slide {...props} direction="up" />;
  }

  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }
    hideNotification();
  };

  return (
    <Snackbar
      open={open}
      autoHideDuration={autoHideDuration}
      onClose={handleClose}
      anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
      slots={{ transition: SlideTransition }}
      sx={{
        bottom: { xs: 70, sm: 24 },
      }}
    >
      <Alert onClose={handleClose} severity={severity} sx={{ width: "100%" }} >
        {message}
      </Alert>
    </Snackbar>
  );
};

export default NotificationSystem;
