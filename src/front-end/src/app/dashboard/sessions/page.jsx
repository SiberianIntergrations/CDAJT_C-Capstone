"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import { ErrorBoundary } from "react-error-boundary";
import SessionDashboard from "@/components/staff/SessionDashboard";
import styled from "@emotion/styled";
import { useAuth } from "@/hooks/useAuth";

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
    if (userRole !== "admin" && userRole !== "staff") {
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
    <FullPageContainer>
      <ErrorBoundary FallbackComponent={ErrorFallback}>
        <SessionDashboard />
      </ErrorBoundary>
    </FullPageContainer>
  );
};

export default SessionsPage;
