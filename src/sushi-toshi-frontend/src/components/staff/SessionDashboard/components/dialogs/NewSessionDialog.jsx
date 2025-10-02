// File: sushi-toshi-frontend/components/staff/SessionDashboard/components/dialogs/NewSessionDialog.jsx
import { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
} from "@mui/material";
import { X } from "lucide-react";
import { useSession } from "../../context/SessionContext";
import { axiosInstance, createApiUrl } from "../../../../../config/api";

const NewSessionDialog = ({ open, onClose }) => {
  const { createSession } = useSession();
  const [selectedMenu, setSelectedMenu] = useState("");
  const [availableMenus, setAvailableMenus] = useState([]);

  useEffect(() => {
    if (open) {
      fetchMenus();
    }
  }, [open]);

  const fetchMenus = async () => {
    try {
      const response = await axiosInstance.get(createApiUrl("/menus"));
      if (response.statusText !== "OK")
        throw new Error("Failed to fetch menus");
      const data = response.data;
      setAvailableMenus(data);
    } catch (err) {
      console.error("Error fetching menus:", err);
    }
  };

  const handleSubmit = async () => {
    const success = await createSession(selectedMenu);
    if (success) {
      setSelectedMenu("");
      onClose();
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth>
      <DialogTitle>
        Create New Session
        <IconButton
          onClick={onClose}
          sx={{ position: "absolute", right: 8, top: 8 }}
        >
          <X />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <FormControl fullWidth sx={{ mt: 2 }}>
          <InputLabel>Select Menu</InputLabel>
          <Select
            value={selectedMenu}
            onChange={(e) => setSelectedMenu(e.target.value)}
            label="Select Menu"
          >
            {availableMenus.map((menu) => (
              <MenuItem key={menu.menu_id} value={menu.menu_id}>
                {menu.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={!selectedMenu}
        >
          Create Session
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default NewSessionDialog;
