"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
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
import { UserPlus } from "lucide-react";
import { registerUser } from "@/utils/auth";

const RegisterForm = () => {
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: "jamie2@example.com",
    password: "somethingCool1",
    confirmPassword: "somethingCool1",
    firstName: "James",
    lastName: "Smith",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

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

    // Validate password match
    if (formData.password !== formData.confirmPassword) {
      setError("Passwords do not match");
      setLoading(false);
      return;
    }

    try {
      await registerUser({
        email: formData.email,
        password: formData.password,
        first_name: formData.firstName,
        last_name: formData.lastName,
      });

      setIsSuccess(true);

      // Reset form fields
      setFormData({
        email: "",
        password: "",
        confirmPassword: "",
        firstName: "",
        lastName: "",
      });
    } catch (err) {
      console.error("Registration error:", err);

      if (err.response) {
        const status = err.response.status;
        const detail = err.response.data?.detail;

        if (status === 400) {
          setError(detail || "Password or Email invalid");
        } else if (status === 422) {
          const errorMessage =
            detail || "Invalid input data";
          setError(errorMessage);
        } else if (status === 409) {
          setError("Email already registered");
        } else {
          setError("Registration failed. Please try again.");
        }
      } else if (err.request) {
        setError("No response from server. Please check your connection.");
      } else {
        setError("Failed to send registration request.");
      }
      console.error("Registration error:", err);
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
          padding: 1,
          width: "100%",
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
          <UserPlus sx={{ fontSize: 48, color: "primary.main" }} />

          <Typography
            component="h1"
            sx={{
              fontSize: "1.25rem",
              fontWeight: "bold",
              textAlign: "center",
            }}
          >
            Create Account
          </Typography>

          {error && (
            <Alert severity="error" sx={{ width: "100%", marginBottom: 2 }}>
              {error}
            </Alert>
          )}

          {isSuccess ? (
            <Box sx={{ width: "100%" }}>
              <Alert severity="success" sx={{ width: "100%", marginBottom: 2 }}>
                Registration successful! Please check your email to confirm
                account creation.
              </Alert>

              <Button
                fullWidth
                variant="contained"
                sx={{
                  paddingY: 2,
                  backgroundColor: "primary.main",
                  "&:hover": {
                    backgroundColor: "primary.dark",
                  },
                  fontSize: "1rem",
                  height: 56,
                  textTransform: "none",
                }}
                onClick={() => router.push("/auth/login")}
              >
                Go to Login
              </Button>
            </Box>
          ) : (
            <form
              onSubmit={handleSubmit}
              sx={{
                width: "100%",
                display: "flex",
                flexDirection: "column",
                gap: 2,
              }}
            >
              <TextField
                label="First Name"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                required
                fullWidth
                autoComplete="given-name"
                sx={{ marginBottom: 2 }}
              />

              <TextField
                label="Last Name"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                required
                fullWidth
                autoComplete="family-name"
                sx={{ marginBottom: 2 }}
              />

              <TextField
                label="Email Address"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                required
                fullWidth
                autoComplete="email"
                sx={{ marginBottom: 2 }}
              />

              <TextField
                label="Password"
                name="password"
                type="password"
                value={formData.password}
                onChange={handleChange}
                required
                fullWidth
                autoComplete="new-password"
                sx={{ marginBottom: 2 }}
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
                sx={{ marginBottom: 3 }}
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
                  "Register"
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
                    Already have an account? Sign in
                  </Button>
                </Link>
              </Box>
            </form>
          )}
        </Box>
      </Paper>
    </Box>
  );
};

export default RegisterForm;
