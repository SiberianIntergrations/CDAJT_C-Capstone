import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Head from "next/head";
import { Box, CircularProgress } from "@mui/material";
import dynamic from "next/dynamic";

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
        setIsLoading(false);
      } catch (error) {
        console.error("Error verifying token:", error);
        router.push("/auth/login");
      }
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
