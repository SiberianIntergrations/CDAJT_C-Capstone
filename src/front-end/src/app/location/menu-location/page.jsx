"use client";
import { useContext } from "react";
import Head from "next/head";
import { Box, CircularProgress } from "@mui/material";
import LocationManagement from "@/components/location/LocationManagement";
import MenuManagement from "@/components/location/MenuManagement";
import { AuthContext } from "@/app/layout";
import { useMsal } from "@azure/msal-react";

const LocationPage = () => {
  const { userRole, isLoggedIn, authLoading } = useContext(AuthContext);
  const msalInstance = useMsal().instance;

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
    console.warn("Unauthorized access attempt to Location Management page.");
    return null;
  }

  return (
    <>
      <Head>
        <title>Location Management | Sushi Toshi</title>
        <meta name="description" content="Manage restaurant locations" />
      </Head>
      <Box sx={{ flexGrow: 1, p: 0 }}>
        <LocationManagement msalInstance={msalInstance} />
        <MenuManagement />
      </Box>
    </>
  );
};

export default LocationPage;
