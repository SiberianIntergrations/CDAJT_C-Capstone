"use client";
import React, { useEffect } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import publicApi from "@/config/publicApi";
import { Container, Box, Typography, CircularProgress } from "@mui/material";

const Join = () => {
  const searchParams = useSearchParams();
  const router = useRouter();
  const tableNumber = searchParams.get("table");

  useEffect(() => {
    const joinAsGuest = async () => {

      const existingSession = localStorage.getItem("session_id");
      if (existingSession) {
        alert("You’re already seated at a table!");
        router.push("/");
        return;
      }
      if (!tableNumber) return;

      try {
        console.log("Joining table:", tableNumber);

        //Place holder - find the location based on what the admin set
        //Figure out current menu
        //Double check what the QR code is sending
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
        localStorage.setItem("guest", "true");
        localStorage.setItem("session_id", res.data.session_Id);

        router.push("/");
      } catch (err) {
        console.error("Error creating guest session:", err);
        alert("Unable to join this table.");
      }
    };

    joinAsGuest();
  }, [tableNumber, router]);

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
