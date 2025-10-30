import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import {
  Grid,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import InsightTextBox from "@/components/analytics/insight";
import api from "@/config/api";

const ItemPerformance = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const router = useRouter();
  const [sortBy, setSortBy] = useState("name_asc");

  useEffect(() => {
    const fetchData = async () => {
      const accessToken = localStorage.getItem("access_token");

      if (!accessToken) {
        setError("No token found");
        router.push("/auth/login");
        return;
      }

      try {
        const response = await api.get("analytics/item-performance");

        setData(response.data);
      } catch (err) {
        console.error("Error fetching data:", err);
        if (err.response && err.response.status === 401) {
        }
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [router]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;

  const top5Items = data.item_performance

    .sort((a, b) => b.totalUnitsSold - a.totalUnitsSold)
    .slice(0, 5);

  const least5Items = data.item_performance
    .sort((b, a) => b.totalUnitsSold - a.totalUnitsSold)
    .slice(0, 5);

  return (
    <div>
      <Grid container spacing={2}>
        <Grid item xs={6}>
          <Typography variant="h6">Top 5 Items</Typography>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell style={{ color: "white" }}>Item Name</TableCell>
                  <TableCell style={{ color: "white" }}>
                    Total Units Sold
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {top5Items.map((item) => (
                  <TableRow key={item.item_id}>
                    <TableCell>{item.name}</TableCell>
                    <TableCell>{item.totalUnitsSold}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Grid>
        <Grid item xs={6}>
          <Typography variant="h6">Bottom 5 Items</Typography>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell style={{ color: "white" }}>Item Name</TableCell>
                  <TableCell style={{ color: "white" }}>
                    Total Units Sold
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {least5Items.map((item) => (
                  <TableRow key={item.item_id}>
                    <TableCell>{item.name}</TableCell>
                    <TableCell>{item.totalUnitsSold}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Grid>
      </Grid>

      <InsightTextBox
        topItems={top5Items}
        bottomItems={least5Items}
        question="This is a buffet japanese sushi restaurant, based on top 5 and bottom 5 items, Is there anything the restaurant can improve? maybe a SWOT analysis"
      />

      <table>
        <thead>
          <tr>
            <th>Item ID</th>
            <th>
              Item Name
              <button onClick={() => setSortBy("name_asc")}>Sort A-Z</button>
              <button onClick={() => setSortBy("name_desc")}>Sort Z-A</button>
            </th>
            <th>
              Total Units Sold
              <button onClick={() => setSortBy("units_asc")}>
                Sort by Least
              </button>
              <button onClick={() => setSortBy("units_desc")}>
                Sort by Most
              </button>
            </th>
          </tr>
        </thead>
        <tbody>
          {data &&
            data.item_performance
              .sort((a, b) => {
                if (sortBy === "name_asc") return a.name.localeCompare(b.name);
                if (sortBy === "name_desc") return b.name.localeCompare(a.name);
                if (sortBy === "units_asc")
                  return a.totalUnitsSold - b.totalUnitsSold;
                if (sortBy === "units_desc")
                  return b.totalUnitsSold - a.totalUnitsSold;
                return 0;
              })
              .map((item, index) => (
                <tr key={index}>
                  <td>{item.item_id}</td>
                  <td>{item.name}</td>
                  <td>{item.totalUnitsSold}</td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  );
};

export default ItemPerformance;
