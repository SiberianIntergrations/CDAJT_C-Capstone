import React, { useState, useRef, useCallback } from "react";
import { Box, Typography, Chip, Button } from "@mui/material";
import { styled } from "@mui/material/styles";
import { AlertCircle } from "lucide-react";

const CardWrapper = styled("div")(() => ({
  position: "relative",
  marginBottom: "8px",
  transition: "transform 0.3s ease-out",
  background: "none",
}));

const ConfirmCloseButton = styled(Box, {
  shouldForwardProp: (prop) => prop !== "showConfirm",
})(({ theme, showConfirm }) => ({
  position: "absolute",
  top: 0,
  left: 0,
  width: "100%",
  height: "100%",
  display: showConfirm ? "flex" : "none",
  alignItems: "center",
  justifyContent: "space-between",
  backgroundColor: "#C01E2E",
  color: theme.palette.error.contrastText,
  zIndex: 2,
  borderRadius: theme.shape.borderRadius,
  padding: theme.spacing(0, 3),
  transition: "all 0.3s ease-out",
}));

const BillContent = styled(Box)(({ theme }) => ({
  backgroundColor: "rgba(255, 255, 255, 0.5)",
  borderRadius: theme.shape.borderRadius,
  border: "1px solid",
  borderColor: theme.palette.grey[200],
  padding: theme.spacing(1.5),
}));

const SwipeableBillCard = ({ bill, onClose, disabled = false }) => {
  const [dragX, setDragX] = useState(0);
  const [showConfirm, setShowConfirm] = useState(false);
  const [isClosing, setIsClosing] = useState(false);
  const touchStartX = useRef(null);
  // console.log("Bill", bill);

  const handleTouchStart = useCallback(
    (e) => {
      if (disabled || bill.status !== "Open" || isClosing || showConfirm)
        return;
      touchStartX.current = e.touches[0].clientX;
    },
    [disabled, bill.status, isClosing, showConfirm]
  );

  const handleTouchMove = useCallback(
    (e) => {
      if (
        !touchStartX.current ||
        disabled ||
        bill.status !== "Open" ||
        isClosing ||
        showConfirm
      )
        return;
      const currentX = e.touches[0].clientX;
      const diff = touchStartX.current - currentX;
      const newDragX = Math.min(Math.max(-diff, -150), 0);
      setDragX(newDragX);
    },
    [disabled, bill.status, isClosing, showConfirm]
  );

  const handleTouchEnd = useCallback(() => {
    if (disabled || bill.status !== "Open" || isClosing || showConfirm) return;
    if (dragX <= -100) {
      setShowConfirm(true);
    }
    setDragX(0);
    touchStartX.current = null;
  }, [dragX, disabled, bill.status, isClosing, showConfirm]);

  const handleCancelClose = (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    setShowConfirm(false);
    setDragX(0);
    setIsClosing(false);
  };

  const handleConfirmClose = async (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }

    if (isClosing) return;

    try {
      setIsClosing(true);
      await onClose(bill.bill_Id);
    } catch (error) {
      console.error("Error closing bill:", error);
    } finally {
      setShowConfirm(false);
      setDragX(0);
      setIsClosing(false);
    }
  };

  const handleCloseClick = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setShowConfirm(true);
  };

  const formatGuestCount = () => {
    const counts = [];
    if (bill.adult_count > 0) counts.push(`${bill.adult_count} Adults`);
    if (bill.child_count > 0) counts.push(`${bill.child_count} Children`);
    if (bill.senior_count > 0) counts.push(`${bill.senior_count} Seniors`);
    if (bill.tot_count > 0) counts.push(`${bill.tot_count} Tots`);
    return counts.join(" • ");
  };

  return (
    <CardWrapper>
      {showConfirm && (
        <ConfirmCloseButton
          showConfirm={showConfirm}
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            gap: 2,
          }}
        >
          <Button
            variant="outlined"
            onClick={handleCancelClose}
            disabled={isClosing}
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
              flex: 1,
              color: "white",
            }}
          >
            Confirm Close
          </Typography>
          <Button
            variant="contained"
            onClick={handleConfirmClose}
            disabled={isClosing}
            sx={{
              bgcolor: "white",
              color: "#C01E2E",
              "&:hover": {
                bgcolor: "rgba(255, 255, 255, 0.9)",
              },
            }}
          >
            {isClosing ? "Closing..." : "Confirm"}
          </Button>
        </ConfirmCloseButton>
      )}

      <BillContent
        onTouchStart={handleTouchStart}
        onTouchMove={handleTouchMove}
        onTouchEnd={handleTouchEnd}
        sx={{
          transform: `translateX(${dragX}px)`,
          opacity: showConfirm ? 0.5 : 1,
          pointerEvents: showConfirm || isClosing ? "none" : "auto",
          transition: "all 0.3s ease-out",
        }}
      >
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "flex-start",
            width: "100%",
          }}
        >
          <Box
            sx={{
              display: "flex",
              flexDirection: "column",
              gap: 1,
            }}
          >
            <Typography variant="body1" fontWeight="medium">
              {bill.bill_Name}
            </Typography>

            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              <Chip
                label={bill.status}
                size="small"
                color={bill.status === "Open" ? "primary" : "default"}
                sx={{
                  height: "20px",
                  "& .MuiChip-label": {
                    px: 1,
                    fontSize: "0.75rem",
                  },
                }}
              />
              <Typography variant="caption" color="text.secondary">
                {formatGuestCount()}
              </Typography>
            </Box>
          </Box>

          {bill.status === "Open" && (
            <Button
              variant="contained"
              size="small"
              onClick={handleCloseClick}
              sx={{
                bgcolor: "#C01E2E",
                color: "white",
                "&:hover": {
                  bgcolor: "#A01725",
                },
              }}
            >
              Close
            </Button>
          )}
        </Box>
      </BillContent>
    </CardWrapper>
  );
};

export default SwipeableBillCard;
