"use client";

import React, { useState, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  Alert,
  CircularProgress,
} from "@mui/material";
import { KeyRound } from "lucide-react";
import api from "@/config/api";

const ResetPasswordFormContent = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const token = searchParams.get("token");

  const [formData, setFormData] = useState({
    newPassword: "",
    confirmPassword: "",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    if (formData.newPassword !== formData.confirmPassword) {
      setError("Passwords do not match");
      setLoading(false);
      return;
    }

    if (!token) {
      setError("Reset token is missing");
      setLoading(false);
      return;
    }

    try {
      await api.post("/auth/reset-password", {
        token,
        new_password: formData.newPassword,
      });

      router.push("/auth/login?reset=success");
    } catch (err) {
      setError(
        err.response?.data?.detail ||
          "Failed to reset password. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        backgroundColor: "gray.100",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 1,
      }}
    >
      <Paper
        sx={{
          padding: 4,
          width: "100%",
          maxWidth: 400,
          backgroundColor: "white",
          borderRadius: 2,
        }}
      >
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: 3,
          }}
        >
          <KeyRound
            className="w-12 h-12 text-primary"
            style={{ fontSize: 48 }}
          />

          <Typography
            component="h1"
            sx={{
              fontSize: "1.25rem",
              fontWeight: "bold",
              textAlign: "center",
            }}
          >
            Set New Password
          </Typography>

          {error && (
            <Alert severity="error" sx={{ width: "100%", marginBottom: 2 }}>
              {error}
            </Alert>
          )}

          <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{
              width: "100%",
              display: "flex",
              flexDirection: "column",
              gap: 2,
            }}
          >
            <TextField
              label="New Password"
              name="newPassword"
              type="password"
              value={formData.newPassword}
              onChange={handleChange}
              required
              fullWidth
              autoComplete="new-password"
              sx={{ marginBottom: 3 }}
            />

            <TextField
              label="Confirm Password"
              name="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              fullWidth
              autoComplete="new-password"
              sx={{ marginBottom: 4 }}
            />

            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={loading}
              sx={{
                paddingY: 2,
                marginBottom: 3,
                backgroundColor: "red.600",
                "&:hover": {
                  backgroundColor: "red.700",
                },
                fontSize: "1rem",
                height: 56,
                textTransform: "none",
              }}
            >
              {loading ? (
                <CircularProgress size={24} sx={{ color: "white" }} />
              ) : (
                "Reset Password"
              )}
            </Button>

            <Box sx={{ textAlign: "center", marginTop: 2 }}>
              <Link href="/auth/login" passHref>
                <Button
                  sx={{
                    textTransform: "none",
                    fontSize: "0.875rem",
                    padding: "0.5rem 1rem",
                    color: "blue.600",
                    "&:hover": { color: "blue.800" },
                  }}
                >
                  Back to Login
                </Button>
              </Link>
            </Box>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
};

const ResetPasswordForm = () => {
  return (
    <Suspense
      fallback={
        <Box
          sx={{
            minHeight: "100vh",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <CircularProgress />
        </Box>
      }
    >
      <ResetPasswordFormContent />
    </Suspense>
  );
};

export default ResetPasswordForm;
