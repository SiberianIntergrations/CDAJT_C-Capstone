import { useState, useEffect } from "react";
import { Box, Card, Typography, Badge } from "@mui/material";
import { Bell, Plus } from "lucide-react";
import { keyframes, styled } from "@mui/material/styles";
import { useSession } from "../context/SessionContext";
import api from "@/config/api";

const pulseAnimation = keyframes`
  0% { transform: scale(1); }
  50% { transform: scale(1.2); }
  100% { transform: scale(1); }
`;

const ringAnimation = keyframes`
  0% { transform: rotate(0); }
  15% { transform: rotate(15deg); }
  30% { transform: rotate(-15deg); }
  45% { transform: rotate(15deg); }
  60% { transform: rotate(-15deg); }
  75% { transform: rotate(0); }
  100% { transform: rotate(0); }
`;

const StyledBell = styled(Bell, {
  shouldForwardProp: (prop) => prop !== "isNew",
})(({ theme, isNew }) => ({
  color: theme.palette.warning.main,
  animation: isNew
    ? `${ringAnimation} 1s ease-in-out, ${pulseAnimation} 1s ease-in-out`
    : "none",
}));

const CompactCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== "hasRequests",
})(({ theme, hasRequests }) => ({
  backgroundColor: hasRequests
    ? theme.palette.warning.light
    : theme.palette.common.white,
  transition: "background-color 0.3s ease",
  minHeight: "80px",
  display: "flex",
  alignItems: "center",
  padding: theme.spacing(1.5),
}));

const AddButton = styled(Box)(({ theme }) => ({
  backgroundColor: theme.palette.common.white,
  border: `2px dashed ${theme.palette.divider}`,
  borderRadius: theme.spacing(1),
  padding: theme.spacing(2),
  cursor: "pointer",
  transition: "all 0.2s ease-in-out",
  "&:hover": {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.action.hover,
    transform: "translateX(4px)",
  },
}));

const DashboardSummary = () => {
  const { dashboardSummary, openDialog, sessions } = useSession();
  const [serviceRequests, setServiceRequests] = useState(0);
  const [isNewRequest, setIsNewRequest] = useState(false);
  const [previousCount, setPreviousCount] = useState(0);

  const checkServiceRequests = async () => {
    try {
      const response = await api.get("/ServiceRequest/pending");

      if (response.status === 200) {
        const data = response.data;

        // filter to only use service request that are for a location
        const filtereddata = data.filter((sr) =>
          sessions.map((s) => s.session_Id).includes(sr.session_id)
        );

        setPreviousCount(serviceRequests);
        setServiceRequests(filtereddata.length);

        if (data.length > previousCount && previousCount !== 0) {
          setIsNewRequest(true);
          setTimeout(() => setIsNewRequest(false), 1000);
        }
      }
    } catch (error) {
      console.error("Error fetching service requests:", error);
    }
  };

  useEffect(() => {
    checkServiceRequests();
    const interval = setInterval(checkServiceRequests, 5000);
    return () => clearInterval(interval);
  }, []);

  return (
    <Box
      sx={{
        marginBottom: "20px",
        display: "flex",
        flexDirection: "column",
        gap: "1rem",
      }}
    >
      <CompactCard hasRequests={serviceRequests > 0}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            width: "100%",
          }}
        >
          <Box sx={{ flex: 1 }}>
            <Box display="flex" alignItems="center" gap={2}>
              <Typography
                variant="h5"
                color={serviceRequests > 0 ? "warning.dark" : "primary.dark"}
              >
                Active Sessions: {dashboardSummary.total_active_sessions}
              </Typography>
            </Box>
          </Box>

          {serviceRequests > 0 && (
            <Badge
              badgeContent={serviceRequests}
              color="warning"
              sx={{ ml: 1, mr: 1 }}
            >
              <StyledBell size={16} isNew={isNewRequest} />
            </Badge>
          )}
        </Box>
      </CompactCard>

      <AddButton onClick={() => openDialog("newSession")}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
          <Plus size={20} />
          <Box>
            <Typography variant="subtitle1" color="text.primary">
              Add New Session
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Start a new dining session
            </Typography>
          </Box>
        </Box>
      </AddButton>
    </Box>
  );
};

export default DashboardSummary;
