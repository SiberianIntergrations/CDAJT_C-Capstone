import { AccordionSummary, Box, Typography, Chip } from "@mui/material";
import { ChevronDown } from "lucide-react";

const SessionHeader = ({ session }) => (
  <AccordionSummary expandIcon={<ChevronDown />}>
    <Box className="flex justify-between items-center w-full mr-4">
      <Typography variant="h6">Session #{session.session_id}</Typography>
      <Chip
        size="small"
        label={session.is_closable ? "Ready to Close" : "Active"}
        color={session.is_closable ? "success" : "primary"}
      />
    </Box>
  </AccordionSummary>
);

export default SessionHeader;
