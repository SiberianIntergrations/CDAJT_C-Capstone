import React from "react";
import Link from "next/link";

const NotFound = () => {
  return (
    <div style={{ textAlign: "center", padding: "50px" }}>
      <h1>Page Not Found</h1>
      <p>Sorry, the page you are looking for does not exist.</p>
      <Link href="/">Return to Home</Link>
    </div>
  );
};

export default NotFound;
