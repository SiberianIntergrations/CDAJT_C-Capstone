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
  Select,
  MenuItem,
  FormControl,
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
  Table,
} from "lucide-react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { useAuth } from "@/hooks/useAuth";
import { logoutUser } from "@/utils/auth";
import api from "@/config/api";
import storage from "@/utils/storage";
import { LocationOn } from "@mui/icons-material";

const AppBarWithTitle = ({ title }) => {
  const router = useRouter();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedLocation, setSelectedLocation] = useState(
    storage.get("branch-location")
  );
  const [allLocations, setAllLocations] = useState([]);

  const { isAuthenticated, userRole, loading } = useAuth();

  useEffect(() => {
    const fetchAllLocations = async () => {
      try {
        if (
          !isAuthenticated ||
          !userRole ||
          (userRole !== "staff" && userRole !== "admin")
        ) {
          return;
        }
        const response = await api.get("location/");
        setAllLocations(response.data);
        // const locationResponse = await api.get(
        //   "staff/get-initial-staff-location"
        // );
        // setSelectedLocation(locationResponse.data.location_Id);
      } catch (err) {
        console.error("Error Fetching Locations", err);
      }
    };
    fetchAllLocations();
  }, [isAuthenticated, userRole, loading]);

  const handleLogout = async () => {
    await logoutUser();
    setDrawerOpen(false);
  };

  const handleNavigation = (path) => {
    router.push(path);
    setDrawerOpen(false);
  };

  const handleLocationChange = async (event) => {
    const locationId = event.target.value;
    console.log("Event: ", event.target.value);
    setSelectedLocation(locationId);
    storage.set("branch-location", locationId);
    window.location.reload();

    try {
      // const response = await api.put(
      // `staff/update-user-location/${locationId}`
      // );
    } catch (err) {
      // console.error("Error updating location:", err);
      // Optionally revert the selection on error
      // setSelectedLocation(previousValue);
    }
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
      { icon: Table, label: "Tables", path: "/dashboard/tables" },
    ],
    admin: [
      { icon: Home, label: "Home", path: "/" },
      { icon: Clock, label: "Sessions", path: "/dashboard/sessions" },
      { icon: Table, label: "Tables", path: "/dashboard/tables" },
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
                cursor: "pointer",
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
          display: "flex",
          justifyContent: "space-between",
          px: { xs: 1, sm: 2 },
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "start",
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
            display: "flex",
            alignItems: "center",
            gap: 2,
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
          sx={{
            flex: "1 1 0",
            display: "flex",
            justifyContent: "flex-end",
            alignItems: "center",
            gap: 2,
          }}
        >
          {/* Location Dropdown - Only visible for staff and admin */}
          {isAuthenticated &&
            (userRole === "staff" || userRole === "admin") && (
              <FormControl size="small">
                <Select
                  value={Number(selectedLocation)}
                  onChange={handleLocationChange}
                  renderValue={(value) => {
                    const location = allLocations?.find(
                      (loc) => loc.location_Id === value
                    );
                    return (
                      <Box
                        sx={{ display: "flex", alignItems: "center", gap: 1 }}
                      >
                        <LocationOn sx={{ color: "white" }} />
                        <Box sx={{ display: { xs: "none", md: "block" } }}>
                          {location?.name || "Select Location"}
                        </Box>
                      </Box>
                    );
                  }}
                  displayEmpty
                  sx={{
                    color: "white",
                    ".MuiOutlinedInput-notchedOutline": {
                      borderColor: "rgba(255, 255, 255, 0.5)",
                    },
                    "&:hover .MuiOutlinedInput-notchedOutline": {
                      borderColor: "rgba(255, 255, 255, 0.7)",
                    },
                    "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                      borderColor: "white",
                    },
                    ".MuiSvgIcon-root": {
                      color: "white",
                    },
                  }}
                >
                  <MenuItem value="" disabled>
                    Select Location
                  </MenuItem>
                  {Array.isArray(allLocations) &&
                    allLocations.map((location) => (
                      <MenuItem
                        key={location.location_Id}
                        value={location.location_Id}
                      >
                        {location.name}
                      </MenuItem>
                    ))}
                </Select>
              </FormControl>
            )}

          <Button
            color="inherit"
            onClick={
              isAuthenticated
                ? handleLogout
                : () => handleNavigation("/auth/login")
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
