import { useState, useCallback, useEffect } from "react";
import api from "@/config/api";
import publicApi from "@/config/publicApi";
import storage from "@/utils/storage";

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
      const storedLocationId = storage.get("branch-location");
      if (!storedLocationId) {
        const isGuest = localStorage.getItem("guest") === "true";
        if(! isGuest){
          setError(
            "Invalid location. Make sure you have selected a location in the navigation bar dropdown at the top of the page."
          );
          console.error(
            "Invalid location. Make sure you have selected a location in the navigation bar dropdown at the top of the page."
          );
      }

        return [];
      }
      console.log(storedLocationId)
      const response = await api.get(
        `/Dashboard/sessions?locationId=${storedLocationId}`
      );

      if (response.status === 204) {
        setError("No active dining sessions found.");
        throw new Error("No active dining sessions found.");
      }

      if (response.status !== 200) {
        setError("Failed to fetch sessions");
        throw new Error("Failed to fetch sessions");
      }
      const data = response.data;
      setSessions(data);
    } catch (err) {
      console.error(err);
      console.log(err);
    }
  }, []);

  const fetchDashboardSummary = useCallback(async () => {
    try {
      const response = await api.get("/Dashboard/summary");
      if (response.status !== 200) throw new Error("Failed to fetch summary");
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
