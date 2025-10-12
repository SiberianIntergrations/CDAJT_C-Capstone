import React, { useEffect } from "react";
import { Box, Typography } from "@mui/material";
import { Html5Qrcode } from "html5-qrcode";

const QRScanner = ({ onScanSuccess }) => {
  useEffect(() => {
    const qrCodeReader = new Html5Qrcode("reader");

    qrCodeReader
      .start(
        { facingMode: "environment" },
        { fps: 10, qrbox: 250 },
        (decodedText, decodedResult) => {
          console.log("Scanned result:", decodedText);
          onScanSuccess(decodedText); 
          qrCodeReader.stop();
        },
        (error) => {
          console.warn("Scanning failed:", error);
        }
      )
      .catch((err) => console.error("Failed to start QR code reader:", err));

    return () => {
      qrCodeReader.stop().catch((err) => console.warn("Failed to stop:", err));
    };
  }, [onScanSuccess]);

  return (
    <Box textAlign="center">
      <Typography variant="h5" marginBottom={2}>
        QR Code Scanner
      </Typography>
      <Box id="reader" sx={{ margin: "0 auto", maxWidth: 300 }} />
    </Box>
  );
};

export default QRScanner;
