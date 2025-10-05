import React from "react";
import { Box, Toolbar } from "@mui/material";
import AppBarWithTitle from "./AppBarWithTitle";

const Layout = ({ children }) => {
  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        minHeight: "100vh",
      }}
    >
      <AppBarWithTitle />
      <Toolbar />

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          display: "flex",
          flexDirection: "column",
        }}
      >
        {children}
      </Box>

      <Box
        component="footer"
        sx={{
          py: 2,
          px: 3,
          mt: "auto",
          backgroundColor: "#14171a",
          color: "#657786",
          textAlign: "center",
        }}
      >
        <h3>Copyright 2025</h3>
      </Box>
    </Box>
  );
};

export default Layout;
