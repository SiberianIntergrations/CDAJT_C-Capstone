"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import dynamic from "next/dynamic";
import { Box, CircularProgress } from "@mui/material";

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
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("access_token");
    if (!token) {
      router.push("/auth/login");
      return;
    }

    // TODO: Fix role-based access control
    try {
      const tokenData = JSON.parse(atob(token.split(".")[1]));
      // if (tokenData.role !== "admin") {
      //   router.push("/unauthorized");
      // }
    } catch (error) {
      console.error("Error verifying token:", error);
      router.push("/auth/login");
    }
  }, [router]);

  return <StaffManagementPage />;
};

export default StaffPage;
