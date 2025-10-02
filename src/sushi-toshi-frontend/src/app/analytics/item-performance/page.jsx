// File: sushi-toshi-frontend/pages/analytics/item-performance.js
import React from 'react';
import { useRouter } from 'next/router';
import ItemPerformance from '../../components/analytics/ItemPerformance';

const ItemPerformancePage = () => {
  const router = useRouter();

  return (
    <div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem', marginTop: '1rem' }}>
        <button onClick={() => router.push('/analytics/browsing-behavior')}>Browsing Behavior</button>
        <button onClick={() => router.push('/analytics/order-timing')}>Order Timing</button>
        <button onClick={() => router.push('/analytics/table-turnover')}>Table Turnover</button>
      </div>
      <ItemPerformance />
    </div>
  );
}

export default ItemPerformancePage;
