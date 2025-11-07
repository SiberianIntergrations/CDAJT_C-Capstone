import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import Alert from "@mui/material"
import api from "@/config/api";

const BrowsingBehavior = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const router = useRouter();

  useEffect(() => {
    const fetchData = async () => {
      // const accessToken = localStorage.getItem("access_token");

      // if (!accessToken) {
      //   setError("No token found");
      //   router.push("/auth/login");
      //   return;
      // }

      try {
        const response = await api.get("analytics/browsing-behavior");

        setData(response.data);
      } catch (err) {
      if (err.response) {
        const { status, data } = err.response;
        console.error("Error fetching data:", err);
        console.log("Status", status)
        console.log("Data", err.response.data)
        
        switch (status) {
          case 401:
            setError("Unauthorized. Please log in again.");
            router.push("/auth/login");
            break;
          case 404:
            setError(err.response.data || "Resource not found");
            break;
          case 500:
            setError(err.response.data || "Server error occurred");
            break;
          default:
            setError(err.response.data || "An error occurred");
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
