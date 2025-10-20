import { useState, useEffect } from "react";
import {
  Box,
  Card,
  CardContent,
  Typography,
  Badge,
  IconButton,
} from "@mui/material";
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
  marginLeft: theme.spacing(1),
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

const AddButton = styled(IconButton)(({ theme }) => ({
  backgroundColor: theme.palette.primary.main,
  color: "white",
  padding: theme.spacing(1),
  minWidth: "auto",
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
  },
  "& svg": {
    width: 20,
    height: 20,
  },
}));

const DashboardSummary = () => {
  const { dashboardSummary, openDialog } = useSession();
  const [serviceRequests, setServiceRequests] = useState(0);
  const [isNewRequest, setIsNewRequest] = useState(false);
  const [previousCount, setPreviousCount] = useState(0);

  const checkServiceRequests = async () => {
    try {
      const response = await api.get("/ServiceRequest"); // service-requests/for-all-sessions

      if (response.status === 200) {
        const data = response.data;
        setPreviousCount(serviceRequests);
        setServiceRequests(data.length);

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
    <Box className="grid grid-cols-1 sm:grid-cols-2 gap-2 mb-3">
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
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                mb: 0.5,
              }}
            >
              <Box display="flex" alignItems="center" gap={2}>
                <Typography
                  variant="h5"
                  color={serviceRequests > 0 ? "warning.dark" : "primary.dark"}
                >
                  Active Sessions: {dashboardSummary.total_active_sessions}
                </Typography>
                <AddButton onClick={() => openDialog("newSession")}>
                  <Plus />
                </AddButton>
              </Box>
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

      <CompactCard>
        <Box sx={{ width: "100%", textAlign: "left" }}>
          <Typography variant="h5" color="secondary">
            Tables in Use: {dashboardSummary.total_tables_in_use}
          </Typography>
        </Box>
      </CompactCard>
    </Box>
  );
};

export default DashboardSummary;
