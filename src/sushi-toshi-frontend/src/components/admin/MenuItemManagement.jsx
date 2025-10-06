import React, { useState, useEffect } from "react";
import {
  Box,
  Button,
  Dialog,
  IconButton,
  Typography,
  CircularProgress,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Stack,
  Card,
  CardMedia,
  Input,
  TextField,
} from "@mui/material";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import {
  Plus,
  Edit,
  Trash2,
  Menu as MenuIcon,
  AlertTriangle,
  Image as ImageIcon,
} from "lucide-react";
import { styled } from "@mui/material/styles";
import MenuAssignmentDialog from "@/components/admin/MenuAssignmentDialog";
import TagChip from "@/components/tags/TagChip";
import TagsDialog from "@/components/admin/TagsDialog";
import CategoryManagementDialog from "@/components/admin/CategoryManagementDialog";
import { axiosInstance, createApiUrl } from "@/config/api";
import axios from "axios";

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

const MenuItemManagement = () => {
  const [items, setItems] = useState([]);
  const [menus, setMenus] = useState([]);
  const [categories, setCategories] = useState([]);
  const [tags, setTags] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [assignmentDialogOpen, setAssignmentDialogOpen] = useState(false);
  const [tagDialogOpen, setTagDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [itemToDelete, setItemToDelete] = useState(null);
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [categoryDialogOpen, setCategoryDialogOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);

  const getToken = () => localStorage.getItem("access_token");

  const fetchData = async () => {
    try {
      console.log("Fetching data...");
      const [
        { data: itemsData },
        { data: categoriesData },
        { data: menusData },
        { data: tagsData },
      ] = await Promise.all([
        axiosInstance.get(createApiUrl("/menu-items")),
        axiosInstance.get(createApiUrl("/categories")),
        axiosInstance.get(createApiUrl("/menus")),
        axiosInstance.get(createApiUrl("/tag/colors")),
      ]);

      setItems(Array.isArray(itemsData) ? itemsData : []);
      setCategories(Array.isArray(categoriesData) ? categoriesData : []);
      setMenus(Array.isArray(menusData) ? menusData : []);
      setTags(Array.isArray(tagsData) ? tagsData : []);
    } catch (err) {
      console.error("Failed to fetch data:", err);
      setError("Failed to fetch data");
      setItems([]);
      setCategories([]);
      setMenus([]);
      setTags([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    console.log("Items updated:", items);
    console.log("Categories updated:", categories);
    console.log("Menus updated:", menus);
    console.log("Tags updated:", tags);
  }, [items, categories, tags, menus]);

  useEffect(() => {
    fetchData();
  }, []);

  const handleTagAction = async (itemId, tagId, action) => {
    try {
      console.log(`Performing tag ${action} action...`);
      if (action === "add") {
        await axiosInstance.post(
          createApiUrl(`/menu-items/${itemId}/tags/${tagId}`)
        );
      } else {
        await axiosInstance.delete(
          createApiUrl(`/menu-items/${itemId}/tags/${tagId}`)
        );
      }
      await fetchData();
    } catch (err) {
      console.error(`Error ${action}ing tag:`, err);
      setError(`Failed to ${action} tag`);
    }
  };

  const handleCreate = () => {
    setSelectedItem(null);
    setImageFile(null);
    setImagePreview("");
    setDialogOpen(true);
  };

  const handleEdit = (item) => {
    setSelectedItem(item);
    setImagePreview(item.item_image_url || "");
    setDialogOpen(true);
  };

  const handleDelete = async (item) => {
    try {
      const response = await axiosInstance.get(
        createApiUrl(`/menu-item-assignments/by-item/${item.item_id}`)
      );
      console.log("Checking item assignments (api)...");
      const assignments = response.data;
      console.log("Assignments (api):", assignments);

      if (assignments.length > 0) {
        setError(
          "Cannot delete item - it is currently assigned to one or more menus. Remove menu assignments first."
        );
        return;
      }

      setItemToDelete(item);
      setDeleteDialogOpen(true);
    } catch (err) {
      setError("Failed to check item assignments");
    }
  };

  const handleDeleteConfirm = async () => {
    try {
      console.log("Deleting item...", itemToDelete);
      const response = axiosInstance.delete(
        createApiUrl(`/menu-items/${itemToDelete.item_id}`)
      );
      await fetchData();
      setDeleteDialogOpen(false);
      setItemToDelete(null);
    } catch (err) {
      setError("Failed to delete menu item");
    }
  };

  const columns = [
    {
      field: "item_image_url",
      headerName: "Image",
      width: 100,
      renderCell: (params) =>
        params.value ? (
          <Box
            component="img"
            src={params.value}
            alt={params.row.name}
            sx={{
              width: 60,
              height: 60,
              objectFit: "cover",
              borderRadius: 1,
            }}
          />
        ) : (
          <Box
            sx={{
              width: 60,
              height: 60,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              bgcolor: "grey.100",
              borderRadius: 1,
            }}
          >
            <ImageIcon size={24} color="grey" />
          </Box>
        ),
      sortable: false,
      filterable: false,
    },
    {
      field: "name",
      headerName: "Name",
      flex: 1,
      minWidth: 200,
    },
    {
      field: "category",
      headerName: "Category",
      flex: 1,
      minWidth: 150,
      valueGetter: (params) => {
        const category = categories.find(
          (c) => c.category_id === params.row.category_id
        );
        return category?.name || "";
      },
    },
    {
      field: "description",
      headerName: "Description & Tags",
      flex: 2,
      minWidth: 300,
      renderCell: (params) => (
        <Box>
          <Typography variant="body2" sx={{ mb: 1 }}>
            {params.row.description}
          </Typography>
          <Box
            sx={{
              display: "flex",
              flexWrap: "wrap",
              gap: 0.5,
              alignItems: "center",
            }}
          >
            {params.row.tags?.map((tag) => (
              <TagChip
                key={`tag-${tag.tag_id}`}
                tag={tag}
                isActionChip={true}
                isUsedFor="menu-item"
                onToggle={() =>
                  handleTagAction(params.row.item_id, tag.tag_id, "remove")
                }
              />
            ))}
            <IconButton
              size="small"
              onClick={() => {
                setSelectedItem(params.row);
                setTagDialogOpen(true);
              }}
            >
              <Plus size={16} />
            </IconButton>
          </Box>
        </Box>
      ),
    },
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
            onClick={() => {
              setSelectedItem(params.row);
              setAssignmentDialogOpen(true);
            }}
            size="small"
          >
            <MenuIcon size={20} />
          </IconButton>
          <IconButton
            onClick={() => handleDelete(params.row)}
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

  const ItemForm = () => {
    const [formData, setFormData] = useState({
      name: selectedItem?.name || "",
      description: selectedItem?.description || "",
      category_id: selectedItem?.category_id || "",
    });

    const handleSubmit = async (e) => {
      e.preventDefault();
      setSubmitting(true);

      try {
        // Save menu item basic data
        const url = selectedItem
          ? `/menu-items/${selectedItem.item_id}`
          : "/menu-items";

        const savedItem = await axiosInstance({
          method: selectedItem ? "PUT" : "POST",
          url,
          data: formData,
        });

        // Handle image upload if there is one
        if (imageFile) {
          const formData = new FormData();
          formData.append("file", imageFile);

          await axiosInstance({
            method: "POST",
            url: `/menu-items/${savedItem.data.item_id}/image`,
            data: formData,
            headers: {
              "Content-Type": "multipart/form-data",
            },
          });
        }

        await fetchData();
        setDialogOpen(false);
      } catch (err) {
        setError(err.response?.data?.detail || err.message);
      } finally {
        setSubmitting(false);
      }
    };

    return (
      <Dialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <Box component="form" onSubmit={handleSubmit} sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            {selectedItem ? "Edit Menu Item" : "Create Menu Item"}
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          <Stack spacing={3}>
            <Card
              sx={{
                aspectRatio: "16/9",
                overflow: "hidden",
                position: "relative",
              }}
            >
              {imagePreview ? (
                <CardMedia
                  component="img"
                  image={imagePreview}
                  alt="Item preview"
                  sx={{ height: "100%", objectFit: "cover" }}
                />
              ) : (
                <Box
                  sx={{
                    height: "100%",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    bgcolor: "grey.100",
                  }}
                >
                  <ImageIcon size={48} color="grey" />
                </Box>
              )}
              <Input
                type="file"
                accept="image/*"
                onChange={(e) => {
                  const file = e.target.files[0];
                  if (file) {
                    setImageFile(file);
                    const reader = new FileReader();
                    reader.onloadend = () => {
                      setImagePreview(reader.result);
                    };
                    reader.readAsDataURL(file);
                  }
                }}
                sx={{ display: "none" }}
                id="image-input"
              />
              <label htmlFor="image-input">
                <Button
                  component="span"
                  variant="contained"
                  sx={{
                    position: "absolute",
                    bottom: 16,
                    right: 16,
                    bgcolor: "rgba(0, 0, 0, 0.5)",
                    "&:hover": {
                      bgcolor: "rgba(0, 0, 0, 0.7)",
                    },
                  }}
                >
                  Change Image
                </Button>
              </label>
            </Card>

            <TextField
              label="Name"
              value={formData.name}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, name: e.target.value }))
              }
              required
              fullWidth
            />

            <TextField
              label="Description"
              value={formData.description}
              onChange={(e) =>
                setFormData((prev) => ({
                  ...prev,
                  description: e.target.value,
                }))
              }
              multiline
              rows={3}
              fullWidth
            />

            <FormControl fullWidth required>
              <InputLabel>Category</InputLabel>
              <Select
                value={formData.category_id}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    category_id: e.target.value,
                  }))
                }
                label="Category"
              >
                {categories.map((category) => (
                  <MenuItem
                    key={category.category_id}
                    value={category.category_id}
                  >
                    {category.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <Box sx={{ display: "flex", justifyContent: "flex-end", gap: 1 }}>
              <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" variant="contained" disabled={submitting}>
                {submitting ? <CircularProgress size={24} /> : "Save"}
              </Button>
            </Box>
          </Stack>
        </Box>
      </Dialog>
    );
  };

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="50vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Stack
        direction="row"
        justifyContent="left"
        alignItems="center"
        mb={3}
        spacing={2}
      >
        <Typography variant="h4">Menu Items</Typography>
        <AddButton onClick={handleCreate}>
          <Plus />
        </AddButton>
      </Stack>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <Box sx={{ width: "100%", height: 600 }}>
        <DataGrid
          rows={items}
          columns={[
            {
              field: "item_image_url",
              headerName: "Image",
              width: 100,
              renderCell: (params) =>
                params.value ? (
                  <Box
                    component="img"
                    src={params.value}
                    alt={params.row.name}
                    sx={{
                      width: 60,
                      height: 60,
                      objectFit: "cover",
                      borderRadius: 1,
                    }}
                  />
                ) : (
                  <Box
                    sx={{
                      width: 60,
                      height: 60,
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      bgcolor: "grey.100",
                      borderRadius: 1,
                    }}
                  >
                    <ImageIcon size={24} color="grey" />
                  </Box>
                ),
              sortable: false,
              filterable: false,
            },
            {
              field: "name",
              headerName: "Name",
              flex: 1,
              minWidth: 200,
              align: "center",
              headerAlign: "left",
              renderCell: (params) => (
                <Box
                  sx={{ display: "flex", alignItems: "center", height: "100%" }}
                >
                  <Typography>{params.value}</Typography>
                </Box>
              ),
            },
            {
              field: "category_id",
              headerName: "Category",
              flex: 1,
              minWidth: 150,
              align: "center",
              headerAlign: "left",
              type: "singleSelect",
              valueOptions: [
                { value: "", label: "All Categories" },
                ...categories.map((cat) => ({
                  value: cat.category_id,
                  label: cat.name,
                })),
              ],
              renderCell: (params) => {
                const category = categories.find(
                  (c) => c.category_id === params.value
                );
                return (
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      height: "100%",
                    }}
                  >
                    <Typography>{category?.name || "(No category)"}</Typography>
                  </Box>
                );
              },
            },
            {
              field: "description",
              headerName: "Description & Tags",
              flex: 2,
              minWidth: 300,
              renderCell: (params) => (
                <Box sx={{ py: 1 }}>
                  <Typography variant="body2" sx={{ mb: 1 }}>
                    {params.row.description}
                  </Typography>
                  <Box
                    sx={{
                      display: "flex",
                      flexWrap: "wrap",
                      gap: 0.5,
                      alignItems: "center",
                    }}
                  >
                    {params.row.tags?.map((tag) => (
                      <TagChip
                        key={`tag-${tag.tag_id}`}
                        tag={tag}
                        isActionChip={true}
                        isUsedFor="menu-item"
                        onToggle={() =>
                          handleTagAction(
                            params.row.item_id,
                            tag.tag_id,
                            "remove"
                          )
                        }
                      />
                    ))}
                    <IconButton
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        setSelectedItem(params.row);
                        setTagDialogOpen(true);
                      }}
                    >
                      <Plus size={16} />
                    </IconButton>
                  </Box>
                </Box>
              ),
            },
            {
              field: "actions",
              headerName: "Actions",
              width: 150,
              align: "center",
              headerAlign: "left",
              renderCell: (params) => (
                <Box
                  sx={{ display: "flex", alignItems: "center", height: "100%" }}
                >
                  <Stack direction="row" spacing={1}>
                    <IconButton
                      onClick={(e) => {
                        e.stopPropagation();
                        handleEdit(params.row);
                      }}
                      size="small"
                    >
                      <Edit size={20} />
                    </IconButton>
                    <IconButton
                      onClick={(e) => {
                        e.stopPropagation();
                        setSelectedItem(params.row);
                        setAssignmentDialogOpen(true);
                      }}
                      size="small"
                    >
                      <MenuIcon size={20} />
                    </IconButton>
                    <IconButton
                      onClick={(e) => {
                        e.stopPropagation();
                        handleDelete(params.row);
                      }}
                      size="small"
                      color="error"
                    >
                      <Trash2 size={20} />
                    </IconButton>
                  </Stack>
                </Box>
              ),
            },
          ]}
          getRowId={(row) => row.item_id}
          getRowHeight={() => "auto"}
          autoHeight
          density="comfortable"
          disableRowSelectionOnClick
          slots={{
            toolbar: GridToolbar,
          }}
          slotProps={{
            toolbar: {
              showQuickFilter: true,
            },
          }}
          initialState={{
            pagination: {
              paginationModel: { pageSize: 10 },
            },
          }}
          pageSizeOptions={[10, 25, 50]}
          sx={{
            "& .MuiDataGrid-cell:focus": {
              outline: "none",
            },
            "& .MuiDataGrid-row": {
              minHeight: "100px",
            },
          }}
        />
      </Box>

      <ItemForm />

      <TagsDialog
        open={tagDialogOpen}
        onClose={() => setTagDialogOpen(false)}
        selectedItem={selectedItem}
        tags={tags}
        onTagAction={handleTagAction}
      />

      <MenuAssignmentDialog
        open={assignmentDialogOpen}
        onClose={() => setAssignmentDialogOpen(false)}
        selectedItem={selectedItem}
        menus={menus}
        onSuccess={fetchData}
      />

      <Dialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
      >
        <Box sx={{ p: 3 }}>
          <Stack direction="row" spacing={1} alignItems="center" mb={2}>
            <AlertTriangle color="error" />
            <Typography variant="h6">Confirm Delete</Typography>
          </Stack>
          <Typography mb={3}>
            Are you sure you want to delete "{itemToDelete?.name}"? This action
            cannot be undone.
          </Typography>
          <Stack direction="row" spacing={1} justifyContent="flex-end">
            <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
            <Button
              variant="contained"
              color="error"
              onClick={handleDeleteConfirm}
            >
              Delete
            </Button>
          </Stack>
        </Box>
      </Dialog>

      <CategoryManagementDialog
        open={categoryDialogOpen}
        onClose={() => setCategoryDialogOpen(false)}
        selectedCategory={selectedCategory}
        onSuccess={fetchData}
      />
    </Box>
  );
};

export default MenuItemManagement;
