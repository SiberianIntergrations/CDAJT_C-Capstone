// File: sushi-toshi-frontend/components/staff/SessionDashboard/index.jsx
import { Box, Alert, CircularProgress } from '@mui/material';
import { styled, keyframes } from '@mui/material/styles';
import { useSessionData } from './hooks/useSessionData';
import { SessionProvider } from './context/SessionContext';
import DashboardSummary from './components/DashboardSummary';
import SessionList from './components/SessionList';
import DialogContainer from './components/dialogs/DialogContainer';
import { useState, useEffect } from 'react';

const pulseAnimation = keyframes`
  0% {
    background-color: #ffffff;
  }
  50% {
    background-color: rgba(255, 193, 7, 0.15);
  }
  100% {
    background-color: #ffffff;
  }
`;

const PageWrapper = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isBlinking'
})(({ theme, isBlinking }) => ({
  minHeight: '100vh',
  width: '100%',
  position: 'fixed',
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  zIndex: 0,
  animation: isBlinking ? `${pulseAnimation} 2s ease-in-out infinite` : 'none',
  backgroundColor: '#ffffff',
}));

const DashboardContainer = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2),
  maxWidth: '600px',
  margin: '0 auto',
  position: 'relative',
  zIndex: 1,
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(3),
  },
}));

const LoadingContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  minHeight: '100vh',
}));

const StyledAlert = styled(Alert)(({ theme }) => ({
  marginBottom: theme.spacing(2),
}));

const DashboardContent = ({ children, isBlinking }) => (
  <>
    <PageWrapper isBlinking={isBlinking} />
    <DashboardContainer>{children}</DashboardContainer>
  </>
);

const SessionDashboardInner = () => {
  const { isLoading, error, sessions, setError, fetchSessions } = useSessionData();
  const [isBlinking, setIsBlinking] = useState(false);
  const [activeSessions, setActiveSessions] = useState(0);

  useEffect(() => {
    const updateSessionStats = () => {
      const activeCount = sessions?.filter(session => !session.ended_at).length || 0;
      setActiveSessions(activeCount);

      const needsAttention = sessions?.some(session => {
        const hasOpenBills = session.bills?.some(bill => bill.status === 'OPEN');
        const hasPendingOrders = session.orders_count > 0;
        return hasOpenBills && hasPendingOrders;
      });
      setIsBlinking(needsAttention);
    };

    updateSessionStats();

    const intervalId = setInterval(async () => {
      await fetchSessions();
      updateSessionStats();
    }, 30000);

    return () => clearInterval(intervalId);
  }, [sessions, fetchSessions]);

  const handleSessionCreated = async () => {
    setActiveSessions(prev => prev + 1);
    await fetchSessions();
  };

  const handleSessionEnded = async () => {
    setActiveSessions(prev => Math.max(0, prev - 1));
    await fetchSessions();
  };

  if (isLoading) {
    return (
      <LoadingContainer>
        <CircularProgress />
      </LoadingContainer>
    );
  }

  return (
    <DashboardContent isBlinking={isBlinking}>
      {error && (
        <StyledAlert severity="error" onClose={() => setError(null)}>
          {error}
        </StyledAlert>
      )}
      <DashboardSummary 
        activeSessions={activeSessions} 
        onSessionCreated={handleSessionCreated} 
      />
      <SessionList 
        sessions={sessions}
        onSessionEnd={handleSessionEnded}
      />
      <DialogContainer 
        onSessionCreated={handleSessionCreated} 
      />
    </DashboardContent>
  );
};

const SessionDashboard = () => (
  <SessionProvider>
    <SessionDashboardInner />
  </SessionProvider>
);

export default SessionDashboard;