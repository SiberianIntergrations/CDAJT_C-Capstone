"use client";
import dynamic from "next/dynamic";
import { Box, CircularProgress } from "@mui/material";
import { useContext } from "react";
import { AuthContext } from "@/app/layout";

const TagManagement = dynamic(() => import("@/components/tags/TagManagement"), {
  loading: () => (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      minHeight="100vh"
    >
      <CircularProgress />
    </Box>
  ),
  ssr: false,
});

const TagManagementPage = () => {
  const { userRole, isLoggedIn, authLoading } = useContext(AuthContext);

  if (authLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (!isLoggedIn || userRole !== "admin") {
    console.warn("Unauthorized access attempt to Tag Management page.");
    return null;
  }

  return (
    <Box sx={{ width: "100%", minHeight: "100vh" }}>
      <TagManagement />
    </Box>
  );
};

export default TagManagementPage;
