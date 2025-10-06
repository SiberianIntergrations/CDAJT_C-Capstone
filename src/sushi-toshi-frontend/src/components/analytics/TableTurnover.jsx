import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";

const TableTurnover = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [partySize, setPartySize] = useState(2);
  const [maxPartySize, setMaxPartySize] = useState(null); // Add state for max party size
  const router = useRouter();

  useEffect(() => {
    const fetchMaxPartySize = async () => {
      try {
        const response = await axios.get(
          "http://127.0.0.1:8000/analytics/max-party-size",
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("access_token")}`,
            },
          }
        );
        setMaxPartySize(response.data.max_party_size);
      } catch (err) {
        console.error("Error fetching max party size:", err);
        setError("Failed to fetch max party size");
      }
    };

    fetchMaxPartySize();
  }, []);

  useEffect(() => {
    const fetchData = async () => {
      const accessToken = localStorage.getItem("access_token");
      const refreshToken = localStorage.getItem("refresh_token");
      console.log("Access Token:", accessToken);
      console.log("Refresh Token:", refreshToken);

      if (!accessToken || !refreshToken) {
        setError("No token found");
        router.push("/auth/login");
        return;
      }

      try {
        const response = await axios.get(
          `http://127.0.0.1:8000/analytics/table-turnover?party_size=${partySize}`,
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );
        setData(response.data);
      } catch (err) {
        console.error("Error fetching data:", err);
        if (err.response && err.response.status === 401) {
          try {
            const tokenResponse = await axios.post(
              "http://127.0.0.1:8000/auth/refresh",
              new URLSearchParams({
                refresh_token: refreshToken,
                grant_type: "refresh_token",
              }),
              {
                headers: {
                  "Content-Type": "application/x-www-form-urlencoded",
                  accept: "application/json",
                },
              }
            );

            const { access_token } = tokenResponse.data;
            localStorage.setItem("access_token", access_token);
            fetchData();
          } catch (refreshErr) {
            console.error("Error refreshing token:", refreshErr);
            setError("Session expired, please log in again");
            router.push("/auth/login");
          }
        } else {
          setError("Failed to fetch data");
        }
      } finally {
        setLoading(false);
      }
    };

    if (maxPartySize !== null) {
      fetchData();
    }
  }, [router, partySize, maxPartySize]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;

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
          {data.monthly &&
            data.monthly
              .filter((entry) => entry.party_size === partySize)
              .map((entry, index) => (
                <tr key={index}>
                  <td>{entry.month}</td>
                  <td>{entry.party_size}</td>
                  <td>
                    {entry.average_duration < 0
                      ? "Invalid data"
                      : entry.average_duration}
                  </td>
                </tr>
              ))}
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
          {data.daily &&
            data.daily
              .filter((entry) => entry.party_size === partySize)
              .map((entry, index) => (
                <tr key={index}>
                  <td>{entry.day}</td>
                  <td>{entry.party_size}</td>
                  <td>
                    {entry.average_duration < 0
                      ? "no entry in end_time "
                      : entry.average_duration}
                  </td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  );
};

export default TableTurnover;
