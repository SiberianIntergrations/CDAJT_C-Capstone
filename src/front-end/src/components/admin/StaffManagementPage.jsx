import React, { useState, useEffect } from "react";
import {
  Box,
  Button,
  Container,
  Typography,
  Alert,
  Snackbar,
  Dialog,
  IconButton,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { UserPlus } from "lucide-react";
import StaffManagementForm from "@/components/admin/StaffManagementForm";
import StaffList from "@/components/admin/StaffList";
import PasswordChangeDialog from "@/components/admin/PasswordChangeDialog";
import api from "@/config/api";

// TODO: Missing Staff api

const StaffManagementPage = () => {
  const [staff, setStaff] = useState([]);
  
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [passwordDialogOpen, setPasswordDialogOpen] = useState(false);
  const [selectedStaff, setSelectedStaff] = useState(null);
  const [editMode, setEditMode] = useState(false);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success",
  });
  const [submitting, setSubmitting] = useState(false);

  const fetchStaff = async () => {
    try {
      setLoading(true);

      const response = await api.get("/staff");
      console.log(response);
      console.log(response.statusText);
      if (response.status !== 200)
        throw new Error("Failed to fetch staff members");

      const data = response.data;
      setStaff(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const StyledButton = styled(IconButton)(({ theme }) => ({
    backgroundColor: theme.palette.primary.main,
    color: "white",
    padding: theme.spacing(1),
    minWidth: "auto",
    "&:hover": {
      backgroundColor: theme.palette.primary.dark,
    },
    "& svg": {
      width: 30,
      height: 30,
    },
  }));

  const token = localStorage.getItem("access_token");
  const tokenData = JSON.parse(atob(token.split(".")[1]));
  const currentUser = tokenData.sub;

  useEffect(() => {
    fetchStaff();
    console.log(staff);
  }, []);

const handleStatusChange = async (staffMember, newStatus) => {
  try {
    setSubmitting(true);
    
    const response = await api.put(`/staff/${staffMember.user_id}`, {
      status: newStatus,
    });
    
    await fetchStaff();
    
    setSnackbar({
      open: true,
      message: `Staff member ${
        newStatus === "active" ? "activated" : "deactivated"
      } successfully`,
      severity: "success",
    });
  } catch (err) {
    console.error("Error updating staff status:", err);
    setError(
      err.response?.data?.detail || "Failed to update staff member status"
    );
  } finally {
    setSubmitting(false);
  }
};

  const handleCreateStaff = async (formData) => {
    try {
      setSubmitting(true);

      let role = formData.role;
      formData.status = "active";
      // delete formData.role;

      // await api.post(`/staff/?role=${role}`, formData);
      await api.post(`/staff/`, formData);
      await fetchStaff();
      setFormOpen(false);
      setSnackbar({
        open: true,
        message: "Staff member created successfully",
        severity: "success",
      });
    } catch (err) {
      console.error("Error creating staff member:", err);
      setError(err.response?.data?.detail || "Failed to create staff member");
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdateStaff = async (formData) => {
    try {
      setSubmitting(true);
      console.log(formData);

      const response = await api.put(`/staff/${selectedStaff.user_id}`,
        formData
      );

      if (response.status !== 200)
        throw new Error("Failed to update staff member");

      await fetchStaff();
      setFormOpen(false);
      setSnackbar({
        open: true,
        message: "Staff member updated successfully",
        severity: "success",
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
      setSelectedStaff(null);
      setEditMode(false);
    }
  };

  const handleChangePassword = async (passwordData) => {
    try {
      setSubmitting(true);
      console.log(passwordData)

      const response = await api.post(`/staff/${selectedStaff.user_id}/reset-password`,
        { new_password: passwordData.new_password }
      );
      if (response.status !== 200) {
        throw new Error(response.data?.detail || "Failed to reset password");
      }

      setPasswordDialogOpen(false);
      setSnackbar({
        open: true,
        message: "Password reset successfully",
        severity: "success",
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
      setSelectedStaff(null);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "left",
          alignItems: "center",
          gap: 2,
          mb: 4,
        }}
      >
        <Typography variant="h4">Staff Management</Typography>
        <StyledButton
          onClick={() => {
            setEditMode(false);
            setSelectedStaff(null);
            setFormOpen(true);
          }}
        >
          <UserPlus />
        </StyledButton>
      </Box>

      <StaffList
        staff={staff}
        currentUser={currentUser}
        isLoading={loading}
        error={error}
        onEdit={(staff) => {
          setSelectedStaff(staff);
          setEditMode(true);
          setFormOpen(true);
        }}
        onStatusChange={handleStatusChange}
        onChangePassword={(staff) => {
          setSelectedStaff(staff);
          setPasswordDialogOpen(true);
          
        }}
      />

      <Dialog
        open={formOpen}
        onClose={() => setFormOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <StaffManagementForm
          initialData={editMode ? selectedStaff : null}
          onSubmit={editMode ? handleUpdateStaff : handleCreateStaff}
          isLoading={submitting}
          error={error}
          mode={editMode ? "edit" : "create"}
        />
      </Dialog>

      <PasswordChangeDialog
        open={passwordDialogOpen}
        onClose={() => setPasswordDialogOpen(false)}
        onSubmit={handleChangePassword}
        staffMember={selectedStaff}
        isLoading={submitting}
        error={error}
      />

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert
          severity={snackbar.severity}
          onClose={() => setSnackbar({ ...snackbar, open: false })}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Container>
  );
};

export default StaffManagementPage;
