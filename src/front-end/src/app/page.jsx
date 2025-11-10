"use client";

import React, { useEffect, useState } from "react";
import { getUserName, isAuthenticated } from "@/utils/token";
import msalInstance from "@/config/msalInstance";
import AppBarWithTitle from "@/components/AppBarWithTitle";
import { useRouter } from "next/navigation";

const HomePage = () => {
  const [userName, setUserName] = useState(null);
  const [isGuest, setIsGuest] = useState(false);
  const [ready, setReady] = useState(false);
  const [tableNumber, setTableNumber] = useState("");
  const router = useRouter();

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

  const handleJoinTable = () => {
    if (!tableNumber.trim()) return alert("Please enter a table number.");
    router.push(`/join?table=${tableNumber.trim()}`);
  };

  const handleGuestLogout = () => {
    localStorage.clear();
    window.location.reload();
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
        <h1 style={{ fontSize: "2.2rem", marginBottom: "0.5rem", fontWeight: "600" }}>
          Welcome to Sushi Toshi{userName ? `, ${userName}` : ""}!
        </h1>

        <p style={{ color: "#555", fontSize: "1.1rem", marginBottom: "2rem" }}>
          All You Can Eat Authentic Japanese Food.
        </p>

        {showGuestButton && (
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
            }}
          >
            Continue as Guest
          </button>
        )}

  {isGuest && (
    <div style={{ marginTop: "2rem" }}>
      {localStorage.getItem("tableNumber") ? (
        <h3>
          You’re seated at table {localStorage.getItem("tableNumber")}
        </h3>
      ) : (
        <>
          <h3>Enter Your Table Number</h3>
          <input
            type="number"
            placeholder="Table #"
            value={tableNumber}
            onChange={(e) => setTableNumber(e.target.value)}
            style={{
              padding: "10px",
              borderRadius: "6px",
              border: "1px solid #ccc",
              marginRight: "10px",
            }}
          />
          <button
            onClick={() => {
              if (!tableNumber.trim()) return alert("Please enter a table number.");
              localStorage.setItem("tableNumber", tableNumber.trim());
              router.push(`/join?table=${tableNumber.trim()}`);
            }}
            style={{
              backgroundColor: "#388e3c",
              color: "white",
              border: "none",
              borderRadius: "8px",
              padding: "10px 22px",
              cursor: "pointer",
            }}
          >
            Join Table
          </button>
        </>
      )}


            <div style={{ marginTop: "1.5rem" }}>
              <button
                onClick={handleGuestLogout}
                style={{
                  backgroundColor: "#d32f2f",
                  color: "white",
                  border: "none",
                  borderRadius: "8px",
                  padding: "10px 22px",
                  cursor: "pointer",
                }}
              >
                Logout as Guest
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default HomePage;
