"use client";
import React from "react";
import QRScanner from "@/components/QRScanner";
import { Container, Box, Typography } from "@mui/material";

const Join = () => {
  const handleScanSuccess = async (scannedData) => {
    console.log("Scanned data:", scannedData);

    // Send scanned data to the backend
    try {
      const response = await fetch("http://localhost:8000/process", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ scanned_text: scannedData }),
      });

      const result = await response.json();
      console.log("Server response:", result);
    } catch (error) {
      console.error("Error sending scanned data to the backend:", error);
    }
  };

  return (
    <Container>
      <Box marginTop={5}>
        <Typography variant="h4" align="center" marginBottom={3}>
          Join with QR Code
        </Typography>
        <QRScanner onScanSuccess={handleScanSuccess} />
      </Box>
    </Container>
  );
};

export default Join;
