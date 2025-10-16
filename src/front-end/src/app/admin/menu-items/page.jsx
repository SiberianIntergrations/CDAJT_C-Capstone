"use client";
import { useContext, useEffect, useState } from "react";
import Head from "next/head";
import { Box, CircularProgress } from "@mui/material";
import dynamic from "next/dynamic";
import { AuthContext } from "@/app/layout";

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

const MenuItemsPage = () => {
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
    console.warn("Unauthorized access attempt to Menu Items Management page.");
    return null;
  }

  return (
    <>
      <Head>
        <title>Menu Item Management | Sushi Toshi</title>
        <meta name="description" content="Manage menu items" />
      </Head>
      <Box sx={{ width: "100%", minHeight: "100vh" }}>
        <MenuItemManagement />
      </Box>
    </>
  );
};

export default MenuItemsPage;
