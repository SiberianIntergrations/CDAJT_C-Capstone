// File: sushi-toshi-frontend/pages/analytics/browsing-behavior.js
import React from 'react';
import { useRouter } from 'next/router';
import BrowsingBehavior from '../../components/analytics/BrowsingBehavior';

const BrowsingBehaviorPage = () => {
  const router = useRouter();

  return (
    <div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem', marginTop: '1rem' }}>
        <button onClick={() => router.push('/analytics/item-performance')}>Item Performance</button>
        <button onClick={() => router.push('/analytics/order-timing')}>Order Timing</button>
        <button onClick={() => router.push('/analytics/table-turnover')}>Table Turnover</button>
      </div>
      <BrowsingBehavior />
    </div>
  );
}

export default BrowsingBehaviorPage;
