import { useEffect, useState } from "react";
import NewBillDialog from "./staff/SessionDashboard/components/dialogs/NewBillDialog";
import { SessionProvider } from "./staff/SessionDashboard/context/SessionContext";
import api from "@/config/api";

const SessionContextWrapper = ({ children }) => {
  const [localSessionId, setLocalSessionId] = useState(null);

  const createBill = async (sessionId, billData) => {
    console.log(
      "Creating bill with sessionId:",
      sessionId,
      "and data:",
      billData
    );
    try {
      const response = await api.post(
        `/sessions/${sessionId}/bills`,
        {
          bill_name: billData.billName,
          adult_count: billData.adultCount,
          child_count: billData.childCount,
          senior_count: billData.seniorCount,
          tot_count: billData.totCount,
        }
      );
      return response.status === 200 || response.status === 201;
    } catch (error) {
      console.error("Error creating bill:", error);
      return false;
    }
  };

  return (
    <SessionProvider
      value={{
        createBill,
        dialogState: {
          newBill: false,
          addTable: false,
          currentSessionId: localSessionId,
        },
        openDialog: () => console.log("openDialog called"),
        closeDialog: () => console.log("closeDialog called"),
        addTable: async () => console.log("addTable called"),
        createSession: async () => console.log("createSession called"),
        closeBill: async () => console.log("closeBill called"),
        endSession: async () => console.log("endSession called"),
        refreshData: async () => console.log("refreshData called"),
        actionError: null,
        clearActionError: () => console.log("clearActionError called"),
      }}
    >
      {children}
    </SessionProvider>
  );
};

const BillDialog = ({ onBillCreated }) => {
  const [isDialogOpen, setIsDialogOpen] = useState(true);
  const [sessionId, setSessionId] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchActiveSession = async () => {
      try {
        const response = await api.get(
          "/dining-sessions/participants/active-session-id"
        );

        if (response.data && response.data.session_id) {
          setSessionId(response.data.session_id);
          setError(null);
        }
      } catch (err) {
        console.error("Error in fetchActiveSession:", err);
        if (err.response) {
          setError(err.response.data.detail || "Error fetching session");
        } else if (err.request) {
          setError("No response received from server");
        } else {
          setError("Error setting up request");
        }
      }
    };

    fetchActiveSession();
  }, []);

  const handleDialogClose = async (success) => {
    if (success && onBillCreated) {
      await onBillCreated();
    }
  };

  return (
    <NewBillDialog
      open={isDialogOpen}
      sessionId={sessionId}
      onClose={handleDialogClose}
    />
  );
};

export default function BillDialogAdapter({ onBillCreated }) {
  return (
    <SessionContextWrapper>
      <BillDialog onBillCreated={onBillCreated} />
    </SessionContextWrapper>
  );
}
