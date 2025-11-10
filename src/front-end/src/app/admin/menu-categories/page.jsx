"use client";
import { useState } from "react";
import CategoryList from "@/components/admin/CategoryList";
import CategoryManagementDialog from "@/components/admin/CategoryManagementDialog";

export default function MenuCategoriesPage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleEditCategory = (category) => {
    setSelectedCategory(category);
    setDialogOpen(true);
  };

  const handleCreateCategory = () => {
    setSelectedCategory(null);
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedCategory(null);
  };

  const handleDialogSuccess = () => {
    setDialogOpen(false);
    setSelectedCategory(null);
    setRefreshKey((k) => k + 1); // trigger CategoryList refresh
  };

  return (
    <>
      <CategoryList
        onEditCategory={handleEditCategory}
        onCreateCategory={handleCreateCategory}
        key={refreshKey}
      />
      <CategoryManagementDialog
        open={dialogOpen}
        onClose={handleDialogClose}
        selectedCategory={selectedCategory}
        onSuccess={handleDialogSuccess}
      />
    </>
  );
}
