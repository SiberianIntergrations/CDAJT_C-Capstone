import dynamic from "next/dynamic";
import { Box, CircularProgress } from "@mui/material";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";

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

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("access_token");
      if (!token) {
        router.push("/auth/login");
        return;
      }

      try {
        const tokenData = JSON.parse(atob(token.split(".")[1]));
        if (!["admin"].includes(tokenData.role)) {
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
    <Box sx={{ width: "100%", minHeight: "100vh" }}>
      <TagManagement />
    </Box>
  );
};

export default TagManagementPage;
