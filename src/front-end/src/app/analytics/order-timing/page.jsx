"use client";
import React, { useContext } from "react";
import { useRouter } from "next/navigation";
import OrderTiming from "@/components/analytics/OrderTiming";
import { AuthContext } from "@/app/layout";

const OrderTimingPage = () => {
  const router = useRouter();
  const { userRole, isLoggedIn, authLoading } = useContext(AuthContext);

  if (authLoading) {
    return <div>Loading...</div>;
  }

  if (!isLoggedIn || userRole !== "admin") {
    console.warn("Unauthorized access attempt to Order Timing page.");
    return null;
  }

  return (
    <div>
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 1fr)",
          gap: "1rem",
          marginTop: "1rem",
        }}
      >
        <button onClick={() => router.push("/analytics/item-performance")}>
          Item Performance
        </button>
        <button onClick={() => router.push("/analytics/browsing-behavior")}>
          Browsing Behavior
        </button>
        <button onClick={() => router.push("/analytics/table-turnover")}>
          Table Turnover
        </button>
      </div>
      <OrderTiming />
    </div>
  );
};

export default OrderTimingPage;
