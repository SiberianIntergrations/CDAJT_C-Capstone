"use client";
import { useContext } from "react";
import dynamic from "next/dynamic";
import { Box, CircularProgress } from "@mui/material";
import { AuthContext } from "@/app/layout";

const StaffManagementPage = dynamic(
  () => import("@/components/admin/StaffManagementPage"),
  {
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
  }
);

const StaffPage = () => {
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
    console.warn("Unauthorized access attempt to Staff Management page.");
    return null;
  }

  return <StaffManagementPage />;
};

export default StaffPage;
