import { useState, useEffect } from "react";
import {
  Box,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  CircularProgress,
  Alert,
} from "@mui/material";
import { Trash, Plus } from "lucide-react";

const MenuAssignmentForm = ({ location, onClose, axiosInstance }) => {
  const [menus, setMenus] = useState([]);
  const [assignedMenus, setAssignedMenus] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [allMenusRes, assignedMenusRes] = await Promise.all([
          axiosInstance.get("/menus"),
          axiosInstance.get(`/locations/${location.location_id}/menus`),
        ]);

        setMenus(allMenusRes.data);
        setAssignedMenus(assignedMenusRes.data);
      } catch (err) {
        setError("Failed to fetch menus");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [location]);

  const handleAssign = async (menuId) => {
    try {
      await axiosInstance.post(`/locations/${location.location_id}/menus`, {
        menu_id: menuId,
      });
      setAssignedMenus([...assignedMenus, menuId]);
    } catch (err) {
      setError("Failed to assign menu");
      console.error(err);
    }
  };

  const handleUnassign = async (menuId) => {
    try {
      await axiosInstance.delete(
        `/locations/${location.location_id}/menus/${menuId}`
      );
      setAssignedMenus(assignedMenus.filter((id) => id !== menuId));
    } catch (err) {
      setError("Failed to unassign menu");
      console.error(err);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={3}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <>
      <DialogTitle>Manage Menus - {location.name}</DialogTitle>
      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <List>
          {menus.map((menu) => (
            <ListItem key={menu.menu_id}>
              <ListItemText primary={menu.name} />
              <ListItemSecondaryAction>
                {assignedMenus.includes(menu.menu_id) ? (
                  <IconButton
                    edge="end"
                    onClick={() => handleUnassign(menu.menu_id)}
                    color="error"
                  >
                    <Trash size={20} />
                  </IconButton>
                ) : (
                  <IconButton
                    edge="end"
                    onClick={() => handleAssign(menu.menu_id)}
                    color="primary"
                  >
                    <Plus size={20} />
                  </IconButton>
                )}
              </ListItemSecondaryAction>
            </ListItem>
          ))}
        </List>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </>
  );
};

export default MenuAssignmentForm;
