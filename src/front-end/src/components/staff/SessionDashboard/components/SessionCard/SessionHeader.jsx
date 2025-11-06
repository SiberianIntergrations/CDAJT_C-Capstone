import React, { useMemo } from "react";
import { Box, Typography, Chip } from "@mui/material";
import { Clock, Users, Receipt, Bell } from "lucide-react";

/**
 * 
 * Displays header information for a dining session card including:
 * - Session ID
 * - Duration with live timer
 * - Active/Closed status
 * - Start/End timestamps
 * - Table assignments
 */
const SessionHeader = ({ session, requests = [] }) => {
  // Calculate session duration in real-time
  const sessionInfo = useMemo(() => {
    const startTime = session?.started_At
    const endTime = session?.ended_At

    if (!startTime) {
      return {
        duration: "0 min",
        minutes: 0,
        isActive: true,
      };
    }

    const start = new Date(startTime);
    const end = endTime ? new Date(endTime) : new Date();
    const totalMinutes = Math.floor((end - start) / 60000);
    
    let durationText;
    if (totalMinutes < 60) {
      durationText = `${totalMinutes} min`;
    } else {
      const hours = Math.floor(totalMinutes / 60);
      const mins = totalMinutes % 60;
      durationText = `${hours}h ${mins}m`;
    }

    return {
      duration: durationText,
      minutes: totalMinutes,
      isActive: !endTime,
    };
  }, [session?.started_At, session?.ended_At]);

  // Determine duration chip color based on AYCE time limits
  const getDurationColor = () => {
    if (!sessionInfo.isActive) return "default";
    if (sessionInfo.minutes >= 120) return "error"; // Exceeded 2hr limit
    if (sessionInfo.minutes >= 100) return "warning"; // Approaching limit
    return "primary";
  };

  // Format timestamp for display
  const formatTime = (timestamp) => {
    if (!timestamp) return "N/A";
    return new Date(timestamp).toLocaleTimeString();
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
      {/* Top Row: Session ID and Status Chips */}
      <Box sx={{ 
        display: "flex", 
        justifyContent: "space-between", 
        alignItems: "center", 
        mb: 1.5 
      }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
          <Typography variant="h6">
            Session #{session?.session_Id}
          </Typography>
          
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
        </Box>
        
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          {/* Duration Chip */}
          <Chip
            icon={<Clock size={14} />}
            label={sessionInfo.duration}
            size="small"
            color={getDurationColor()}
            variant={sessionInfo.isActive ? "filled" : "outlined"}
          />
          
          {/* Status Chip */}
          <Chip
            label={sessionInfo.isActive ? "Active" : "Closed"}
            size="small"
            color={sessionInfo.isActive ? "success" : "default"}
            variant={sessionInfo.isActive ? "filled" : "outlined"}
          />
        </Box>
      </Box>

      {/* Location Row */}
      <Box sx={{ mb: 1 }}>
        <Typography variant="body2" color="text.secondary">{session?.location_Name}
        </Typography>
      </Box>

      {/* Timestamp Row */}
      <Box sx={{ mb: 1 }}>
        <Typography variant="body2" color="text.secondary">
          <Box component="span" sx={{ fontWeight: 500 }}>Started:</Box> {formatTime(session?.started_At)}
          {session?.ended_At && (
            <>
              {" • "}
              <Box component="span" sx={{ fontWeight: 500 }}>Ended:</Box> {formatTime(session.ended_At)}
            </>
          )}
        </Typography>
      </Box>

      {/* Info Row: Tables*/}
      <Box sx={{ 
        display: "flex", 
        gap: 2, 
        flexWrap: "wrap",
        alignItems: "center" 
      }}>
        {/* Tables */}
        <Typography variant="body2" color="text.secondary">
          <Box component="span" sx={{ fontWeight: 500 }}>Tables:</Box> {tableDisplay}
        </Typography>
      </Box>
    </Box>
  );
};

export default SessionHeader;