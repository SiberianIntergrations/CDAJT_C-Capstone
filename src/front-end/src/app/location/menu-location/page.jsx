"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import LocationManagement from "@/components/location/LocationManagement";
import MenuManagement from "@/components/location/MenuManagement";

// export const metadata = {
//   title: "Location Management | Sushi Toshi",
//   description: "Manage restaurant locations",
//

const LocationPage = () => {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);

  // TODO: Fix role-based access control. Currently commented out for testing purposes.
  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("access_token");
      if (!token) {
        router.push("/auth/login");
        return;
      }

      try {
        // const tokenData = JSON.parse(atob(token.split(".")[1]));
        // if (tokenData.role !== "admin") {
        //   router.push("/unauthorized");
        //   return;
        // }
      } catch (error) {
        console.error("Error verifying token:", error);
        router.push("/auth/login");
        return;
      }

      setIsLoading(false);
    };

    checkAuth();
  }, [router]);

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
    <>
      <Box sx={{ flexGrow: 1, p: 0 }}>
        <LocationManagement />
        <MenuManagement />
      </Box>
    </>
  );
};

export default LocationPage;
