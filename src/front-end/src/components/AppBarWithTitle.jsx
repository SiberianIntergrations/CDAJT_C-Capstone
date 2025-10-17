"use client";
import { useState, useEffect } from "react";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Box,
  useTheme,
  useMediaQuery,
  CircularProgress,
} from "@mui/material";
import {
  Menu as MenuIcon,
  LogOut,
  LogIn,
  Home,
  Clock,
  Users,
  BarChart,
  Tag,
  Phone,
  MapPin,
} from "lucide-react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { useAuth } from "@/hooks/useAuth";
import { logoutUser } from "@/utils/auth";

const AppBarWithTitle = ({ title }) => {
  const router = useRouter();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const [drawerOpen, setDrawerOpen] = useState(false);

  const { isAuthenticated, userRole, loading } = useAuth();

  const handleLogout = async () => {
    await logoutUser();
    setDrawerOpen(false);
  };

  const handleNavigation = (path) => {
    router.push(path);
    setDrawerOpen(false);
  };

  const menuItems = {
    customer: [
      { icon: Home, label: "Home", path: "/" },
      { icon: Clock, label: "Bills", path: "/dashboard/bills" },
      { icon: MenuIcon, label: "Menu", path: "/menu/full-menu" },
      { icon: Users, label: "Orders", path: "/dashboard/orders" },
      { icon: Phone, label: "Call Server", path: "/dashboard/call-server" },
    ],
    staff: [
      { icon: Home, label: "Home", path: "/" },
      { icon: Clock, label: "Sessions", path: "/dashboard/sessions" },
    ],
    admin: [
      { icon: Home, label: "Home", path: "/" },
      { icon: Clock, label: "Sessions", path: "/dashboard/sessions" },
      {
        icon: MapPin,
        label: "Manage Locations",
        path: "/location/menu-location",
      },
      { icon: MenuIcon, label: "Manage Menu Items", path: "/admin/menu-items" },
      { icon: Users, label: "Manage Staff", path: "/admin/staff" },
      { icon: Tag, label: "Manage Tags", path: "/tags/tag-management" },
      { icon: BarChart, label: "Analytics", path: "/analytics/analytic-page" },
    ],
  };

  const renderMenuList = () => (
    <List
      sx={{
        width: 320,
        pt: 3,
      }}
    >
      {userRole &&
        menuItems[userRole].map((item, index) => {
          const Icon = item.icon;
          return (
            <ListItem
              button
              key={index}
              onClick={() => handleNavigation(item.path)}
              sx={{
                py: 3,
                "& .MuiListItemIcon-root": {
                  minWidth: 56,
                  "& svg": {
                    width: 24,
                    height: 24,
                  },
                },
                "& .MuiListItemText-primary": {
                  fontSize: "1.1rem",
                  fontWeight: 500,
                },
              }}
            >
              <ListItemIcon>
                <Icon />
              </ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItem>
          );
        })}
    </List>
  );

  if (loading) return <CircularProgress />;

  return (
    <AppBar position="fixed" sx={{ zIndex: theme.zIndex.drawer + 1 }}>
      <Toolbar
        sx={{
          minHeight: { xs: 56, sm: 64 },
          justifyContent: "space-between",
          px: { xs: 1, sm: 2 },
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1,
            flex: "1 1 0",
          }}
        >
          {isAuthenticated && (
            <IconButton
              color="inherit"
              aria-label="menu"
              onClick={() => setDrawerOpen(true)}
              sx={{ p: { xs: 1, sm: 1.5 } }}
            >
              <MenuIcon />
            </IconButton>
          )}
          <Typography
            variant="h6"
            component="div"
            sx={{
              fontSize: { xs: "1rem", sm: "1.25rem" },
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
          >
            {title}
          </Typography>
        </Box>

        <Box
          sx={{
            position: "absolute",
            left: "50%",
            top: "50%",
            transform: "translate(-50%, -50%)",
          }}
        >
          <Image
            src="/images/logo.png"
            alt="Logo"
            width={isMobile ? 80 : 115}
            height={isMobile ? 48 : 70}
            style={{
              objectFit: "contain",
              padding: isMobile ? "2px" : "4px",
            }}
            priority
          />
        </Box>

        <Box
          sx={{ flex: "1 1 0", display: "flex", justifyContent: "flex-end" }}
        >
          <Button
            color="inherit"
            onClick={
              isAuthenticated ? handleLogout : () => handleNavigation("/auth/login")
            }
            startIcon={isAuthenticated ? <LogOut /> : <LogIn />}
            sx={{
              minWidth: { xs: 40, sm: "auto" },
              px: { xs: 1, sm: 2 },
              "& .MuiButton-startIcon": {
                margin: { xs: 0, sm: "auto" },
              },
            }}
          >
            <Typography sx={{ display: { xs: "none", sm: "block" } }}>
              {isAuthenticated ? "Logout" : "Login"}
            </Typography>
          </Button>
        </Box>

        <Drawer
          anchor="left"
          open={drawerOpen}
          onClose={() => setDrawerOpen(false)}
          sx={{
            "& .MuiDrawer-paper": {
              top: { xs: 56, sm: 64 },
            },
          }}
        >
          {renderMenuList()}
        </Drawer>
      </Toolbar>
    </AppBar>
  );
};

export default AppBarWithTitle;
