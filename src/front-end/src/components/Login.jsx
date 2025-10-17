// import React, { useState } from "react";
// import { TextField, Button, Container, Typography, Alert } from "@mui/material";
// import axios from "axios";

// export default function Login() {
//   const [email, setEmail] = useState("");
//   const [password, setPassword] = useState("");
//   const [error, setError] = useState("");
//   const [success, setSuccess] = useState("");

//   function handleEmailChange(event) {
//     setEmail(event.target.value);
//   }

//   function handlePasswordChange(event) {
//     setPassword(event.target.value);
//   }

//   function saveAuthToken(jwtToken) {
//     localStorage.setItem("access_token", jwtToken);
//   }

//   function handleSubmit(event) {
//     event.preventDefault();
//     setError("");
//     setSuccess("");

//     axios
//       .post("/auth/login", {
//         username: email,
//         password: password,
//       })
//       .then(function (response) {
//         console.log("Login successful:", response.data);
//         setSuccess("Login successful! Redirecting...");
//         saveAuthToken(response.data.access_token);
//       })
//       .catch(function (error) {
//         setError(
//           error.response?.data?.detail || "An error occurred during login"
//         );
//       });
//   }

//   return (
//     <Container maxWidth="sm">
//       <Typography variant="h4" align="center" gutterBottom>
//         Login
//       </Typography>
//       <form onSubmit={handleSubmit}>
//         <TextField
//           fullWidth
//           label="Email"
//           variant="outlined"
//           margin="normal"
//           value={email}
//           onChange={handleEmailChange}
//           required
//         />
//         <TextField
//           fullWidth
//           label="Password"
//           type="password"
//           variant="outlined"
//           margin="normal"
//           value={password}
//           onChange={handlePasswordChange}
//           required
//         />
//         <Button type="submit" variant="contained" color="primary" fullWidth>
//           Login
//         </Button>
//       </form>
//       {error && (
//         <Alert severity="error" style={{ marginTop: "20px" }}>
//           {error}
//         </Alert>
//       )}
//       {success && (
//         <Alert severity="success" style={{ marginTop: "20px" }}>
//           {success}
//         </Alert>
//       )}
//     </Container>
//   );
// }
