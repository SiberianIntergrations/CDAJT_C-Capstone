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
  Divider,
} from "@mui/material";
import { Lock } from "lucide-react";
import { api } from '@/config/api';
import { useRouter, useSearchParams } from "next/navigation";
import { loginUser } from "@/utils/auth";
import { GoogleLogin,GoogleOAuthProvider } from '@react-oauth/google';


const googleClientID =  process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID
console.log(googleClientID)
const LoginForm = () => {
  const guestEmail = "guestemail@email.com"
  const guestPassword = "GuestUser!"
  const [userName, setUserName] = useState(null);
  const [isGuest, setIsGuest] = useState(false);
  const [ready, setReady] = useState(false);
  const searchParams = useSearchParams();
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: "admin.user@sushitoshi.ca",
    password: "AdminPass123!",
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

      console.log("Logging in");
      localStorage.clear()

      localStorage.setItem("locationId",searchParams.get("locationId"))
      localStorage.setItem("tableNumber",searchParams.get("tableNumber"))
      console.log()
      const response = await loginUser(formData.email, formData.password);

      console.log("AfterLogin");
      router.push("/");
    } catch (err) {
      console.error("Login error:", err);
      setError(
        err.response?.data?.detail ||
          err.response?.data?.message ||
          "Login failed. Please check your credentials and try again."
      );
    } finally {
      setLoading(false);
    }
  };

  const handleContinueAsGuest = async() => {
      localStorage.clear()

      localStorage.setItem("locationId",searchParams.get("locationId"))
      localStorage.setItem("tableNumber",searchParams.get("tableNumber"))
    const response = await loginUser(guestEmail,guestPassword);

    console.log("AfterLogin");
    router.push("/");
    localStorage.setItem("guest", "true");
    setIsGuest(true);
    setUserName("Guest");
    setReady(true);
};


  const handleGoogleSuccess= async (credentialResponse)=> {
    // Implement Google OAuth login here
    localStorage.clear()
    localStorage.setItem("locationId",searchParams.get("locationId"))
    localStorage.setItem("tableNumber",searchParams.get("tableNumber"))
    console.log('Credential received:', credentialResponse)
    try{
      const response = await api.post(`/auth/google`, {
        idToken: credentialResponse.credential
      })

      const data = response.data;
      console.log(`BackEnd Response: ${data}`)
      localStorage.setItem("authToken", response.data.access_token);
      localStorage.setItem('access_token', data.access_token);

      router.push("/");

    }
    catch(e){
      console.log(`Error: ${e}`)
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
        padding: { xs: 2, sm: 4, md: 6 },
      }}
    >
      <Paper
        sx={{
          padding: { xs: 3, sm: 5, md: 8 },
          width: "100%",
          maxWidth: "500px",
          backgroundColor: "white",
          borderRadius: 2,
        }}
      >
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: { xs: 2, sm: 3, md: 4 },
          }}
        >
          <Box
            sx={{
              borderRadius: "50%",
              backgroundColor: "red.100",
              padding: { xs: 0.75, sm: 1 },
            }}
          >
            <Lock
              sx={{
                width: { xs: 32, sm: 40 },
                height: { xs: 32, sm: 40 },
                color: "red.600",
              }}
            />
          </Box>

          <Typography
            variant="h4"
            component="h1"
            sx={{
              fontWeight: "bold",
              textAlign: "center",
              color: "gray.900",
              marginBottom: { xs: 2, sm: 3, md: 4 },
              fontSize: { xs: "1.5rem", sm: "2rem", md: "2.125rem" },
            }}
          >
            Login to Sushi Toshi
          </Typography>

          {error && (
            <Alert
              severity="error"
              sx={{
                width: "100%",
                marginBottom: { xs: 2, sm: 3, md: 4 },
                fontSize: { xs: "0.875rem", sm: "0.95rem" },
              }}
            >
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
              gap: { xs: 2, sm: 3 },
            }}
          >
            <GoogleOAuthProvider clientId={googleClientID}>
              <GoogleLogin
                onSuccess={handleGoogleSuccess}
                onError={() => {
                  console.log('Login Failed');
                  notifyError('Google login failed');
                }}
              />
            </GoogleOAuthProvider>


            <Divider sx={{ marginY: { xs: 1, sm: 2 } }}>
              <Typography variant="body2" color="text.secondary">
                OR
              </Typography>
            </Divider>

            <TextField
              label="Email Address"
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              required
              fullWidth
              variant="outlined"
              sx={{ backgroundColor: "white" }}
              size="medium"
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
              sx={{ backgroundColor: "white" }}
              size="medium"
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
                paddingY: { xs: 1.5, sm: 2 },
                fontSize: { xs: "1rem", sm: "1.1rem" },
                height: { xs: 48, sm: 56 },
                textTransform: "none",
                marginTop: { xs: "0.5rem", sm: "1rem" },
              }}
            >
              {loading ? (
                <CircularProgress size={24} sx={{ color: "white" }} />
              ) : (
                "Sign In"
              )}
            </Button>


            <Button
              onClick={handleContinueAsGuest}
              fullWidth
              variant="contained"
              disabled={loading}
              sx={{
                backgroundColor: "red.600",
                "&:hover": {
                  backgroundColor: "red.700",
                },
                color: "white",
                paddingY: { xs: 1.5, sm: 2 },
                fontSize: { xs: "1rem", sm: "1.1rem" },
                height: { xs: 48, sm: 56 },
                textTransform: "none",
                marginTop: { xs: "0.5rem", sm: "1rem" },
              }}
            >
              {loading ? (
                <CircularProgress size={24} sx={{ color: "white" }} />
              ) : (
                "Sign In As Guest"
              )}
            </Button>           

            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: { xs: 2, sm: 3 },
                marginTop: { xs: 1, sm: 2 },
              }}
            >
              <Button
                onClick={handleForgotPassword}
                fullWidth
                variant="text"
                sx={{
                  color: "gray.600",
                  "&:hover": {
                    color: "gray.800",
                    backgroundColor: "gray.50",
                  },
                  fontSize: { xs: "0.875rem", sm: "1rem" },
                  paddingY: { xs: 1, sm: 1.5 },
                  textTransform: "none",
                }}
              >
                Forgot Password?
              </Button>

              <Button
                onClick={handleRegister}
                fullWidth
                variant="text"
                sx={{
                  color: "gray.600",
                  "&:hover": {
                    color: "gray.800",
                    backgroundColor: "gray.50",
                  },
                  fontSize: { xs: "0.875rem", sm: "1rem" },
                  paddingY: { xs: 1, sm: 1.5 },
                  textTransform: "none",
                }}
              >
                Create New Account
              </Button>
            </Box>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
};

export default LoginForm;