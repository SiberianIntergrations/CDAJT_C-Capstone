import React, { useState } from "react";
import CategoryList from "@/components/admin/CategoryList";
import CategoryManagementDialog from "@/components/admin/CategoryManagementDialog";
import { http, HttpResponse, delay } from "msw";
import api from "@/config/api";

const mockMsalInstance = {
  getActiveAccount: () => ({}),
  acquireTokenSilent: async () => {
    return { accessToken: "mock-access-token" };
  }
};

const mockCategories = [
  {
    category_id: 1,
    category_name: "Sushi Rolls",
    description: "Traditional and specialty sushi rolls",
    image_url: null,
    total_views: 0,
    total_view_seconds: 0,
    last_viewed_at: "0001-01-01T00:00:00",
    adult_limit: 12,
    child_limit: 8,
    senior_limit: 10,
    total_limit: 4,
    menuItems: []
  },
  {
    category_id: 2,
    category_name: "Nigiri",
    description: "Hand-pressed sushi with fresh fish",
    image_url: null,
    total_views: 0,
    total_view_seconds: 0,
    last_viewed_at: "0001-01-01T00:00:00",
    adult_limit: 10,
    child_limit: 6,
    senior_limit: 8,
    total_limit: 3,
    menuItems: []
  },
  {
    category_id: 3,
    category_name: "Sashimi",
    description: "Fresh sliced raw fish",
    image_url: null,
    total_views: 0,
    total_view_seconds: 0,
    last_viewed_at: "0001-01-01T00:00:00",
    adult_limit: 8,
    child_limit: 4,
    senior_limit: 6,
    total_limit: 2,
    menuItems: []
  },
  {
    category_id: 4,
    category_name: "Appetizers",
    description: "Starters and small dishes",
    image_url: null,
    total_views: 0,
    total_view_seconds: 0,
    last_viewed_at: "0001-01-01T00:00:00",
    adult_limit: 6,
    child_limit: 4,
    senior_limit: 5,
    total_limit: 2,
    menuItems: []
  },
  {
    category_id: 5,
    category_name: "Hot Dishes",
    description: "Cooked meals and hot specialties",
    image_url: null,
    total_views: 0,
    total_view_seconds: 0,
    last_viewed_at: "0001-01-01T00:00:00",
    adult_limit: 5,
    child_limit: 3,
    senior_limit: 4,
    total_limit: 2,
    menuItems: []
  },
  {
    category_id: 6,
    category_name: "Desserts",
    description: "Sweet treats to end your meal",
    image_url: null,
    total_views: 0,
    total_view_seconds: 0,
    last_viewed_at: "0001-01-01T00:00:00",
    adult_limit: 2,
    child_limit: 2,
    senior_limit: 2,
    total_limit: 1,
    menuItems: []
  }
];

// Mock api methods for Storybook to bypass MSAL and handle category CRUD
api.get = async (url) => {
  if (url === "/category" || url === "/category") {
    return { data: mockCategories };
  }
  if (url.startsWith("/Category/get_category/")) {
    const id = parseInt(url.split("/").pop());
    const found = mockCategories.find(c => c.category_id === id);
    return { data: found || null };
  }
  return { data: [] };
};
api.put = async (url, data) => {
  if (url.startsWith("/Category/update_category/")) {
    const id = parseInt(url.split("/").pop());
    return { data: { ...data, category_id: id } };
  }
  return { data: {} };
};
api.post = async (url, data) => {
  if (url === "/Category") {
    return { data: { ...data, category_id: 999 } };
  }
  return { data: {} };
};
api.delete = async (url) => {
  if (url.startsWith("/Category/delete_category/")) {
    return { data: { success: true } };
  }
  return { data: {} };
};

export default {
  title: "Admin/CategoryList",
  component: CategoryList,
  parameters: {
    msw: [
      http.get("/category", async () => {
        await delay(200);
        return HttpResponse.json(mockCategories, { status: 200 });
      }),
      http.get("/Category/get_category/:categoryId", async ({ params }) => {
        await delay(200);
        const found = mockCategories.find(c => c.category_id === parseInt(params.categoryId));
        return HttpResponse.json(found || {}, { status: 200 });
      }),
      http.put("/Category/update_category/:categoryId", async ({ request, params }) => {
        await delay(200);
        const body = await request.json();
        return HttpResponse.json({ ...body, category_id: parseInt(params.categoryId) }, { status: 200 });
      }),
      http.post("/Category", async ({ request }) => {
        await delay(200);
        const body = await request.json();
        return HttpResponse.json({ ...body, category_id: 999 }, { status: 201 });
      }),
      http.delete("/Category/delete_category/:categoryId", async () => {
        await delay(200);
        return HttpResponse.json({ success: true }, { status: 200 });
      }),
    ],
  },
};

export const Default = (args) => {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);

  const handleEditCategory = (category) => {
    // Map fields for CategoryManagementDialog
    setSelectedCategory({
      ...category,
      name: category.category_name,
      tot_limit: category.total_limit,
    });
    setDialogOpen(true);
  };

  const handleCreateCategory = () => {
    setSelectedCategory(null);
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedCategory(null);
  };

  return (
    <>
      <CategoryList
        onEditCategory={handleEditCategory}
        onCreateCategory={handleCreateCategory}
        {...args}
      />
      <CategoryManagementDialog
        open={dialogOpen}
        onClose={handleDialogClose}
        selectedCategory={selectedCategory}
        onSuccess={handleDialogClose}
      />
    </>
  );
};

