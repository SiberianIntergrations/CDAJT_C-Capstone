import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
// import { Line } from "react-charts-2";
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


      useEffect(() => {
        const fetchData = async () => {
          const accessToken = localStorage.getItem("access_token");
    
    
          if (!accessToken) {
            setError("No token found");
            router.push("/auth/login");
            return;
          }
    
          try {
            const response = await api.get("analytics/order-timing")
            
            setData(response.data);
            setDailyAverageTiming(response.data.dailyAverageTiming)
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
      (sum, item) => sum + item.averageTimeToFirstOrderSeconds,
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
                (item.averageTimeToFirstOrderSeconds / 60).toFixed(2)
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
                  <td>{(item.averageTimeToFirstOrderSeconds /60 ).toFixed(2)}</td>
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
            data.orderTiming.map((item, index) => (
              <tr key={index}>
                <td>{item.sessionId}</td>
                <td>{(item.timeToFirstOrderSeconds / 60).toFixed(2)}</td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
};

export default OrderTiming;
