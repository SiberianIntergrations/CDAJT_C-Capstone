"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import { ErrorBoundary } from "react-error-boundary";
import SessionDashboard from "@/components/staff/SessionDashboard";
import styled from "@emotion/styled";
import { getUserRole } from "@/utils/token";

// export const metadata = {
//   title: "Session Management | Sushi Toshi",
//   description: "Manage dining sessions and tables",
//   name: "viewport",
//   content: "width=device-width, initial-scale=1",
// };

const ErrorMessage = styled.div`
  padding: 16px;
  color: red;
`;

const FullPageContainer = styled.div`
  position: fixed;
  top: 64px;
  left: 0;
  right: 0;
  bottom: 0;
  overflow-y: auto;
  background-color: #f5f5f5;
`;

function ErrorFallback({ error }) {
  return (
    <ErrorMessage>
      <h3>Something went wrong:</h3>
      <pre>{error.message}</pre>
    </ErrorMessage>
  );
}

const SessionsPage = () => {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("access_token");
      if (!token) {
        router.push("/auth/login");
        return false;
      }

      try {
        const role = getUserRole(token);

        if (role !== "admin" && role !== "staff") {
          router.push("/unauthorized");
          return false;
        }
        return true;
      } catch (error) {
        console.error("Error verifying token:", error);
        router.push("/auth/login");
        return false;
      }
    };

    if (checkAuth()) {
      setIsLoading(false);
    }
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
    <FullPageContainer>
      <ErrorBoundary FallbackComponent={ErrorFallback}>
        <SessionDashboard />
      </ErrorBoundary>
    </FullPageContainer>
  );
};

export default SessionsPage;
