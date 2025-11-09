"use client";

import React, { useEffect, useState } from "react";
import { getUserName, isAuthenticated } from "@/utils/token";
import msalInstance from "@/config/msalInstance";
import AppBarWithTitle from "@/components/AppBarWithTitle";

const HomePage = () => {
  const [userName, setUserName] = useState(null);
  const [isGuest, setIsGuest] = useState(false);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    const init = async () => {
      let name = null;
      let guestMode = false;

      if (isAuthenticated()) {
        name = getUserName();
      }

      if (!name) {
        try {
          await msalInstance.initialize();
          const accounts = msalInstance.getAllAccounts();
          if (accounts?.length > 0) {
            const acc = accounts[0];
            name = acc.name || acc.username || "User";
          }
        } catch {}
      }

      if (!name && localStorage.getItem("guest") === "true") {
        guestMode = true;
        name = "Guest";
      }

      setIsGuest(guestMode);
      setUserName(name);
      setReady(true);
    };

    init();
  }, []);


  const handleContinueAsGuest = () => {
    localStorage.setItem("guest", "true");
    setIsGuest(true);
    setUserName("Guest");
    setReady(true);
  };

  if (!ready) return null;

  const showGuestButton = !userName && !isGuest;

  return (
    <div>
      {(userName || isGuest) && <AppBarWithTitle />}

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
          }}
        >
          All You Can Eat Authentic Japanese Food.
        </p>

        {showGuestButton && (
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
              onMouseOver={(e) => (e.currentTarget.style.backgroundColor = "#1565c0")}
              onMouseOut={(e) => (e.currentTarget.style.backgroundColor = "#1976d2")}
            >
              Continue as Guest
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default HomePage;