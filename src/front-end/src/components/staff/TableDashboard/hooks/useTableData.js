import { useState, useCallback } from "react";
import api from "@/config/api";
import storage from "@/utils/storage";

export const useTableData = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [tables, setTables] = useState([]);
  const [tableGroups, setTableGroups] = useState([]);
  const [tableSummary, setTableSummary] = useState({
    total_tables: 0,
    active_tables: 0,
    tables_in_use: 0,
    available_tables: 0,
  });

  const fetchTables = useCallback(async () => {
    try {
      const response = await api.get(
        `/TableEntity?locationId=${storage.get("branch-location")}`
      );
      if (response.status === 200) {
        setTables(response.data);

        return response.data;
      }
    } catch (err) {
      setError("Failed to fetch tables");
      console.error("Error fetching tables:", err);
      return [];
    }
  }, []);

  const fetchTableGroups = useCallback(async () => {
    try {
      const response = await api.get(
        `/TableGroup?locationId=${storage.get("branch-location")}`
      );
      if (response.status === 200) {
        setTableGroups(response.data);
        // console.log("Table Group Data", response.data);
        return response.data;
      }
    } catch (err) {
      setError("Failed to fetch table groups");
      console.error("Error fetching table groups:", err);
      return [];
    }
  }, []);

  const fetchTableSummary = useCallback(async () => {
    try {
      const [tablesData, emptyTablesData] = await Promise.all([
        api.get("/TableEntity"),
        api.get("/TableEntity/empty"),
      ]);

      const allTables = tablesData.data || [];
      const emptyTables = emptyTablesData.data || [];

      setTableSummary({
        total_tables: allTables.length,
        active_tables: allTables.filter((t) => t.is_active).length,
        tables_in_use: allTables.length - emptyTables.length,
        available_tables: emptyTables.length,
      });
    } catch (err) {
      console.error("Error fetching table summary:", err);
    }
  }, []);

  const fetchAllData = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      await Promise.all([
        fetchTables(),
        fetchTableGroups(),
        fetchTableSummary(),
      ]);
    } catch (err) {
      setError("Failed to load table data");
    } finally {
      setIsLoading(false);
    }
  }, [fetchTables, fetchTableGroups, fetchTableSummary]);

  return {
    isLoading,
    error,
    setError,
    tables,
    tableGroups,
    tableSummary,
    fetchTables,
    fetchTableGroups,
    fetchTableSummary,
    fetchAllData,
  };
};
