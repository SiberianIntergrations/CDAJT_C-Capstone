import React, { useState } from "react";
import CategoryManagementDialog from "@/components/admin/CategoryManagementDialog";
import { http, HttpResponse, delay } from "msw";
import api from "@/config/api";

const mockCategory = {
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
};

// Mock api methods for Storybook
api.put = async (url, data) => {
  if (url.startsWith("/api/Category/update_category/")) {
    return { data: { ...mockCategory, ...data } };
  }
  return { data: {} };
};
api.post = async (url, data) => {
  if (url === "/api/Category") {
    return { data: { ...mockCategory, ...data, category_id: 999 } };
  }
  return { data: {} };
};
api.delete = async (url) => {
  if (url.startsWith("/api/Category/delete_category/")) {
    return { data: { success: true } };
  }
  return { data: {} };
};
api.get = async (url) => {
  if (url.startsWith("/api/Category/get_category/")) {
    return { data: mockCategory };
  }
  return { data: {} };
};

export default {
  title: "Admin/CategoryManagementDialog",
  component: CategoryManagementDialog,
  parameters: {
    msw: [
      http.get("/api/Category/get_category/:categoryId", async ({ params }) => {
        await delay(200);
        return HttpResponse.json(mockCategory, { status: 200 });
      }),
      http.put("/api/Category/update_category/:categoryId", async () => {
        await delay(200);
        return HttpResponse.json({ ...mockCategory }, { status: 200 });
      }),
      http.post("/api/Category", async () => {
        await delay(200);
        return HttpResponse.json({ ...mockCategory, category_id: 999 }, { status: 201 });
      }),
      http.delete("/api/Category/delete_category/:categoryId", async () => {
        await delay(200);
        return HttpResponse.json({ success: true }, { status: 200 });
      }),
    ],
  },
};

export const EditCategory = (args) => {
  const [open, setOpen] = useState(true);
  return (
    <CategoryManagementDialog
      open={open}
      onClose={() => setOpen(false)}
      selectedCategory={{
        category_id: mockCategory.category_id,
        name: mockCategory.category_name,
        description: mockCategory.description,
        adult_limit: mockCategory.adult_limit,
        child_limit: mockCategory.child_limit,
        senior_limit: mockCategory.senior_limit,
        tot_limit: mockCategory.total_limit,
      }}
      onSuccess={() => setOpen(false)}
      {...args}
    />
  );
};

export const NewCategory = (args) => {
  const [open, setOpen] = useState(true);
  return (
    <CategoryManagementDialog
      open={open}
      onClose={() => setOpen(false)}
      selectedCategory={null}
      onSuccess={() => setOpen(false)}
      {...args}
    />
  );
};
