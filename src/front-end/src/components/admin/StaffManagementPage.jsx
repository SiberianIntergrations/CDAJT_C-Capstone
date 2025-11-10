import React, { useState, useEffect } from "react";
import {
  Box,
  Container,
  Typography,
  Alert,
  Snackbar,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { UserPlus } from "lucide-react";
import StaffList from "@/components/admin/StaffList";
import api from "@/config/api";
import { useMsal } from "@azure/msal-react";
import { silentRequest } from "@/config/auth";

const StaffManagementPage = () => {
  const [staff, setStaff] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success",
  });

  const { instance: msalInstance } = useMsal();
  const accounts = msalInstance.getAllAccounts();
  const account = accounts && accounts.length > 0 ? accounts[0] : null;
  const currentUser = account ? account.username : null;

  // Replace with your actual Entra group IDs
  const STAFF_GROUP_ID = "972540cb-bba6-4ebd-9f74-222073fce290";
  const ADMIN_GROUP_ID = "53b1f86b-82a1-4572-b8d3-9a78b5a6e689";

  // Fetch staff from Entra group members using MS Graph API
  const fetchStaff = async () => {
    try {
      setLoading(true);
      setError(null);

      if (!account) throw new Error("No authenticated account found");

      // Add required Graph scopes
      const graphScopes = [
        "User.ReadBasic.All",
        "GroupMember.Read.All",
        "Directory.Read.All"
      ];

      // Acquire access token for MS Graph
      const response = await msalInstance.acquireTokenSilent({
        ...silentRequest,
        account,
        scopes: graphScopes,
      });
      const accessToken = response.accessToken;

      // Helper to fetch group members
      const fetchGroupMembers = async (groupId) => {
        const res = await fetch(
          `https://graph.microsoft.com/v1.0/groups/${groupId}/members?$select=id,displayName,givenName,surname,mail,userPrincipalName,accountEnabled,officeLocation`,
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );
        if (!res.ok) throw new Error("Failed to fetch group members");
        const data = await res.json();
        return data.value || [];
      };

      // Fetch both groups, merge, and dedupe by user id
      const [staffMembers, adminMembers] = await Promise.all([
        fetchGroupMembers(STAFF_GROUP_ID),
        fetchGroupMembers(ADMIN_GROUP_ID),
      ]);
      const allUsersMap = {};
      staffMembers.forEach((u) => {
        allUsersMap[u.id] = { ...u, role: "staff" };
      });
      adminMembers.forEach((u) => {
        allUsersMap[u.id] = { ...u, role: "admin" };
      });

      const allUsers = Object.values(allUsersMap);

      // Map to staff list format, filter out group objects
      const mapped = allUsers
        .filter(user => !user['@odata.type'] || !user['@odata.type'].toLowerCase().includes("group"))
        .map((user) => ({
          user_id: user.id,
          first_name: user.givenName || "",
          last_name: user.surname || "",
          email: user.mail || user.userPrincipalName,
          location: "", // Always empty for now
          role: user.role,
          status: user.accountEnabled === false ? "inactive" : "active",
        }));

      setStaff(mapped);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStaff();
    // eslint-disable-next-line
  }, []);

  // Only redirect to Entra for edit/password
  const handleEditRedirect = (staffMember) => {
    window.open(
      `https://entra.microsoft.com/#view/Microsoft_AAD_UsersAndTenants/UserProfileMenuBlade/~/overview/userId/${staffMember.user_id}`,
      "_blank"
    );
  };

  const handlePasswordRedirect = (staffMember) => {
    window.open(
      `https://entra.microsoft.com/#view/Microsoft_AAD_UsersAndTenants/UserProfileMenuBlade/~/resetPassword/userId/${staffMember.user_id}`,
      "_blank"
    );
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
      </Box>

      <StaffList
        staff={staff}
        currentUser={currentUser}
        isLoading={loading}
        error={error}
        onEdit={handleEditRedirect}
        onStatusChange={null}
        onChangePassword={handlePasswordRedirect}
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
