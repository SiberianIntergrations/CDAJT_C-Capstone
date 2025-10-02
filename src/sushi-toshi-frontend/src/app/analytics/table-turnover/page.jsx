import React from "react";
import { useRouter } from "next/navigation";
import TableTurnover from "@/components/analytics/TableTurnover";

const TableTurnoverPage = () => {
  const router = useRouter();

  return (
    <div>
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 1fr)",
          gap: "1rem",
          marginTop: "1rem",
        }}
      >
        <button onClick={() => router.push("/analytics/item-performance")}>
          Item Performance
        </button>
        <button onClick={() => router.push("/analytics/browsing-behavior")}>
          Browsing Behavior
        </button>
        <button onClick={() => router.push("/analytics/order-timing")}>
          Order Timing
        </button>
      </div>
      <TableTurnover />
    </div>
  );
};

export default TableTurnoverPage;
