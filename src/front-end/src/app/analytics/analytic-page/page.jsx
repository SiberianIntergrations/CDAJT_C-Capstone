"use client";
import React, { useContext } from "react";
import { AuthContext } from "@/app/layout";
import { useRouter } from "next/navigation";
import styles from "@/styles/Analytics.module.css";

function AnalyticsPage() {
  const { userRole, isLoggedIn, authLoading } = useContext(AuthContext);
  const router = useRouter();

  if (authLoading) {
    return <div className={styles.menuTitleContainer}><p>Loading...</p></div>;
  }

  if (!isLoggedIn || userRole !== "admin") {
    console.warn("Unauthorized access attempt to Analytics page.");
    return null;
  }

  return (
    <div className={styles.menuTitleContainer}>
      <h1 className={styles.menuTitle}>Analytics</h1>
      <p className={styles.description}>Welcome to the analytics page!</p>

      <div className={styles.categoryList}>
        <button
          className={styles.categoryButton}
          onClick={() => router.push("/analytics/item-performance")}
        >
          View Item Performance
        </button>

        <button
          className={styles.categoryButton}
          onClick={() => router.push("/analytics/browsing-behavior")}
        >
          View Browsing Behavior
        </button>

        <button
          className={styles.categoryButton}
          onClick={() => router.push("/analytics/order-timing")}
        >
          View Order Timing
        </button>

        <button
          className={styles.categoryButton}
          onClick={() => router.push("/analytics/table-turnover")}
        >
          View Table Turnover
        </button>
      </div>
    </div>
  );
}

export default AnalyticsPage;
