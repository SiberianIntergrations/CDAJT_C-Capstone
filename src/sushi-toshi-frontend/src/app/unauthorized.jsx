// File: sushi-toshi-frontend/pages/unauthorized.js
import React from "react";
import Link from "next/link";

const Unauthorized = () => {
  return (
    <div style={{ textAlign: "center", padding: "50px" }}>
      <h1>Unauthorized Access</h1>
      <p>Sorry, you do not have permission to access this page.</p>
      <Link href="/">Return to Home</Link>
    </div>
  );
};

export default Unauthorized;
