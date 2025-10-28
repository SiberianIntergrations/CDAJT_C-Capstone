'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import dynamic from 'next/dynamic';

// Dynamically import the QR code management component
const QRCodeManagement = dynamic(
  () => import('@/components/admin/QRCodeManagement'),
  { ssr: false }
);

export default function QRCodesPage() {
  const router = useRouter();
  const [isAuthorized, setIsAuthorized] = useState(false);

  useEffect(() => {
    // Check if user is authenticated and has admin/staff role
    const token = localStorage.getItem('token');
    const userRole = localStorage.getItem('userRole');

    if (!token) {
      router.push('/login');
      return;
    }

    // Allow both admin and staff to access QR code generation
    if (userRole === 'Admin' || userRole === 'Staff') {
      setIsAuthorized(true);
    } else {
      router.push('/dashboard');
    }
  }, [router]);

  if (!isAuthorized) {
    return (
      <div style={{ padding: '20px', textAlign: 'center' }}>
        <p>Checking authorization...</p>
      </div>
    );
  }

  return <QRCodeManagement />;
}
