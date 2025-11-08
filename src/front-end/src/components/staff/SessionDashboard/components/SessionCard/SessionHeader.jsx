import React, { useMemo } from "react";
import { Box, Typography, Chip } from "@mui/material";
import { Bell } from "lucide-react";

/**
 * Displays header information for a dining session card including:
 * - Session ID
 * - Active/Closed status
 * - Table assignments
 * - Location
 */
const SessionHeader = ({ session, requests = [] }) => {
  // Calculate session status
  const isActive = useMemo(() => {
    return !session?.ended_At;
  }, [session?.ended_At]);

  // Format timestamp for display
  const formatTime = (timestamp) => {
    if (!timestamp) return "N/A";
    return new Date(timestamp).toLocaleTimeString([], { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  };

  // Get table numbers display
  const tableDisplay = useMemo(() => {
    if (session?.table_Numbers) {
      // If it's an array, join them
      if (Array.isArray(session.table_Numbers)) {
        return session.table_Numbers.join(", ");
      }
      return session.table_Numbers;
    }
    return "N/A";
  }, [session?.table_Numbers]);

  return (
    <Box>
      {/* Top Row: Session ID, Tables, Requests and Status */}
      <Box sx={{ 
        display: "flex", 
        justifyContent: "space-between", 
        alignItems: "center", 
        mb: 0.5 
      }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
          <Typography variant="h6">
            Session #{session?.session_Id}
          </Typography>
          
          <Typography variant="body2" color="text.secondary">
            Tables: {tableDisplay}
          </Typography>
        </Box>
        
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          {/* Service Requests Notification Bell */}
          {requests.length > 0 && (
            <Chip
              icon={<Bell size={14} />}
              label={requests.length}
              size="small"
              color="warning"
              sx={{
                animation: "pulse 2s ease-in-out infinite",
                "@keyframes pulse": {
                  "0%, 100%": { opacity: 1 },
                  "50%": { opacity: 0.7 },
                },
              }}
            />
          )}
          
          {/* Status Chip */}
          <Chip
            label={isActive ? "Active" : "Closed"}
            size="small"
            color={isActive ? "success" : "default"}
            variant={isActive ? "filled" : "outlined"}
          />
        </Box>
      </Box>
      {/* Location Row */}
      <Box sx={{ display: "flex"}}>
        <Typography variant="body2" color="text.secondary">
          {session?.location_Name}
        </Typography>
      </Box>
    </Box>
  );
};

export default SessionHeader;