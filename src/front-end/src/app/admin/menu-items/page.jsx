"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import dynamic from "next/dynamic";

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

  useEffect(() => {
    const checkAuth = () => {
      try {
        const token = localStorage.getItem("access_token");
        
        if (!token) {
          router.push("/auth/login");
          return;
        }

        const tokenData = JSON.parse(atob(token.split(".")[1]));
        
        // if (tokenData.role !== "admin") {
        //   router.push("/unauthorized");
        //   return;
        // }
        
        setIsAuthorized(true);
      } catch (error) {
        console.error("Error verifying token:", error);
        router.push("/auth/login");
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, [router]);

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