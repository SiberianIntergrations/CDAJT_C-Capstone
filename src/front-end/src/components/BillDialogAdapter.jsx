import { useEffect, useState } from "react";
import NewBillDialog from "./staff/SessionDashboard/components/dialogs/NewBillDialog";
import { SessionProvider } from "./staff/SessionDashboard/context/SessionContext";
import api from "@/config/api";

const SessionContextWrapper = ({ children }) => {
  const [localSessionId, setLocalSessionId] = useState(null);

  const createBill = async (sessionId, billData) => {
    try {
      const response = await api.post(`/sessions/${sessionId}/bills`, {
        bill_name: billData.billName,
        adult_count: billData.adultCount,
        child_count: billData.childCount,
        senior_count: billData.seniorCount,
        tot_count: billData.totCount,
      });
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
  const [loading, setLoading] = useState(true);


  useEffect(() => {
    console.log("fetchActiveSession called")
    const fetchActiveSession = async () => {
      try {
        const response = await api.get("/DiningSession/participants/active-session-id");
        console.log("API RESPONSE" + response)

        if (response?.status === 200 && typeof response.data?.session_id === "number") {
          setSessionId(response.data.session_id);
          setError(null);
        } else {
          setError("No active session found");
        }
      } catch (err) {
        setError("Error fetching session");
      } finally {
        setLoading(false);
      }
    };

    fetchActiveSession();
  }, []);

    console.log("BillDialog render — sessionId =", sessionId);

  const handleDialogClose = async (success) => {
    if (success && onBillCreated) {
      await onBillCreated();
    }
  };

  if (loading) return null; 

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
    <SessionProvider>
      <BillDialog onBillCreated={onBillCreated} />
    </SessionProvider>
  );
}