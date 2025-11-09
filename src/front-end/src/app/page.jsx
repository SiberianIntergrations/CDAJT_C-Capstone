"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getUserName, isAuthenticated } from "@/utils/token";
import msalInstance from "@/config/msalInstance"; 

const HomePage = () => {
  const router = useRouter();
  const [userName, setUserName] = useState(null);
  const [isGuest, setIsGuest] = useState(false);

  useEffect(() => {
    if (isAuthenticated()) {
      const name = getUserName();
      setUserName(name);
      return;
    }

    try {
      const accounts = msalInstance.getAllAccounts?.();
      if (accounts && accounts.length > 0) {
        const account = accounts[0];
        setUserName(account.name || account.username || "User");
      }
    } catch (err) {
      console.warn("MSAL not ready yet:", err);
    }

    if (typeof window !== "undefined" && localStorage.getItem("guest") === "true") {
      setIsGuest(true);
    }
  }, []);

  // Simulates a guest sign-in
  const handleContinueAsGuest = () => {
    localStorage.setItem("guest", "true");
    setIsGuest(true);
    setUserName("Guest");
    console.log("GUEST - signed in");
  };

  // Handles sign out for both guest and authenticated users
  const handleSignOut = async () => {
    if (isGuest) {
      localStorage.removeItem("guest");
      setIsGuest(false);
      setUserName(null);
      console.log("Guest signed out");
    } else {
      try {
        await msalInstance.logoutRedirect();
      } catch (err) {
        console.error("Error during MSAL logout:", err);
      }
    }
  };

  return (
    <div
      style={{
        padding: "3rem 2rem",
        maxWidth: "800px",
        margin: "0 auto",
        textAlign: "center",
      }}
    >
      <h1
        style={{
          fontSize: "2.2rem",
          marginBottom: "0.5rem",
          fontWeight: "600",
        }}
      >
        Welcome to Sushi Toshi{userName ? `, ${userName}` : ""}!
      </h1>

      <p
        style={{
          color: "#555",
          fontSize: "1.1rem",
          marginTop: "0.25rem",
          marginBottom: "2rem",
          textAlign: "center",
        }}
      >
        All You Can Eat Authentic Japanese Food.
      </p>

      {!userName && !isGuest && (
        <div style={{ marginTop: "1rem" }}>
          <button
            onClick={handleContinueAsGuest}
            style={{
              backgroundColor: "#1976d2",
              color: "white",
              border: "none",
              borderRadius: "8px",
              padding: "10px 22px",
              fontSize: "1rem",
              cursor: "pointer",
              transition: "background-color 0.2s ease",
            }}
            onMouseOver={(e) => (e.target.style.backgroundColor = "#1565c0")}
            onMouseOut={(e) => (e.target.style.backgroundColor = "#1976d2")}
          >
            Continue as Guest
          </button>
        </div>
      )}

      {(isGuest || userName) && (
        <div style={{ marginTop: "1.5rem" }}>
          <button
            onClick={handleSignOut}
            style={{
              background: "none",
              border: "none",
              color: "#d32f2f",
              fontSize: "0.95rem",
              textDecoration: "underline",
              cursor: "pointer",
            }}
          >
            Sign Out
          </button>
        </div>
      )}
    </div>
  );
};

export default HomePage;
