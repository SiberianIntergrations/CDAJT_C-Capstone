import { axiosInstance, createApiUrl } from "@/config/api";
import React, { useState, useEffect } from "react";
import {
  Dialog,
  Box,
  Stack,
  Typography,
  IconButton,
  Alert,
  TableContainer,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  Paper,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Switch,
  FormControlLabel,
  Grid,
  Button,
  CircularProgress,
} from "@mui/material";
import { X, Edit, Trash2 } from "lucide-react";

const MenuAssignmentDialog = ({
  open,
  onClose,
  selectedItem,
  menus,
  onSuccess,
  initialAssignments = [],
}) => {
  const [formData, setFormData] = useState({
    menu_id: "",
    price: "0",
    is_add_on: false,
    status: "available",
    adult_limit: 0,
    child_limit: 0,
    senior_limit: 0,
    tot_limit: 0,
  });
  const [submitting, setSubmitting] = useState(false);
  const [assignments, setAssignments] = useState(initialAssignments);
  const [editingAssignment, setEditingAssignment] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchAssignments = async () => {
      if (!selectedItem) return;
      try {
        const response = await axiosInstance.get(
          createApiUrl(`/menu-item-assignments/by-item/${selectedItem.item_id}`)
        );
        setAssignments(response.data);
      } catch (err) {
        setError("Failed to fetch assignments");
      }
    };
    fetchAssignments();
  }, [selectedItem]);

  useEffect(() => {
    if (editingAssignment) {
      setFormData({
        menu_id: editingAssignment.menu_id,
        price: editingAssignment.price,
        is_add_on: editingAssignment.is_add_on === true,
        status: (editingAssignment.status || "AVAILABLE").toLowerCase(),
        adult_limit: editingAssignment.adult_limit || 0,
        child_limit: editingAssignment.child_limit || 0,
        senior_limit: editingAssignment.senior_limit || 0,
        tot_limit: editingAssignment.tot_limit || 0,
      });
    }
  }, [editingAssignment]);

  const handleDeleteAssignment = async (menuId, itemId) => {
    try {
      await axiosInstance.delete(
        createApiUrl(`/menu-item-assignments/${menuId}/${itemId}`)
      );
      setAssignments((prev) => prev.filter((a) => a.menu_id !== menuId));
      resetForm();
    } catch (err) {
      setError("Failed to delete assignment");
    }
  };

  const resetForm = () => {
    setFormData({
      menu_id: "",
      price: "",
      is_add_on: false,
      status: "available",
      adult_limit: 0,
      child_limit: 0,
      senior_limit: 0,
      tot_limit: 0,
    });
    setEditingAssignment(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const url = editingAssignment
        ? createApiUrl(
            `/menu-item-assignments/${editingAssignment.menu_id}/${selectedItem.item_id}`
          )
        : createApiUrl("/menu-item-assignments");

      const response = await axiosInstance({
        method: editingAssignment ? "PUT" : "POST",
        url,
        data: {
          ...formData,
          menu_id: parseInt(formData.menu_id),
          item_id: selectedItem.item_id,
          price: parseFloat(formData.price),
          adult_limit: parseInt(formData.adult_limit) || 0,
          child_limit: parseInt(formData.child_limit) || 0,
          senior_limit: parseInt(formData.senior_limit) || 0,
          tot_limit: parseInt(formData.tot_limit) || 0,
          status: formData.status,
        },
      });

      if (editingAssignment) {
        setAssignments((prev) =>
          prev.map((a) =>
            a.menu_id === response.data.menu_id ? response.data : a
          )
        );
      } else {
        setAssignments((prev) => [...prev, response.data]);
      }

      resetForm();
      if (onSuccess) onSuccess();
    } catch (err) {
      setError(
        editingAssignment
          ? "Failed to update assignment"
          : "Failed to create assignment"
      );
    } finally {
      setSubmitting(false);
    }
  };

  const availableMenus = menus.filter(
    (menu) =>
      !assignments.some(
        (assignment) =>
          assignment.menu_id === menu.menu_id &&
          assignment.menu_id !== editingAssignment?.menu_id
      )
  );

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <Box sx={{ p: 3 }}>
          <Stack
            direction="row"
            justifyContent="space-between"
            alignItems="center"
            mb={2}
          >
            <Stack spacing={0.5}>
              <Typography variant="h6">Menu Assignments</Typography>
              <Typography variant="subtitle2" color="text.secondary">
                {selectedItem?.name}
              </Typography>
            </Stack>
            <IconButton onClick={onClose} size="small">
              <X size={20} />
            </IconButton>
          </Stack>

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          <Typography variant="subtitle1" gutterBottom sx={{ mt: 3 }}>
            Current Assignments
          </Typography>

          {assignments.length > 0 ? (
            <TableContainer component={Paper} sx={{ mb: 3 }}>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell sx={{ color: "white" }}>Menu</TableCell>
                    <TableCell sx={{ color: "white" }}>Price</TableCell>
                    <TableCell sx={{ color: "white" }}>Status</TableCell>
                    <TableCell sx={{ color: "white" }}>Add-on</TableCell>
                    <TableCell sx={{ color: "white" }} align="right">
                      Actions
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {assignments.map((assignment) => (
                    <TableRow key={assignment.menu_id}>
                      <TableCell>
                        {
                          menus.find((m) => m.menu_id === assignment.menu_id)
                            ?.name
                        }
                      </TableCell>
                      <TableCell>${assignment.price.toFixed(2)}</TableCell>
                      <TableCell>
                        {assignment.status.charAt(0).toUpperCase() +
                          assignment.status.slice(1)}
                      </TableCell>
                      <TableCell>
                        {assignment.is_add_on ? "Yes" : "No"}
                      </TableCell>
                      <TableCell align="right">
                        <IconButton
                          size="small"
                          onClick={() => setEditingAssignment(assignment)}
                          sx={{ mr: 1 }}
                        >
                          <Edit size={16} />
                        </IconButton>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() =>
                            handleDeleteAssignment(
                              assignment.menu_id,
                              selectedItem.item_id
                            )
                          }
                        >
                          <Trash2 size={16} />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          ) : (
            <Alert severity="info" sx={{ mb: 3 }}>
              No current assignments
            </Alert>
          )}

          <Typography variant="subtitle1" gutterBottom>
            {editingAssignment ? "Edit Assignment" : "Add New Assignment"}
          </Typography>

          <Stack spacing={2}>
            <FormControl fullWidth required>
              <InputLabel>Menu</InputLabel>
              <Select
                value={formData.menu_id}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, menu_id: e.target.value }))
                }
                label="Menu"
                disabled={Boolean(editingAssignment)}
              >
                {availableMenus.map((menu) => (
                  <MenuItem key={menu.menu_id} value={menu.menu_id}>
                    {menu.name}
                  </MenuItem>
                ))}
                {editingAssignment && (
                  <MenuItem value={editingAssignment.menu_id}>
                    {
                      menus.find((m) => m.menu_id === editingAssignment.menu_id)
                        ?.name
                    }
                  </MenuItem>
                )}
              </Select>
            </FormControl>

            <Stack direction="row" spacing={2}>
              <TextField
                label="Price"
                type="number"
                value={formData.price}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, price: e.target.value }))
                }
                required
                disabled={formData.is_add_on}
                inputProps={{ step: "0.01" }}
                sx={{ flex: 1 }}
              />

              <FormControl sx={{ flex: 1 }}>
                <InputLabel>Status</InputLabel>
                <Select
                  value={formData.status || "available"}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev, status: e.target.value }))
                  }
                  label="Status"
                >
                  <MenuItem value="available">Available</MenuItem>
                  <MenuItem value="unavailable">Unavailable</MenuItem>
                  <MenuItem value="discontinued">Discontinued</MenuItem>
                </Select>
              </FormControl>
            </Stack>

            <FormControlLabel
              control={
                <Switch
                  checked={formData.is_add_on === true}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      is_add_on: e.target.checked,
                      price: e.target.checked ? "0" : prev.price,
                    }))
                  }
                />
              }
              label="Is Add-on Item"
            />

            <Grid
              container
              spacing={2}
              sx={{
                m: 0,
                pl: 0,
                pr: "0.75rem",
                "& .MuiGrid-item": {
                  pl: 0,
                  pr: 0,
                },
              }}
            >
              <Grid item xs={6}>
                <TextField
                  sx={{ pr: 0.75 }}
                  fullWidth
                  label="Adult Limit"
                  type="number"
                  value={formData.adult_limit}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      adult_limit: e.target.value,
                    }))
                  }
                  inputProps={{ min: 0, max: 99 }}
                />
              </Grid>
              <Grid item xs={6} sx={{ pr: 0 }}>
                <TextField
                  sx={{ pl: 1 }}
                  fullWidth
                  label="Child Limit"
                  type="number"
                  value={formData.child_limit}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      child_limit: e.target.value,
                    }))
                  }
                  inputProps={{ min: 0, max: 99 }}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  sx={{ pr: 0.75 }}
                  fullWidth
                  label="Senior Limit"
                  type="number"
                  value={formData.senior_limit}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      senior_limit: e.target.value,
                    }))
                  }
                  inputProps={{ min: 0, max: 99 }}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  sx={{ pl: 1 }}
                  fullWidth
                  label="Tot Limit"
                  type="number"
                  value={formData.tot_limit}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      tot_limit: e.target.value,
                    }))
                  }
                  inputProps={{ min: 0, max: 99 }}
                />
              </Grid>
            </Grid>

            <Stack direction="row" spacing={2}>
              {editingAssignment && (
                <Button
                  onClick={resetForm}
                  disabled={submitting}
                  sx={{ flex: 1 }}
                >
                  Cancel Edit
                </Button>
              )}
              <Button
                type="submit"
                variant="contained"
                disabled={submitting}
                sx={{ flex: 1 }}
              >
                {submitting ? (
                  <CircularProgress size={24} />
                ) : editingAssignment ? (
                  "Update Assignment"
                ) : (
                  "Add Assignment"
                )}
              </Button>
            </Stack>
          </Stack>
        </Box>
      </form>
    </Dialog>
  );
};

export default MenuAssignmentDialog;
