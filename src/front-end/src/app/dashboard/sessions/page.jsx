"use client";
import { useContext, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import { ErrorBoundary } from "react-error-boundary";
import SessionDashboard from "@/components/staff/SessionDashboard";
import styled from "@emotion/styled";
import { AuthContext } from "@/app/layout";

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
  const { userRole, isLoggedIn, authLoading } = useContext(AuthContext);

  useEffect(() => {
    if (!authLoading && !isLoggedIn) {
      router.push("/auth/login");
    }
  }, [authLoading, isLoggedIn, router]);

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

  if (!isLoggedIn || !["staff", "admin"].includes(userRole)) {
    console.warn("Unauthorized access attempt to Sessions page.");
    return null;
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
