import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import api from "@/config/api";


const BrowsingBehavior = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const router = useRouter();

  useEffect(() => {
    const fetchData = async () => {
      const accessToken = localStorage.getItem("access_token");


      if (!accessToken) {
        setError("No token found");
        router.push("/auth/login");
        return;
      }

      try {
        const response = await api.get("analytics/browsing-behavior")
        
        setData(response.data);
        console.log(response.data)
      } 
      catch (err) {
        console.error("Error fetching data:", err);
        if (err.response && err.response.status === 401) {
          console.log(err.response)
        } 
      }
      finally {
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
