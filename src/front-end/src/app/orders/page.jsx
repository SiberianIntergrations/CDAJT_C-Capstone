"use client";
import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Orders from "@/components/employee/Orders";
import { useAuth } from "@/hooks/useAuth";

const OrdersPage = () => {
  const router = useRouter();
  const { isAuthenticated, userRole, loading } = useAuth();
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthorized, setIsAuthorized] = useState(false);

  useEffect(() => {
    if (loading) {
      setIsLoading(true);
      return;
    }
    if (!isAuthenticated) {
      router.push("/auth/login");
      return;
    }
    if (userRole !== "staff" && userRole !== "admin") {
      router.push("/unauthorized");
      return;
    }
    setIsAuthorized(true);
    setIsLoading(false);
  }, [router, isAuthenticated, userRole, loading]);

  if (isLoading || !isAuthorized) return null;

  return <Orders />;
};

export default OrdersPage;
