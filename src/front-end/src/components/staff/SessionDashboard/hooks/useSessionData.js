import { useState, useCallback, useEffect } from "react";
import api from "@/config/api";

export const useSessionData = (updateTrigger = 0) => {
  const [sessions, setSessions] = useState([]);
  const [dashboardSummary, setDashboardSummary] = useState({
    total_active_sessions: 0,
    total_tables_in_use: 0,
    total_active_bills: 0,
    total_active_participants: 0,
  });
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchSessions = useCallback(async () => {
    try {
      const response = await api.get("/Dashboard/sessions");

      if (response.status !== 200)
        throw new Error("Failed to fetch sessions");
      const data = response.data;
      setSessions(data);
    } catch (err) {
      setError("Failed to load sessions");
      console.error(err);
    }
  }, []);

  const fetchDashboardSummary = useCallback(async () => {
    try {
      const response = await api.get("/Dashboard/summary");
      if (response.status !== 200)
        throw new Error("Failed to fetch summary");
      const data = response.data;
      setDashboardSummary(data);
    } catch (err) {
      console.error(err);
    }
  }, []);

  useEffect(() => {
    const loadData = async () => {
      setIsLoading(true);
      await Promise.all([fetchSessions(), fetchDashboardSummary()]);
      setIsLoading(false);
    };

    loadData();
  }, [fetchSessions, fetchDashboardSummary, updateTrigger]);

  return {
    sessions,
    dashboardSummary,
    isLoading,
    error,
    fetchSessions,
    fetchDashboardSummary,
  };
};
