import { useState, useCallback, useRef, useEffect } from "react";
import {
  Card,
  CardContent,
  Stack,
  Divider,
  Box,
  Typography,
  IconButton,
  Button,
  Snackbar,
  Alert,
  Collapse,
} from "@mui/material";
import { ChevronDown, ChevronUp, Bell, Check, AlertCircle } from "lucide-react";
import { styled } from "@mui/material/styles";
import TableSection from "./TableSection";
import BillSection from "./BillSection";
import SessionHeader from "./SessionHeader";
import { useSession } from "../../context/SessionContext";
import api from "@/config/api";

const CardWrapper = styled("div")(({ theme }) => ({
  position: "relative",
  marginBottom: theme.spacing(2),
  transition: "transform 0.3s ease-out",
  background: "none",
}));

const ConfirmEndButton = styled(Box, {
  shouldForwardProp: (prop) => prop !== "showConfirm",
})(({ theme, showConfirm }) => ({
  position: "absolute",
  top: 0,
  left: 0,
  width: "100%",
  height: "100%",
  display: showConfirm ? "flex" : "none",
  alignItems: "center",
  backgroundColor: "#C01E2E",
  color: "#ffffff",
  zIndex: 2,
  borderRadius: theme.shape.borderRadius,
  transition: "all 0.3s ease-out",
}));

const StyledCard = styled(Card, {
  shouldForwardProp: (prop) =>
    prop !== "dimmed" && prop !== "hasRequests" && prop !== "isBlinking",
})(({ theme, dimmed, hasRequests, isBlinking }) => ({
  marginBottom: 0,
  backgroundColor: hasRequests
    ? "rgba(255, 220, 100, 0.15)"
    : theme.palette.background.paper,
  transition: "all 0.3s ease",
  opacity: dimmed ? 0.5 : 1,
  pointerEvents: dimmed ? "none" : "auto",
  boxShadow: theme.shadows[2],
  borderRadius: `${theme.shape.borderRadius}px`,
  animation:
    isBlinking && hasRequests ? "blink 1s ease-in-out infinite" : "none",
  "@keyframes blink": {
    "0%": { backgroundColor: "rgba(255, 220, 100, 0.25)" },
    "50%": { backgroundColor: "rgba(255, 220, 100, 0.5)" },
    "100%": { backgroundColor: "rgba(255, 220, 100, 0.25)" },
  },
  "&:hover": {
    boxShadow: dimmed ? theme.shadows[2] : theme.shadows[4],
  },
}));

const ServiceRequest = ({ request, onComplete }) => (
  <Box
    sx={{
      display: "flex",
      alignItems: "center",
      justifyContent: "space-between",
      bgcolor: "rgba(255, 220, 100, 0.6)",
      borderRadius: 1,
      px: 2,
      py: 1,
      my: 0.5,
    }}
  >
    <Typography variant="body2">{request.notes}</Typography>
    <IconButton
      component="span"
      size="small"
      onClick={(e) => {
        e.stopPropagation();
        onComplete(request.request_id);
      }}
      sx={{ ml: 1 }}
    >
      <Check size={16} />
    </IconButton>
  </Box>
);

const EndSessionButton = styled(Button)(({ theme }) => ({
  marginTop: theme.spacing(2),
  width: "100%",
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  gap: theme.spacing(1),
  backgroundColor: "#C01E2E",
  "&:hover": {
    backgroundColor: "#A01725",
  },
  "&.Mui-disabled": {
    backgroundColor: "rgba(192, 30, 46, 0.5)",
  },
}));

const SessionCard = ({ session, onRequestUpdate }) => {
  const [requests, setRequests] = useState([]);
  const [isBlinking, setIsBlinking] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [isEnding, setIsEnding] = useState(false);
  const [expanded, setExpanded] = useState(false);
  const { endSession, actionError, clearActionError } = useSession();

  useEffect(() => {
    if (actionError) {
      setShowConfirm(false);
    }
  }, [actionError]);

  const fetchRequests = useCallback(async () => {
    try {
      const response = await api.get(
        `/ServiceRequest/by-session/${session.session_Id}`
      );

      if (response.status !== 200) throw new Error("Failed to fetch requests");
      const data = response.data;

      setRequests(data);
      setIsBlinking(data.length > 0);
      onRequestUpdate(session.session_Id, data);
    } catch (error) {
      console.error("Error fetching requests:", error);
      setRequests([]); // Clear requests on error
    }
  }, [session.session_Id, onRequestUpdate]);

  const handleComplete = async (requestId) => {
    try {
      const response = await api.post(`/ServiceRequest/${requestId}/complete`);

      if (response.status !== 200)
        throw new Error("Failed to complete request");
      await fetchRequests();
    } catch (error) {
      console.error("Error completing request:", error);
    }
  };

  const handleInitiateEnd = useCallback((e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    setExpanded(false);
    setShowConfirm(true);
  }, []);

  const handleCancelEnd = (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    setShowConfirm(false);
    setIsEnding(false);
  };

  const handleConfirmEnd = async (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }

    if (isEnding) return;

    try {
      setIsEnding(true);
      clearActionError();
      const success = await endSession(session.session_Id);
      if (!success) {
        // Error handling already done by context
      }
    } catch (error) {
      console.error("Error ending session:", error);
    } finally {
      setIsEnding(false);
      setShowConfirm(false);
      setDragX(0);
    }
  };

  useEffect(() => {
    if (isEnding) return;

    fetchRequests();
    const interval = setInterval(fetchRequests, 5000);
    return () => clearInterval(interval);
  }, [fetchRequests, isEnding]);

  const handleErrorClose = () => {
    clearActionError();
  };

  return (
    <CardWrapper>
      <Snackbar
        open={Boolean(actionError)}
        autoHideDuration={6000}
        onClose={handleErrorClose}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={handleErrorClose}
          severity="error"
          variant="filled"
          sx={{ width: "100%" }}
        >
          {actionError}
        </Alert>
      </Snackbar>

      {showConfirm && (
        <ConfirmEndButton showConfirm={showConfirm}>
          <Box
            sx={{
              px: 3,
              width: "100%",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              gap: 2,
            }}
            onClick={(e) => e.stopPropagation()}
          >
            <Button
              variant="outlined"
              onClick={handleCancelEnd}
              disabled={isEnding}
              sx={{
                borderColor: "white",
                color: "white",
                "&:hover": {
                  borderColor: "white",
                  backgroundColor: "rgba(255, 255, 255, 0.1)",
                },
              }}
            >
              Cancel
            </Button>
            <Typography
              sx={{
                textAlign: "center",
                color: "white",
                flexGrow: 1,
              }}
            >
              End Session #{session.session_Id}?
            </Typography>
            <Button
              variant="contained"
              onClick={handleConfirmEnd}
              disabled={isEnding}
              sx={{
                bgcolor: "white",
                color: "#C01E2E",
                "&:hover": {
                  bgcolor: "rgba(255, 255, 255, 0.9)",
                },
              }}
            >
              {isEnding ? "Ending..." : "Confirm"}
            </Button>
          </Box>
        </ConfirmEndButton>
      )}

      <StyledCard
        dimmed={showConfirm}
        hasRequests={requests.length > 0}
        sx={{ opacity: showConfirm ? 0.5 : 1 }}
      >
        <CardContent>
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="flex-start"
          >
            <Box flex={1}>
              <Box display="flex" alignItems="center" gap={2}>
                <Typography variant="h6">
                  Session #{session.session_Id}
                </Typography>
                <Typography variant="h6">{session.location_Name}</Typography>
                {requests.length > 0 && (
                  <Chip
                    size="small"
                    color="warning"
                    label={requests.length}
                    icon={<Bell size={16} />}
                  />
                )}
                <Chip
                  size="small"
                  label={session.is_closable ? "Ready to Close" : "Active"}
                  color={session.is_closable ? "success" : "primary"}
                />
              </Box>

              {requests.length > 0 && (
                <Box my={2}>
                  {requests.map((request) => (
                    <ServiceRequest
                      key={request.request_id}
                      request={request}
                      onComplete={handleComplete}
                    />
                  ))}
                </Box>
              )}
            </Box>

            <IconButton
              size="small"
              onClick={() => setExpanded(!expanded)}
              disabled={showConfirm || isEnding}
            >
              {expanded ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
            </IconButton>
          </Box>

          <Collapse in={expanded}>
            <Stack spacing={2}>
              <TableSection session={session} />
              <BillSection session={session} />
              <Divider />
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "center",
                  width: "100%",
                  pt: 1,
                }}
              >
                <EndSessionButton
                  variant="contained"
                  onClick={handleInitiateEnd}
                  disabled={isEnding}
                  startIcon={<AlertCircle />}
                >
                  End Session
                </EndSessionButton>
              </Box>
            </Stack>
          </Collapse>
        </CardContent>
      </StyledCard>
    </CardWrapper>
  );
};

export default SessionCard;
