// File: sushi-toshi-frontend/components/analytics/ItemPerformance.jsx
import { useEffect, useState } from 'react';
import axios from 'axios';
import { useRouter } from 'next/router';
import { Grid, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import InsightTextBox from './insight';
const ItemPerformance = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const router = useRouter();
  const [sortBy, setSortBy] = useState('name_asc');

  useEffect(() => {
    const fetchData = async () => {
      const accessToken = localStorage.getItem('access_token');
      const refreshToken = localStorage.getItem('refresh_token');
      console.log('Access Token:', accessToken);
      console.log('Refresh Token:', refreshToken);

      if (!accessToken || !refreshToken) {
        setError('No token found');
        router.push('/auth/login'); 
        return;
      }

      try {
        const response = await axios.get('http://127.0.0.1:8000/analytics/item-performance', {
          headers: {
            'Authorization': `Bearer ${accessToken}`
          }
        });
        setData(response.data);
      } catch (err) {
        console.error('Error fetching data:', err);
        if (err.response) {
          if (err.response.status === 401) {
            try {
              const tokenResponse = await axios.post('http://127.0.0.1:8000/auth/refresh', new URLSearchParams({
                refresh_token: refreshToken,
                grant_type: 'refresh_token'
              }), {
                headers: {
                  'Content-Type': 'application/x-www-form-urlencoded',
                  'accept': 'application/json'
                },
              });

              const { access_token } = tokenResponse.data;
              localStorage.setItem('access_token', access_token);
              fetchData(); 
            } catch (refreshErr) {
              console.error('Error refreshing token:', refreshErr);
              setError('Session expired, please log in again');
              router.push('/auth/login'); 
            }
          } else if (err.response.status === 404) {
            setError('Data not found');
          } else {
            setError('Failed to fetch data');
          }
        } else {
          setError('Failed to fetch data');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [router]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;

  const top5Items = data.items_performance
    .sort((a, b) => b.total_units_sold - a.total_units_sold)
    .slice(0, 5);

  const least5Items = data.items_performance
    .sort((a, b) => a.total_units_sold - b.total_units_sold)
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
                  <TableCell style={{ color: 'white' }}>Item Name</TableCell>
                  <TableCell style={{ color: 'white' }}>Total Units Sold</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {top5Items.map((item) => (
                  <TableRow key={item.item_id}>
                    <TableCell >{item.name}</TableCell>
                    <TableCell >{item.total_units_sold}</TableCell>
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
                  <TableCell style={{ color: 'white' }}>Item Name</TableCell>
                  <TableCell style={{ color: 'white' }}>Total Units Sold</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {least5Items.map((item) => (
                  <TableRow key={item.item_id}>
                    <TableCell >{item.name}</TableCell>
                    <TableCell >{item.total_units_sold}</TableCell>
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
              <button onClick={() => setSortBy('name_asc')}>Sort A-Z</button>
              <button onClick={() => setSortBy('name_desc')}>Sort Z-A</button>
            </th>
            <th>
              Total Units Sold
              <button onClick={() => setSortBy('units_asc')}>Sort by Least</button>
              <button onClick={() => setSortBy('units_desc')}>Sort by Most</button>
            </th>
          </tr>
        </thead>
        <tbody>
          {data && data.items_performance
            .sort((a, b) => {
              if (sortBy === 'name_asc') return a.name.localeCompare(b.name);
              if (sortBy === 'name_desc') return b.name.localeCompare(a.name);
              if (sortBy === 'units_asc') return a.total_units_sold - b.total_units_sold;
              if (sortBy === 'units_desc') return b.total_units_sold - a.total_units_sold;
              return 0;
            })
            .map((item, index) => (
              <tr key={index}>
                <td>{item.item_id}</td>
                <td>{item.name}</td>
                <td>{item.total_units_sold}</td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
};

export default ItemPerformance;