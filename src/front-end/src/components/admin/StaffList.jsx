import React from "react";
// TODO: Fix depcrecated DataGrid import
import { DataGrid, GridActionsCellItem, GridToolbar } from "@mui/x-data-grid";
import {
  Box,
  CircularProgress,
  Alert,
  Chip,
  Typography,
  Tooltip,
} from "@mui/material";
import { Edit, Key, UserMinus, UserCheck } from "lucide-react";

const StaffList = ({
  staff = [],
  currentUser,
  isLoading = false,
  error = null,
  onEdit,
  onStatusChange,
  onChangePassword,
}) => {

  const getStatusColor = (status) => {

    switch (status) {
      case "active":
        return "success";
      case "inactive":
        return "error";
      default:
        return "default";
    }
  };

  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight={200}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        {error}
      </Alert>
    );
  }

  const columns = [
    {
      field: "name",
      headerName: "Name",
      flex: 1,
      renderCell: (params) => (
        <Typography
          variant="body1"
          sx={{ display: "flex", alignItems: "center", height: "100%" }}
        >
          {params.row.first_name} {params.row.last_name}
        </Typography>
      ),
    },
    {
      field: "email",
      headerName: "Email",
      flex: 1,
      renderCell: (params) => (
        <Typography
          variant="body2"
          sx={{ display: "flex", alignItems: "center", height: "100%" }}
        >
          {params.row.email}
        </Typography>
      ),
    },
    {
      field: "Location",
      headerName: "Location",
      flex:1,
      renderCell: (params) =>(
        <Chip
          label={params.row.location}
          size="small"
        />
      )
    },
    {
      field: "role",
      headerName: "Role",
      flex: 1,
      renderCell: (params) => (
        <Chip
          label={params.row.role}
          color={params.row.role === "admin" ? "error" : "primary"}
          size="small"
        />
      ),
    },
    {
      field: "status",
      headerName: "Status",
      flex: 1,
      renderCell: (params) => (
        <Chip
          label={params.row.status}
          color={getStatusColor(params.row.status)}
          size="small"
        />
      ),
    },
    {
      field: "actions",
      headerName: "Actions",
      type: "actions",
      width: 140,
      getActions: (params) => [
        <Tooltip title="Edit in Entra" key="edit">
          <GridActionsCellItem
            icon={<Edit size={20} />}
            label="Edit"
            onClick={() => onEdit(params.row)}
          />
        </Tooltip>,
        <Tooltip title="Reset Password in Entra" key="changePassword">
          <GridActionsCellItem
            icon={<Key size={20} />}
            label="Change Password"
            onClick={() => onChangePassword(params.row)}
          />
        </Tooltip>,
        // Optionally remove or disable status change
        // <Tooltip title="Status change managed in Entra" key="statusChange">
        //   <span />
        // </Tooltip>,
      ],
    },
  ];

  return (
    <Box sx={{ height: 600, width: "100%" }}>
      <DataGrid
        rows={staff}
        columns={columns}
        getRowId={(row) => row.user_id}
        components={{
          Toolbar: GridToolbar,
        }}
        pageSize={10}
        rowsPerPageOptions={[10, 25, 50]}
        disableSelectionOnClick
        style={{ height: "auto", minHeight: 400 }}
        sx={{
          "& .MuiDataGrid-columnHeaders": {
            backgroundColor: "primary.dark",
            fontSize: "1rem",
            fontWeight: "bold",
          },
          "& .MuiDataGrid-cell": {
            display: "flex",
            alignItems: "center",
          },
        }}
      />
    </Box>
  );
};

export default StaffList;
