import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import { Alert } from "@mui/material";
import api from "@/config/api";

const TableTurnover = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [partySize, setPartySize] = useState(2);
  const [maxPartySize, setMaxPartySize] = useState(4);
  const router = useRouter();

  useEffect(() => {
    const fetchMaxPartySize = async () => {
      try {
        const response = await api.get("analytics/max-party-size");
        setMaxPartySize(response.data);
      } catch (err) {
        console.error("Error fetching max party size:", err);
        setError("Failed to fetch max party size");
      }
    };

    fetchMaxPartySize();
  }, []);

useEffect(() => {
  const fetchData = async () => {
    try {
      const response = await api.get("analytics/table-turnover");
      setData(response.data);
    } catch (err) {
      console.error("Error fetching data:", err);
      if (err.response) {
        const { status, data } = err.response;
        console.log("Status", status)
        console.log("Data", err.response.data)
        
        switch (status) {
          case 401:
            setError("Unauthorized. Please log in again.");
            router.push("/auth/login");
            break;
          case 409:
            setError(err.response.data.message || "Conflict error occurred");
            console.log("Conflict message:", err.response.data);
            break;
          case 404:
            setError(err.response.data.message || "Resource not found");
            break;
          case 500:
            setError(err.response.data.message || "Server error occurred");
            break;
          default:
            setError(err.response.data.message || "An error occurred");
        }
      }
    } finally {
      setLoading(false);
    }
  };

  fetchData();
}, [router]);

  if (loading) return <p>Loading...</p>;
if (error) return (
  <Alert
    severity="error"
    sx={{
      mb: 2,
      borderRadius: 2,
      textAlign: "center",
      justifyContent: "center",
      alignItems: "center",
    }}
  >
      {error}
  </Alert>
);

  if (!data) return <p>No data available</p>; // ADDED: Check if data exists

  return (
    <div>
      <h1>Table Turnover</h1>

      <label>
        Party Size:
        <input
          type="number"
          value={partySize}
          min="1"
          max={maxPartySize}
          onChange={(e) => {
            const value = Number(e.target.value);
            if (value >= 1 && value <= maxPartySize) {
              setPartySize(value);
            }
          }}
        />
      </label>

      <h2>Monthly Average Durations</h2>
      <table>
        <thead>
          <tr>
            <th>Month</th>
            <th>Party Size</th>
            <th>Average Duration (minutes)</th>
          </tr>
        </thead>
        <tbody>
          {data.monthly && data.monthly.length > 0 ? ( // ADDED: Check if monthly exists and has data
            data.monthly
              .filter((entry) => entry.party_size === partySize)
              .map((entry, index) => (
                <tr key={index}>
                  <td>{entry.month}</td>
                  <td>{entry.party_size}</td>
                  <td>
                    {entry.averageDuration < 0
                      ? "Invalid data"
                      : entry.averageDuration}
                  </td>
                </tr>
              ))
          ) : (
            <tr>
              <td colSpan="3">No monthly data available</td>
            </tr>
          )}
        </tbody>
      </table>

      <h2>Daily Average Durations</h2>
      <table>
        <thead>
          <tr>
            <th>Day</th>
            <th>Party Size</th>
            <th>Average Duration (minutes)</th>
          </tr>
        </thead>
        <tbody>
          {data.daily && data.daily.length > 0 ? ( // ADDED: Check if daily exists and has data
            data.daily
              .filter((entry) => entry.party_size === partySize)
              .map((entry, index) => (
                <tr key={index}>
                  <td>{entry.day}</td>
                  <td>{entry.party_size}</td>
                  <td>
                    {entry.averageDuration < 0
                      ? "no entry in end_time"
                      : entry.averageDuration}
                  </td>
                </tr>
              ))
          ) : (
            <tr>
              <td colSpan="3">No daily data available</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default TableTurnover;
