"use client";
import React, { useEffect } from "react";
import { useSearchParams } from "next/navigation";
import publicApi from "@/config/publicApi";
import { Container, Box, Typography, CircularProgress } from "@mui/material";

const Join = () => {
  const searchParams = useSearchParams();
  const tableNumber = searchParams.get("table");

  useEffect(() => {
    const joinAsGuest = async () => {
      if (!tableNumber) return;

      try {
        console.log("Joining table:", tableNumber);

        const payload = {
          menu_Id: 1,
          location_Id: 1,
          table_Id: parseInt(tableNumber),
        };

        const res = await publicApi.post(
          `/DiningSession/Create_Dinning_Session?assignmentType=table`,
          payload
        );

        console.log("Session created:", res.data);

        //Store guest session
        localStorage.setItem("guest", "true");
        localStorage.setItem("session_id", res.data.session_Id);
        localStorage.setItem("guest_oid", res.data.guest_oid || `guest-${crypto.randomUUID()}`);

        //force full reload so HomePage re-runs and shows AppBar
        window.location.href = "/";
      } catch (err) {
        console.error("Error creating guest session:", err);
        alert("Unable to join this table.");
      }
    };

    joinAsGuest();
  }, [tableNumber]);

  return (
    <Container>
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="80vh"
        flexDirection="column"
      >
        <Typography variant="h5" align="center" marginBottom={2}>
          Joining Table {tableNumber || "..."}
        </Typography>
        <CircularProgress />
      </Box>
    </Container>
  );
};

export default Join;
