"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import LocationManagement from "@/components/location/LocationManagement";
import MenuManagement from "@/components/location/MenuManagement";
import { useAuth } from "@/hooks/useAuth";

const LocationPage = () => {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthorized, setIsAuthorized] = useState(false);
  const { isAuthenticated, userRole, loading } = useAuth();

  useEffect(() => {
    if (loading) {
      setIsLoading(true);
      return;
    }
    if (!isAuthenticated) {
      router.push("/auth/login");
      return;
    }
    if (userRole !== "admin") {
      router.push("/unauthorized");
      return;
    }
    setIsAuthorized(true);
    setIsLoading(false);
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
    <>
      <Box sx={{ flexGrow: 1, p: 0 }}>
        <LocationManagement />
        <MenuManagement />
      </Box>
    </>
  );
};

export default LocationPage;
