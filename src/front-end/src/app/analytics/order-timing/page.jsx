"use client";
import React from "react";
import { useRouter } from "next/navigation";
import OrderTiming from "@/components/analytics/OrderTiming";

const OrderTimingPage = () => {
  const router = useRouter();

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
