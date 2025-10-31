"use client";
import dynamic from "next/dynamic";
import { Box, CircularProgress } from "@mui/material";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/hooks/useAuth";

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
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);
  const { isAuthenticated, userRole, loading } = useAuth();

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("access_token");

      if (!token) {
        router.push("/auth/login");
        return;
      }

      // IMPORTANT: Wait for useAuth to finish loading
      if (loading) {
        return;
      }

      try {
        // Now userRole should be loaded
        if (!userRole || userRole.toLowerCase().trim() !== "admin") {
          router.push("/unauthorized");
          return;
        }
        setIsLoading(false);
      } catch (error) {
        console.error("Error verifying token:", error);
        router.push("/auth/login");
      }
    };

    checkAuth();
  }, [router, isAuthenticated, userRole, loading]);

  if (isLoading) {
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

  return (
    <Box sx={{ width: "100%", minHeight: "100vh" }}>
      <TagManagement />
    </Box>
  );
};

export default TagManagementPage;
