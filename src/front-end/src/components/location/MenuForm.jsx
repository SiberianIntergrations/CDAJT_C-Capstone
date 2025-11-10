import { useState } from "react";
import {
  Box,
  TextField,
  Button,
  DialogTitle,
  DialogContent,
  DialogActions,
  Stack,
  FormControl,
  FormControlLabel,
  Switch
} from "@mui/material";

const MenuForm = ({ initialData, onSubmit, onClose, mode }) => {
  const [formData, setFormData] = useState({
    name: initialData?.name || "",
    description: initialData?.description || "",
    start_time: initialData?.start_time || "",
    end_time: initialData?.end_time || "",
    is_active: initialData?.is_active || false,
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      <DialogTitle>
        {mode === "create" ? "Add New Menu" : "Edit Menu"}
      </DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 2 }}>
          <TextField
            name="name"
            label="Menu Name"
            value={formData.name}
            onChange={handleChange}
            required
            fullWidth
          />
          <TextField
            name="description"
            label="Description"
            value={formData.description}
            onChange={handleChange}
            fullWidth
            multiline
            rows={3}
          />
          <TextField
            name="start_time"
            label="Start Time"
            type="time"
            value={formData.start_time}
            onChange={handleChange}
            required
            InputLabelProps={{ shrink: true }}
            fullWidth
          />
          <TextField
            name="end_time"
            label="End Time"
            type="time"
            value={formData.end_time}
            onChange={handleChange}
            required
            InputLabelProps={{ shrink: true }}
            fullWidth
          />
          <FormControlLabel
            control={
              <Switch
                checked={formData.is_active === true}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    is_active: e.target.checked,
                  }))
                }
              />
            }
            label="Is Active Menu"
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button type="submit" variant="contained">
          {mode === "create" ? "Create" : "Save"}
        </Button>
      </DialogActions>
    </form>
  );
};

export default MenuForm;
