import { useState, useCallback, useRef, useEffect } from "react";
import {
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Stack,
  Divider,
  Box,
  Chip,
  Typography,
  IconButton,
  Button,
  Snackbar,
  Alert,
} from "@mui/material";
import { ChevronDown, Bell, Check, AlertCircle } from "lucide-react";
import { styled } from "@mui/material/styles";
import TableSection from "./TableSection";
import BillSection from "./BillSection";
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
  backgroundColor: "#C01E2E", // Changed to site's main red
  color: "#ffffff",
  zIndex: 2,
  borderRadius: theme.shape.borderRadius,
  transition: "all 0.3s ease-out",
}));

const StyledAccordion = styled(Accordion, {
  shouldForwardProp: (prop) => prop !== "hasRequests" && prop !== "isBlinking",
})(({ theme, hasRequests, isBlinking }) => ({
  marginBottom: 0,
  boxShadow: theme.shadows[2],
  borderRadius: `${theme.shape.borderRadius}px !important`,
  position: "relative",
  zIndex: 1,
  "&:before": {
    display: "none",
  },
  "& .MuiAccordionSummary-content": {
    margin: "12px 0",
  },
  "& .MuiAccordionDetails-root": {
    padding: theme.spacing(2),
  },
  backgroundColor: hasRequests ? "rgba(255, 220, 100, 0.15)" : "#ffffff",
  animation:
    isBlinking && hasRequests ? "blink 1s ease-in-out infinite" : "none",
  transition: "transform 0.3s ease-out, opacity 0.3s ease-out",
  "@keyframes blink": {
    "0%": { backgroundColor: "rgba(255, 220, 100, 0.25)" },
    "50%": { backgroundColor: "rgba(255, 220, 100, 0.5)" },
    "100%": { backgroundColor: "rgba(255, 220, 100, 0.25)" },
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

const SessionCard = ({ session, onRequestUpdate, expanded, onExpand }) => {
  const [requests, setRequests] = useState([]);
  const [isBlinking, setIsBlinking] = useState(false);
  const [dragX, setDragX] = useState(0);
  const [showConfirm, setShowConfirm] = useState(false);
  const [isEnding, setIsEnding] = useState(false);
  const [error, setError] = useState(null);
  const touchStartX = useRef(null);
  const { endSession, actionError, clearActionError } = useSession();

  useEffect(() => {
    if (actionError) {
      // Optionally show error in a snackbar or other UI element
      setShowConfirm(false);
      setDragX(0);
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
    }
  }, [session.session_Id, onRequestUpdate]);

  useEffect(() => {
    if (showConfirm && expanded) {
      onExpand(false);
    }
  }, [showConfirm, expanded, onExpand]);

  const handleAccordionChange = (e, isExpanded) => {
    if (!showConfirm) {
      onExpand(isExpanded);
    }
  };

  useEffect(() => {
    if (showConfirm && expanded) {
      onExpand(false);
    }
  }, [showConfirm, expanded, onExpand]);

  const handleInitiateEnd = useCallback(
    (e) => {
      if (e) {
        e.preventDefault();
        e.stopPropagation();
      }
      onExpand(false);
      setShowConfirm(true);
    },
    [onExpand]
  );

  const handleComplete = async (requestId) => {
    try {
      // previously used /service-requests/{requestId}/complete
      // TODO: Endpoint to mark a service request as complete
      const response = await api.post(
        `/service-requests/${requestId}/complete`
      );

      if (response.status !== 200)
        throw new Error("Failed to complete request");
      await fetchRequests();
    } catch (error) {
      console.error("Error completing request:", error);
    }
  };

  const handleCancelEnd = (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    setShowConfirm(false);
    setDragX(0);
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
      clearActionError(); // Clear any previous errors
      const success = await endSession(session.session_Id);
      if (!success) {
      }
    } catch (error) {
      console.error("Error ending session:", error);
    } finally {
      setIsEnding(false);
    }
  };

  const handleTouchStart = useCallback(
    (e) => {
      if (!session.is_closable || isEnding || showConfirm) return;
      touchStartX.current = e.touches[0].clientX;
    },
    [session.is_closable, isEnding, showConfirm]
  );

  const handleTouchMove = useCallback(
    (e) => {
      if (
        !touchStartX.current ||
        !session.is_closable ||
        isEnding ||
        showConfirm
      )
        return;
      const currentX = e.touches[0].clientX;
      const diff = touchStartX.current - currentX;
      const newDragX = Math.min(Math.max(-diff, -150), 0);
      setDragX(newDragX);
    },
    [session.is_closable, isEnding, showConfirm]
  );

  const handleTouchEnd = useCallback(() => {
    if (!session.is_closable || isEnding || showConfirm) return;
    if (dragX <= -100) {
      setShowConfirm(true);
    }
    setDragX(0);
    touchStartX.current = null;
  }, [dragX, session.is_closable, isEnding, showConfirm]);

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
              gap: 2, // Adds space between elements
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

      <StyledAccordion
        expanded={expanded && !showConfirm}
        onChange={handleAccordionChange}
        onTouchStart={handleTouchStart}
        onTouchMove={handleTouchMove}
        onTouchEnd={handleTouchEnd}
        hasRequests={requests.length > 0}
        isBlinking={isBlinking}
        sx={{
          transform: `translateX(${dragX}px)`,
          opacity: showConfirm ? 0.5 : 1,
          pointerEvents: showConfirm || isEnding ? "none" : "auto",
          transition: "all 0.3s ease-out",
        }}
      >
        <AccordionSummary
          expandIcon={<ChevronDown />}
          aria-controls={`session-${session.session_Id}-content`}
          id={`session-${session.session_Id}-header`}
        >
          <Box sx={{ width: "100%" }}>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                mb: 1,
              }}
            >
              <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <Typography variant="h6">
                  Session #{session.session_Id}
                </Typography>
                {requests.length > 0 && (
                  <Chip
                    size="small"
                    color="warning"
                    label={requests.length}
                    icon={<Bell size={16} />}
                  />
                )}
              </Box>
              <Chip
                size="small"
                label={session.is_closable ? "Ready to Close" : "Active"}
                color={session.is_closable ? "success" : "primary"}
              />
            </Box>

            {requests.map((request) => (
              <ServiceRequest
                key={request.request_id}
                request={request}
                onComplete={handleComplete}
              />
            ))}
          </Box>
        </AccordionSummary>

        <AccordionDetails>
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
                // disabled={!session.is_closable || isEnding}
                onClick={handleInitiateEnd}
                startIcon={<AlertCircle />}
              >
                End Session
              </EndSessionButton>
            </Box>
          </Stack>
        </AccordionDetails>
      </StyledAccordion>
    </CardWrapper>
  );
};

export default SessionCard;
