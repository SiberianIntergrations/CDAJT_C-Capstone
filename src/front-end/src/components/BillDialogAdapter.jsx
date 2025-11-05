import { useEffect, useState } from "react";
import NewBillDialog from "./staff/SessionDashboard/components/dialogs/NewBillDialog";
import { SessionProvider } from "./staff/SessionDashboard/context/SessionContext";
import api from "@/config/api";

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