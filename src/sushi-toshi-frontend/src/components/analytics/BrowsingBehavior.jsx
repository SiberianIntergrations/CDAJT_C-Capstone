import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";

const BrowsingBehavior = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const router = useRouter();

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
          "http://127.0.0.1:8000/analytics/browsing-behavior",
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

    fetchData();
  }, [router]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div>
      <h1>Browsing Behavior</h1>

      <table>
        <thead>
          <tr>
            <th>Item ID</th>
            <th>Item Name</th>
            <th>Total View Seconds</th>
            <th>Total Views</th>
          </tr>
        </thead>
        <tbody>
          {data &&
            data.map((behavior, index) => (
              <tr key={index}>
                <td>{behavior.item_id}</td>
                <td>{behavior.name}</td>
                <td>{behavior.total_view_seconds}</td>
                <td>{behavior.total_views}</td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
};

export default BrowsingBehavior;
