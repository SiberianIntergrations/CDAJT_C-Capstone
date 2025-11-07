// src/front-end/src/app/orders/page.jsx
"use client";
import React, { useEffect } from "react";
import { useRouter } from "next/navigation";
import Orders from "@/components/employee/Orders";

const OrdersPage = ({ user }) => {
  const router = useRouter();

  useEffect(() => {
    if (!user || user.role !== "employee") {
      router.push("/unauthorized");
    }
  }, [user, router]);
  if (!user || user.role !== "employee") return null;

  return <Orders />;
};

export default OrdersPage;
