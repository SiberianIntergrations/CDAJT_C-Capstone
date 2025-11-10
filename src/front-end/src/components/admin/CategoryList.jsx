import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  CircularProgress,
  Alert,
  IconButton,
  Stack,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from "@mui/material";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import { Edit, Trash2, Plus, AlertTriangle } from "lucide-react";
import api from "@/config/api";

const CategoryList = ({ onEditCategory, onCreateCategory }) => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // For delete dialog
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [categoryToDelete, setCategoryToDelete] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const fetchCategories = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await api.get("/category");
      setCategories(Array.isArray(data) ? data : []);
    } catch (err) {
      setError("Failed to fetch categories");
      setCategories([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleDeleteClick = (category) => {
    setCategoryToDelete(category);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    setDeleting(true);
    setError(null);
    try {
      await api.delete(
        `/Category/delete_category/${categoryToDelete.category_id}`
      );
      await fetchCategories();
      setDeleteDialogOpen(false);
      setCategoryToDelete(null);
    } catch (err) {
      setError("Failed to delete category");
    } finally {
      setDeleting(false);
    }
  };

  const handleDeleteCancel = () => {
    setDeleteDialogOpen(false);
    setCategoryToDelete(null);
  };

  const columns = [
    {
      field: "category_name",
      headerName: "Name",
      flex: 1,
      minWidth: 180,
    },
    {
      field: "description",
      headerName: "Description",
      flex: 2,
      minWidth: 250,
    },
    {
      field: "adult_limit",
      headerName: "Adult Limit",
      width: 110,
      type: "number",
    },
    {
      field: "child_limit",
      headerName: "Child Limit",
      width: 110,
      type: "number",
    },
    {
      field: "senior_limit",
      headerName: "Senior Limit",
      width: 110,
      type: "number",
    },
    {
      field: "total_limit",
      headerName: "Total Limit",
      width: 110,
      type: "number",
    },
    {
      field: "actions",
      headerName: "Actions",
      width: 140,
      renderCell: (params) => (
        <Stack direction="row" spacing={1}>
          <IconButton
            onClick={() => {
              // Map row fields to dialog expected props
              const row = params.row;
              onEditCategory?.({
                ...row,
                name: row.category_name,
                tot_limit: row.total_limit,
              });
            }}
            size="small"
          >
            <Edit size={20} />
          </IconButton>
          <IconButton
            onClick={() => handleDeleteClick(params.row)}
            size="small"
            color="error"
          >
            <Trash2 size={20} />
          </IconButton>
        </Stack>
      ),
      sortable: false,
      filterable: false,
    },
  ];

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="40vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Stack direction="row" alignItems="center" spacing={2} mb={2}>
        <Typography variant="h4">Categories</Typography>
        <IconButton color="primary" onClick={() => onCreateCategory?.()}>
          <Plus />
        </IconButton>
      </Stack>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      <DataGrid
        rows={categories}
        columns={columns}
        getRowId={(row) => row.category_id}
        autoHeight
        disableRowSelectionOnClick
        slots={{ toolbar: GridToolbar }}
        slotProps={{ toolbar: { showQuickFilter: true } }}
        initialState={{
          pagination: { paginationModel: { pageSize: 10 } },
        }}
        pageSizeOptions={[10, 25, 50]}
        sx={{
          "& .MuiDataGrid-cell:focus": { outline: "none" },
        }}
      />

      {/* Delete Confirmation Dialog */}
      <Dialog
        open={deleteDialogOpen}
        onClose={handleDeleteCancel}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>
          <Stack direction="row" alignItems="center" spacing={1}>
            <AlertTriangle color="error" />
            <Typography variant="h6">Confirm Delete</Typography>
          </Stack>
        </DialogTitle>
        <DialogContent>
          <Typography mb={2}>
            Are you sure you want to delete "
            {categoryToDelete?.category_name}"? This action cannot be undone.
          </Typography>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleDeleteCancel}>Cancel</Button>
          <Button
            variant="contained"
            color="error"
            onClick={handleDeleteConfirm}
            disabled={deleting}
          >
            {deleting ? <CircularProgress size={24} /> : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default CategoryList;
