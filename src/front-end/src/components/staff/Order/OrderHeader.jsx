import React from "react";
import { PageHeader } from "@/components/common/PageHeader";

export const Header = ({ onRefresh, isLoading }) => {
  return (
    <PageHeader
      title="Order Management"
      onRefresh={onRefresh}
      isLoading={isLoading}
    />
  );
};