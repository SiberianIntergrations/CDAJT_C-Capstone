"use client";
import Link from "next/link";

const Error = () => {
  return (
    <div style={{ textAlign: "center", padding: "50px" }}>
      <h1>Error</h1>
      <p>Sorry, an error occurred while processing your request.</p>
      <Link href="/">Return to Home</Link>
    </div>
  );
};

export default Error;
