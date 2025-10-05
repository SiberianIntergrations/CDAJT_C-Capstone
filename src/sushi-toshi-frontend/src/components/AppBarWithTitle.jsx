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

const AppBarWithTitle = ({ title }) => {
  const router = useRouter();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [userRole, setUserRole] = useState(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  const decodeJWT = (token) => {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      return JSON.parse(
        decodeURIComponent(
          atob(base64)
            .split("")
            .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
            .join("")
        )
      );
    } catch (error) {
      console.error("Error decoding token:", error);
      return null;
    }
  };

  const verifyLogin = () => {
    const token = localStorage.getItem("access_token");
    if (token) {
      const decoded = decodeJWT(token);
      if (decoded?.role) {
        setUserRole(decoded.role);
        setIsLoggedIn(true);
      } else {
        handleLogout();
      }
    } else {
      setIsLoggedIn(false);
      setUserRole(null);
    }
  };

  useEffect(() => {
    verifyLogin();
    const interval = setInterval(verifyLogin, 5000);
    const handleRouteChange = () => {
      verifyLogin();
      setDrawerOpen(false);
    };

    // router.events.on("routeChangeComplete", handleRouteChange);
    window.addEventListener("storage", verifyLogin);

    return () => {
      clearInterval(interval);
      router.events.off("routeChangeComplete", handleRouteChange);
      window.removeEventListener("storage", verifyLogin);
    };
  }, [router.events]);

  const handleLogout = () => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    setIsLoggedIn(false);
    setUserRole(null);
    setDrawerOpen(false);
    router.push("/auth/login");
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
          {isLoggedIn && (
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
              isLoggedIn ? handleLogout : () => handleNavigation("/auth/login")
            }
            startIcon={isLoggedIn ? <LogOut /> : <LogIn />}
            sx={{
              minWidth: { xs: 40, sm: "auto" },
              px: { xs: 1, sm: 2 },
              "& .MuiButton-startIcon": {
                margin: { xs: 0, sm: "auto" },
              },
            }}
          >
            <Typography sx={{ display: { xs: "none", sm: "block" } }}>
              {isLoggedIn ? "Logout" : "Login"}
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
