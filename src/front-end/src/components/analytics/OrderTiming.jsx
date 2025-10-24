import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import { Line } from "react-chartjs-2";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import styles from "@/components/analytics/OrderTiming.module.css"; // Import CSS module

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);
import api from "@/config/api";

const OrderTiming = () => {
  const [data, setData] = useState(null);
  const [dailyAverageTiming, setDailyAverageTiming] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const router = useRouter();

  // useEffect(() => {
  //   const fetchData = async () => {
  //     const accessToken = localStorage.getItem("access_token");
  //     const refreshToken = localStorage.getItem("refresh_token");
  //     console.log("Access Token:", accessToken);
  //     console.log("Refresh Token:", refreshToken);

  //     if (!accessToken || !refreshToken) {
  //       setError("No token found");
  //       router.push("/auth/login");
  //       return;
  //     }

  //     try {
  //       const response = await axios.get(
  //         "http://127.0.0.1:8000/analytics/order-timing",
  //         {
  //           headers: {
  //             Authorization: `Bearer ${accessToken}`,
  //             accept: "application/json",
  //           },
  //         }
  //       );
  //       console.log("API Response:", response.data);
  //       setData(response.data.order_timing);
  //       setDailyAverageTiming(response.data.daily_average_timing);
  //       console.log(
  //         "Daily Average Timing:",
  //         response.data.daily_average_timing
  //       );
  //     } catch (err) {
  //       console.error("Error fetching data:", err);
  //       if (err.response && err.response.status === 401) {
  //         try {
  //           const tokenResponse = await axios.post(
  //             "http://127.0.0.1:8000/auth/refresh",
  //             new URLSearchParams({
  //               refresh_token: refreshToken,
  //               grant_type: "refresh_token",
  //             }),
  //             {
  //               headers: {
  //                 "Content-Type": "application/x-www-form-urlencoded",
  //                 accept: "application/json",
  //               },
  //             }
  //           );

  //           const { access_token } = tokenResponse.data;
  //           localStorage.setItem("access_token", access_token);
  //           fetchData();
  //         } catch (refreshErr) {
  //           console.error("Error refreshing token:", refreshErr);
  //           setError("Session expired, please log in again");
  //           router.push("/auth/login");
  //         }
  //       } else {
  //         setError("Failed to fetch data");
  //       }
  //     } finally {
  //       setLoading(false);
  //     }
  //   };

  //   fetchData();
  // }, [router]);

      useEffect(() => {
        const fetchData = async () => {
          const accessToken = localStorage.getItem("access_token");
    
    
          if (!accessToken) {
            setError("No token found");
            router.push("/auth/login");
            return;
          }
    
          try {
            const response = await api.get("analytics/item-performance")
            
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
  

  useEffect(() => {
    console.log("Daily Average Timing State:", dailyAverageTiming);
  }, [dailyAverageTiming]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;

  const calculateAverage = (days) => {
    if (!dailyAverageTiming) return null;
    const filteredData = dailyAverageTiming.slice(-days);
    const total = filteredData.reduce(
      (sum, item) => sum + item.average_time_to_first_order,
      0
    );
    return (total / filteredData.length / 60).toFixed(2);
  };

  const ChartComponent = () => {
    const chartData = {
      labels: dailyAverageTiming
        ? dailyAverageTiming.map((item) => item.day)
        : [],
      datasets: [
        {
          label: "Average Time to First Order (minutes)",
          data: dailyAverageTiming
            ? dailyAverageTiming.map((item) =>
                (item.average_time_to_first_order / 60).toFixed(2)
              )
            : [],
          fill: false,
          borderColor: "rgb(75, 192, 192)",
          tension: 0.1,
        },
      ],
    };

    return (
      <div className={styles.chartContainer}>
        <Line data={chartData} />
      </div>
    );
  };

  return (
    <div className={styles.container}>
      {" "}
      {/* Apply CSS class */}
      <h1>Order Timing</h1>
      {Array.isArray(dailyAverageTiming) && dailyAverageTiming.length > 0 && (
        <div>
          <p>Average last 7 days: {calculateAverage(7)} Minutes</p>
          <p>Average last 14 days: {calculateAverage(14)} Minutes</p>
          <p>Average last 30 days: {calculateAverage(30)} Minutes</p>
        </div>
      )}
      <div>
        <ChartComponent />
      </div>
      {Array.isArray(dailyAverageTiming) && dailyAverageTiming.length > 0 ? (
        <div>
          <h2>Daily Average Timing</h2>
          <table>
            <thead>
              <tr>
                <th>Day</th>
                <th>Average Time to First Order (minutes)</th>
              </tr>
            </thead>
            <tbody>
              {dailyAverageTiming.map((item, index) => (
                <tr key={index}>
                  <td>{item.day}</td>
                  <td>{(item.average_time_to_first_order / 60).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p>No daily average timing data available.</p>
      )}
      <table>
        <thead>
          <tr>
            <th>Session ID</th>
            <th>Time to First Order (minutes)</th>
          </tr>
        </thead>
        <tbody>
          {data &&
            data.map((item, index) => (
              <tr key={index}>
                <td>{item.session_id}</td>
                <td>{(item.time_to_first_order / 60).toFixed(2)}</td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
};

export default OrderTiming;
