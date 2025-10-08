"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Head from "next/head";
import { Box, CircularProgress } from "@mui/material";
import LocationManagement from "@/components/location/LocationManagement";
import MenuManagement from "@/components/location/MenuManagement";

const LocationPage = () => {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("access_token");
      if (!token) {
        router.push("/auth/login");
        return;
      }

      try {
        const tokenData = JSON.parse(atob(token.split(".")[1]));
        if (tokenData.role !== "admin") {
          router.push("/unauthorized");
          return;
        }
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
      <Head>
        <title>Location Management | Sushi Toshi</title>
        <meta name="description" content="Manage restaurant locations" />
      </Head>
      <Box sx={{ flexGrow: 1, p: 0 }}>
        <LocationManagement />
        <MenuManagement />
      </Box>
    </>
  );
};

export default LocationPage;
