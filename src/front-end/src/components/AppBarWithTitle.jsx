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

// MSAL imports
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "@/config/auth"; // Ensure this path matches your project structure

const AppBarWithTitle = ({ title }) => {
  const router = useRouter();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [userRole, setUserRole] = useState(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [activeAccount, setActiveAccount] = useState(null);
  const msalInstance = useMsal();

  // Helper function to map backend roles to frontend roles
  const mapBackendRoleToFrontend = (roles) => {
    // roles can be a string or array
    const roleList = Array.isArray(roles) ? roles : [roles];
    if (roleList.includes("user.Admin")) return "admin";
    if (roleList.includes("user.Staff")) return "staff";
    // Default to customer if no match
    return "customer";
  };

  useEffect(() => {
    // Get accounts from MSAL context
    const accounts = msalInstance.accounts;
    const account = accounts && accounts.length > 0 ? accounts[0] : null;
    if (account) {
      msalInstance.instance.setActiveAccount(account);
      setActiveAccount(account);
      setIsLoggedIn(true);
      msalInstance.instance.acquireTokenSilent({ ...loginRequest, account }).then((response) => {
        console.log("Active account claims:", response.idTokenClaims);
        // Extract roles from claims (array or string)
        const roles = response.idTokenClaims?.roles || response.idTokenClaims?.role;
        const frontendRole = mapBackendRoleToFrontend(roles);
        setUserRole(frontendRole);
      }).catch(() => {
        setIsLoggedIn(false);
        setUserRole(null);
      });
    } else {
      setIsLoggedIn(false);
      setUserRole(null);
      setActiveAccount(null);
    }
    // ...existing cleanup logic...
  }, [msalInstance.accounts]);

  const handleLogin = async () => {
    try {
      const loginResponse = await msalInstance.instance.loginPopup(loginRequest);
      msalInstance.instance.setActiveAccount(loginResponse.account);
      setActiveAccount(loginResponse.account);
      setIsLoggedIn(true);
      // Print claims to console
      console.log("Active account claims:", loginResponse.idTokenClaims);
      // Extract roles from claims (array or string)
      const roles = loginResponse.idTokenClaims?.roles || loginResponse.idTokenClaims?.role;
      const frontendRole = mapBackendRoleToFrontend(roles);
      setUserRole(frontendRole);
    } catch (error) {
      console.error("MSAL login error:", error);
    }
  };

  const handleLogout = () => {
    msalInstance.instance.logoutPopup();
    setIsLoggedIn(false);
    setUserRole(null);
    setDrawerOpen(false);
    setActiveAccount(null);
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
              activeAccount ? handleLogout : handleLogin
            }
            startIcon={activeAccount ? <LogOut /> : <LogIn />}
            sx={{
              minWidth: { xs: 40, sm: "auto" },
              px: { xs: 1, sm: 2 },
              "& .MuiButton-startIcon": {
                margin: { xs: 0, sm: "auto" },
              },
            }}
          >
            <Typography sx={{ display: { xs: "none", sm: "block" } }}>
              {activeAccount ? "Logout" : "Login"}
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
