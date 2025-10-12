// // File: sushi-toshi-frontend/components/staff/SessionDashboard/components/SessionCard/SessionActions.jsx
// import { Box, Button } from '@mui/material';
// import { Ban } from 'lucide-react';
// import { useSession } from '../../context/SessionContext';

// const SessionActions = ({ session }) => {
//   const { endSession } = useSession();

//   const handleEndSession = async () => {
//     if (!confirm('Are you sure you want to end this session?')) return;
//     await endSession(session.session_id);
//   };

//   return (
//     <Box className="flex justify-end">
//       <Button
//         variant="outlined"
//         color="error"
//         startIcon={<Ban />}
//         onClick={handleEndSession}
//         disabled={!session.is_closable}
//       >
//         End Session
//       </Button>
//     </Box>
//   );
// };

// export default SessionActions;
