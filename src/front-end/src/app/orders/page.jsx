"use client";
import React, { useContext } from "react";
import { useRouter } from "next/navigation";
import Orders from "@/components/employee/Orders";
import { AuthContext } from "@/app/layout";

const OrdersPage = () => {
  const { userRole, isLoggedIn, authLoading } = useContext(AuthContext);
  const router = useRouter();

  if (authLoading) {
    return null;
  }

  if (!isLoggedIn || userRole !== "staff") {
    console.warn("Unauthorized access attempt to Orders page.");
    return null;
  }

  return <Orders />;
};

export default OrdersPage;
