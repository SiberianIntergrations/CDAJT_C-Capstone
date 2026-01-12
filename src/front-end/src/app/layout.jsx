"use client";

import { Geist, Geist_Mono } from "next/font/google";
import "@/app/globals.css";

import { ThemeProvider, CssBaseline } from "@mui/material";
import { createTheme } from "@mui/material/styles";
import { MenuProvider } from "@/contexts/MenuContext";
import Layout from "@/components/Layout.jsx";
import "@/styles/global.css";

import { useEffect, useState, createContext } from "react";

import { extractRoles, mapToAppRole } from '@/config/auth';

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

const theme = createTheme({
  palette: {
    primary: {
      main: "#C01E2E",
    },
    secondary: {
      main: "#14171a",
    },
    text: {
      primary: "#000000",
      secondary: "#657786",
    },
    background: {
      default: "#ffffff",
      paper: "#ffffff",
    },
  },
  typography: {
    fontFamily: "'Open Sans', sans-serif",
    h1: {
      fontFamily: "'Montserrat', sans-serif",
      fontWeight: 600,
    },
    h2: {
      fontFamily: "'Montserrat', sans-serif",
      fontWeight: 600,
    },
    h3: {
      fontFamily: "'Montserrat', sans-serif",
      fontWeight: 600,
    },
    h4: {
      fontFamily: "'Montserrat', sans-serif",
      fontWeight: 600,
    },
    h5: {
      fontFamily: "'Montserrat', sans-serif",
      fontWeight: 600,
    },
    h6: {
      fontFamily: "'Montserrat', sans-serif",
      fontWeight: 600,
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: "none",
          borderRadius: "4px",
        },
      },
    },
    MuiDialog: {
      styleOverrides: {
        paper: {
          borderRadius: 8,
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: "#C01E2E",
        },
      },
    },
  },
});

export const AuthContext = createContext();

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable}`}>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <MenuProvider>
            <Layout>{children}</Layout>
          </MenuProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}