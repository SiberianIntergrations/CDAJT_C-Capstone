"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import dynamic from "next/dynamic";
import { getAccessToken } from "@/utils/token";
import api from "@/config/api";
import { useAuth } from "@/hooks/useAuth";
// export const metadata = {
//   title: "Menu Item Management | Sushi Toshi",
//   description: "Manage menu items",
//

const MenuItemManagement = dynamic(
  () => import("@/components/admin/MenuItemManagement"),
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

export default function MenuItemsPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthorized, setIsAuthorized] = useState(false);
  const { isAuthenticated, userRole, loading } = useAuth();

  useEffect(() => {
    const checkAuth = () => {
      try {
        const token = getAccessToken();

        if (!token) {
          router.push("/auth/login");
          return;
        }

        if (loading) {
          return;
        }

        if (userRole !== "admin") {
          router.push("/unauthorized");
          return;
        }

        setIsAuthorized(true);
      } catch (error) {
        console.error("Error verifying token:", error);
        router.push("/auth/login");
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, [router, isAuthenticated, userRole, loading]);

  if (isLoading || !isAuthorized) {
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
      <MenuItemManagement />
    </Box>
  );
}
