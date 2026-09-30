"use client";

import { Suspense } from "react";
import { useEffect, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Box, CircularProgress, Typography, Alert, Paper } from "@mui/material";
import { QrCode } from "lucide-react";
import api from "@/config/api";

/**
 * Start Session Page
 *
 * This page is accessed by scanning a QR code at a restaurant table.
 * It automatically creates a dining session with the location and table number,
 * then redirects the customer to the menu.
 *
 * URL Format: /start-session?locationId=1&tableNumber=5
 */
function StartSessionContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [status, setStatus] = useState("loading"); // loading, error, success
  const [errorMessage, setErrorMessage] = useState("");
  const [sessionDetails, setSessionDetails] = useState(null);

  useEffect(() => {
    createDiningSession();
  }, []);

  const createDiningSession = async () => {
    try {
      // 1. Extract query parameters from QR code URL
      const locationId = searchParams.get("locationId");
      const tableNumber = searchParams.get("tableNumber");

      if (!locationId || !tableNumber) {
        setStatus("error");
        setErrorMessage(
          "Invalid QR code. Missing location or table information.",
        );
        return;
      }

      console.log(
        `Starting session for Location ${locationId}, Table ${tableNumber}`,
      );

      // 2. Look up the table_id from table_number and location_id
      const tableResponse = await api.get("/TableEntity");
      console.log(`Table Response: ${tableResponse.data}`);
      const allTables = tableResponse.data;

      const table = allTables.find(
        (t) =>
          t.location_Id === parseInt(locationId) &&
          t.table_number === parseInt(tableNumber) &&
          t.is_active === true,
      );

      if (!table) {
        setStatus("error");
        setErrorMessage(
          `Table ${tableNumber} not found or inactive at this location.`,
        );
        return;
      }

      // 3. Get the default menu for this location
      // You may need to adjust this based on your menu endpoint
      const menuResponse = await api.get("/Menu");
      const menus = menuResponse.data;

      // Try to find a menu for this location (adjust logic as needed)
      const locationMenu = menus.find(
        (m) => m.location_Id === parseInt(locationId),
      );
      const defaultMenu = locationMenu || menus[0]; // Fallback to first menu

      if (!defaultMenu) {
        setStatus("error");
        setErrorMessage("No menu available for this location.");
        return;
      }

      // 4. Create the dining session
      // const sessionData = {
      //   menu_id: defaultMenu.menu_id,
      //   location_id: parseInt(locationId),
      //   table_id: table.table_Id,
      //   tablegroup_id: null
      // };
      const sessionData = {
        menu_id: defaultMenu.menu_id,
        locationId: parseInt(locationId),
        tableId: table.table_Id,
        tablegroup_id: null,
      };
      console.log("Creating session with data:", sessionData);

      // const sessionResponse = await api.post(
      //   '/DiningSession/Create_Dinning_Session',
      //   sessionData
      // );
      const sessionResponse = await api.post(
        "/DiningSession/addguestparticipant/v2",
        sessionData,
      );

      if (sessionResponse.status === 200) {
        const sessionId = sessionResponse.data.session_Id;

        setSessionDetails({
          sessionId,
          locationId,
          tableNumber,
          menuId: defaultMenu.menu_id,
        });

        setStatus("success");

        // 5. Redirect to menu after 1.5 seconds
        setTimeout(() => {
          router.push(`/menu/full-menu?sessionId=${sessionId}`);
        }, 1500);
      } else {
        throw new Error("Failed to create dining session");
      }
    } catch (error) {
      console.error("Error creating dining session:", error);
      setStatus("error");
      setErrorMessage(
        error.response?.data?.message ||
          error.message ||
          "Failed to start your dining session. Please try scanning the QR code again or ask a staff member for assistance.",
      );
    }
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        bgcolor: "background.default",
        p: 3,
      }}
    >
      <Paper
        elevation={3}
        sx={{
          p: 4,
          maxWidth: 500,
          width: "100%",
          textAlign: "center",
        }}
      >
        <Box sx={{ mb: 3 }}>
          <QrCode size={64} style={{ margin: "0 auto", color: "#1976d2" }} />
        </Box>

        {status === "loading" && (
          <>
            <CircularProgress size={60} sx={{ mb: 3 }} />
            <Typography variant="h5" gutterBottom>
              Starting Your Dining Session
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Please wait while we set up your table...
            </Typography>
          </>
        )}

        {status === "success" && (
          <>
            <Box
              sx={{
                width: 60,
                height: 60,
                borderRadius: "50%",
                bgcolor: "success.main",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                margin: "0 auto 24px",
                fontSize: "32px",
              }}
            >
              ✓
            </Box>
            <Typography variant="h5" gutterBottom color="success.main">
              Session Started!
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
              Location {sessionDetails?.locationId} • Table{" "}
              {sessionDetails?.tableNumber}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Redirecting to menu...
            </Typography>
          </>
        )}

        {status === "error" && (
          <>
            <Alert severity="error" sx={{ mb: 3, textAlign: "left" }}>
              {errorMessage}
            </Alert>
            <Typography variant="body2" color="text.secondary">
              Please try scanning the QR code again, or ask a staff member for
              assistance.
            </Typography>
          </>
        )}
      </Paper>
    </Box>
  );
}

export default function StartSessionPage() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <StartSessionContent />
    </Suspense>
  );
}
