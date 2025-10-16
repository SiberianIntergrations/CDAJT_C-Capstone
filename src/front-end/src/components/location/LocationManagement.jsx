import { useState, useEffect } from "react";
import { DataGrid } from "@mui/x-data-grid";
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
  Typography,
  Stack,
  Alert,
} from "@mui/material";
import { Edit, Delete, Plus, Menu, AlertTriangle, Trash2 } from "lucide-react";
import { styled } from "@mui/material/styles";
import LocationForm from "@/components/location/LocationForm";
import MenuAssignmentForm from "@/components/location/MenuAssignmentForm";
import { axiosInstance, createApiUrl, msalAxiosClient } from "@/config/api";

const LocationManagement = ({ msalInstance }) => {
  const [locations, setLocations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedLocation, setSelectedLocation] = useState(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [isMenuFormOpen, setIsMenuFormOpen] = useState(false);
  const [formMode, setFormMode] = useState("create");
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [locationToDelete, setLocationToDelete] = useState(null);

  const fetchLocations = async () => {
    try {
      setLoading(true);
      const msalAxios = msalAxiosClient(msalInstance);
      const response = await msalAxios.get(createApiUrl("/locations"));
      setLocations(response.data);
    } catch (err) {
      setError("Failed to fetch locations");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLocations();
  }, []);

  const handleCreate = () => {
    setSelectedLocation(null);
    setFormMode("create");
    setIsFormOpen(true);
  };

  const handleEdit = (location) => {
    setSelectedLocation(location);
    setFormMode("edit");
    setIsFormOpen(true);
  };

  const handleDeleteClick = (location) => {
    setLocationToDelete(location);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    try {
      await axiosInstance.delete(
        createApiUrl(`/locations/${locationToDelete.location_id}`)
      );
      await fetchLocations();
      setDeleteDialogOpen(false);
      setLocationToDelete(null);
    } catch (err) {
      setError("Failed to delete location");
      console.error(err);
    }
  };

  const handleSubmit = async (formData) => {
    try {
      if (formMode === "create") {
        await axiosInstance.post(createApiUrl("/locations"), formData);
      } else {
        await axiosInstance.put(
          createApiUrl(`/locations/${selectedLocation.location_id}`),
          formData
        );
      }
      await fetchLocations();
      setIsFormOpen(false);
    } catch (err) {
      setError("Failed to save location");
      console.error(err);
    }
  };

  const AddButton = styled(IconButton)(({ theme }) => ({
    backgroundColor: theme.palette.primary.main,
    color: "white",
    padding: theme.spacing(1),
    minWidth: "auto",
    "&:hover": {
      backgroundColor: theme.palette.primary.dark,
    },
    "& svg": {
      width: 20,
      height: 20,
    },
  }));

  const columns = [
    { field: "location_id", headerName: "ID", width: 90 },
    { field: "name", headerName: "Name", width: 200 },
    { field: "address_one", headerName: "Address 1", width: 200 },
    {
      field: "actions",
      headerName: "Actions",
      width: 150,
      renderCell: (params) => (
        <Stack direction="row" spacing={1}>
          <IconButton onClick={() => handleEdit(params.row)} size="small">
            <Edit size={20} />
          </IconButton>
          <IconButton
            onClick={() => handleDeleteClick(params.row)}
            size="small"
            color="error"
          >
            <Trash2 size={20} />
          </IconButton>
          <IconButton
            onClick={() => {
              setSelectedLocation(params.row);
              setIsMenuFormOpen(true);
            }}
            size="small"
            color="primary"
          >
            <Menu size={20} />
          </IconButton>
        </Stack>
      ),
    },
  ];

  return (
    <Box pt={2}>
      <Stack direction="row" justifyContent="left" alignItems="center" mb={2}>
        <Typography pr={3} variant="h4">
          Location Management
        </Typography>
        <AddButton onClick={handleCreate}>
          <Plus />
        </AddButton>
      </Stack>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <DataGrid
        rows={locations}
        columns={columns}
        loading={loading}
        getRowId={(row) => row.location_id}
        autoHeight
        disableSelectionOnClick
        sx={{
          bgcolor: "background.paper",
          "& .MuiDataGrid-cell": {
            display: "flex",
            alignItems: "center",
          },
          "& .MuiDataGrid-cell:focus": {
            outline: "none",
          },
        }}
      />

      <Dialog
        open={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <LocationForm
          initialData={selectedLocation}
          onSubmit={handleSubmit}
          onClose={() => setIsFormOpen(false)}
          mode={formMode}
        />
      </Dialog>

      <Dialog
        open={isMenuFormOpen}
        onClose={() => setIsMenuFormOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <MenuAssignmentForm
          location={selectedLocation}
          onClose={() => setIsMenuFormOpen(false)}
          axiosInstance={axiosInstance}
        />
      </Dialog>

      <Dialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <AlertTriangle color="error" size={24} />
          Confirm Delete Location
        </DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the location "
            {locationToDelete?.name}"? This will remove all menu assignments for
            this location. This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setDeleteDialogOpen(false)} variant="outlined">
            Cancel
          </Button>
          <Button
            onClick={handleDeleteConfirm}
            variant="contained"
            color="error"
            autoFocus
          >
            Delete Location
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default LocationManagement;
