"use client";
import { useState } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  Alert,
  CircularProgress,
} from "@mui/material";
import { Lock } from "lucide-react";
import { useRouter } from "next/navigation";
import { loginUser } from "@/utils/auth";

const LoginForm = () => {
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: "jamie2@example.com",
    password: "somethingCool1",
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

    try {
      console.log("Attempting login...");
      const response = await loginUser(formData.email, formData.password);
      console.log("Login response:", response);
      router.push("/");
    } catch (err) {
      console.error("Login error:", err);
      setError(
        err.response?.data?.detail ||
          "Login failed. Please check your credentials and try again."
      );
    } finally {
      setLoading(false);
    }
  };

  const handleForgotPassword = () => {
    router.push("/auth/forgot-password");
  };

  const handleRegister = () => {
    router.push("/auth/register");
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        backgroundColor: "gray.100",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 6,
      }}
    >
      <Paper
        sx={{
          padding: 8,
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
            gap: 4,
          }}
        >
          <Box
            sx={{ borderRadius: "50%", backgroundColor: "red.100", padding: 1 }}
          >
            <Lock sx={{ width: 40, height: 40, color: "red.600" }} />
          </Box>

          <Typography
            variant="h4"
            component="h1"
            sx={{
              fontWeight: "bold",
              textAlign: "center",
              color: "gray.900",
              marginBottom: 4,
            }}
          >
            Login to Sushi Toshi
          </Typography>

          {error && (
            <Alert
              severity="error"
              sx={{ width: "100%", marginBottom: 4, fontSize: "0.95rem" }}
            >
              {error}
            </Alert>
          )}

          <form
            onSubmit={handleSubmit}
            sx={{
              width: "100%",
              display: "flex",
              flexDirection: "column",
              gap: 4,
            }}
          >
            <TextField
              label="Email Address"
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              required
              fullWidth
              variant="outlined"
              sx={{ backgroundColor: "white", marginBottom: 4 }}
              size="large"
            />

            <TextField
              label="Password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              required
              fullWidth
              variant="outlined"
              sx={{ backgroundColor: "white", marginBottom: 2 }}
              size="large"
            />

            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={loading}
              sx={{
                backgroundColor: "red.600",
                "&:hover": {
                  backgroundColor: "red.700",
                },
                color: "white",
                paddingY: 2,
                marginBottom: 4,
                fontSize: "1.1rem",
                height: 56,
                textTransform: "none",
                marginTop: "1rem",
              }}
            >
              {loading ? (
                <CircularProgress size={28} sx={{ color: "white" }} />
              ) : (
                "Sign In"
              )}
            </Button>

            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: 3,
              }}
            >
              <Button
                onClick={handleForgotPassword}
                sx={{
                  color: "gray.600",
                  "&:hover": { color: "gray.800" },
                  fontSize: "1rem",
                  padding: "0rem 1rem",
                  textTransform: "none",
                }}
              >
                Forgot Password?
              </Button>

              <Button
                onClick={handleRegister}
                sx={{
                  color: "gray.600",
                  "&:hover": { color: "gray.800" },
                  fontSize: "1rem",
                  padding: "0rem 1rem",
                  textTransform: "none",
                }}
              >
                Create New Account
              </Button>
            </Box>
          </form>
        </Box>
      </Paper>
    </Box>
  );
};

export default LoginForm;