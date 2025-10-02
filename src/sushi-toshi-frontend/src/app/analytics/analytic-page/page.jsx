// File: sushi-toshi-frontend/pages/analytics/analytic-page.js
import React from 'react';
import { useRouter } from 'next/router';
import styles from '../../styles/Analytics.module.css';

function AnalyticsPage() {
  const router = useRouter();

  return (
    <div className={styles.menuTitleContainer}>
      <h1 className={styles.menuTitle}>Analytics</h1>
      <p className={styles.description}>Welcome to the analytics page!</p>

      <div className={styles.categoryList}>
        <button
          className={styles.categoryButton}
          onClick={() => router.push('/analytics/item-performance')}
        >
          View Item Performance
        </button>

        <button
          className={styles.categoryButton}
          onClick={() => router.push('/analytics/browsing-behavior')}
        >
          View Browsing Behavior
        </button>

        <button
          className={styles.categoryButton}
          onClick={() => router.push('/analytics/order-timing')}
        >
          View Order Timing
        </button>

        <button
          className={styles.categoryButton}
          onClick={() => router.push('/analytics/table-turnover')}
        >
          View Table Turnover
        </button>
      </div>
    </div>
  );
}

export default AnalyticsPage;