// src/front-end/src/components/employee/Orders.jsx
import React, { useEffect, useState } from "react";

const Orders = () => {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    const fetchOrders = async () => {
      const response = await fetch("/api/orders"); // Adjust for the api being used
      const data = await response.json();
      setOrders(data);
    };

    fetchOrders();
  }, []);

  return (
    <div>
      <h1>All Orders</h1>
      <table>
        <thead>
          <tr>
            <th>Table Number</th>
            <th>Order Details</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => (
            <tr key={order.id}>
              <td>{order.tableNumber}</td>
              <td>{order.details}</td>
              <td>{order.status}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Orders;
