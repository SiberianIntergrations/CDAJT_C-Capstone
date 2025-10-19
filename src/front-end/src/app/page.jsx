"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import {getUserName, isAuthenticated} from "@/utils/token"

const HomePage = () => {
  const user = {}; // You would fetch or pass the actual user data here
  const router = useRouter();
  const [tableNumber, setTableNumber] = useState(null);
  const [userName, setUserName] = useState(null);

  useEffect(() => {
    if(isAuthenticated()){
      const name = getUserName();
      setUserName(name);
    }
  },[]);

  // useEffect(() => {
  //   const { table } = router.query;
  //   if (table) {
  //     setTableNumber(table);
  //   }
  // }, [router.query]);

  return (
    <div>
      <h1>Welcome to Sushi Toshi{userName ? `, ${userName}` : ""}!</h1>
      {/* add background images to match domain? */}

      {/* When the customer is redirected to this page, we can either make it the splash page before the Menu or directly to the menu. In this as well, you will be able to navigate to the order menu. */}
    </div>
  );
};

export default HomePage;
