import React, { useState, useEffect, useRef } from "react";
import Link from "next/link";
import styles from "../styles/Layout.module.css";
import { useAuth } from "@/hooks/useAuth";
import { logoutUser } from "@/utils/auth";

const Header = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const dropdownContainerRef = useRef(null);

  const { isAuthenticated, userRole } = useAuth();

  const toggleMenu = () => {
    setIsMenuOpen((prev) => !prev);
  };

  const closeMenu = () => {
    setIsMenuOpen(false);
  };

  const handleLogout = async () => {
    await logoutUser();
  };

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownContainerRef.current && !dropdownContainerRef.current.contains(event.target)) {
        closeMenu();
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const renderCustomerNavLinks = () => (
    <div className={styles.navbarLinks}>
      <span className={styles.navbarLinksTitles}>
        <Link href="/" onClick={closeMenu}>
          HOME
        </Link>
      </span>
      <div className={styles.menuDropdownContainer} ref={dropdownContainerRef}>
        <span onClick={toggleMenu} className={styles.navbarLinksTitles}>
          MENUS
        </span>
        {isMenuOpen && (
          <div
            className={`${styles.dropdownMenu} ${styles.dropdownMenuVisible}`}
          >
            <Link href="/menu/drinks" onClick={closeMenu}>
              Drinks
            </Link>
            <Link href="/menu/sashimi" onClick={closeMenu}>
              Sashimi
            </Link>
            <Link href="/menu/nigiri" onClick={closeMenu}>
              Nigiri
            </Link>
            <Link href="/menu/add-on" onClick={closeMenu}>
              Add On
            </Link>
            <Link href="/menu/maki" onClick={closeMenu}>
              Maki
            </Link>
            <Link href="/menu/rolls" onClick={closeMenu}>
              Rolls
            </Link>
            <Link href="/menu/temaki" onClick={closeMenu}>
              Temaki
            </Link>
            <Link href="/menu/hot-stone" onClick={closeMenu}>
              Hot Stone
            </Link>
            <Link href="/menu/cooked-hot" onClick={closeMenu}>
              Cooked/Hot
            </Link>
            <Link href="/menu/kitchen" onClick={closeMenu}>
              Kitchen
            </Link>
            <Link href="/menu/deep-fried" onClick={closeMenu}>
              Deep Fried
            </Link>
            <Link href="/menu/full-menu" onClick={closeMenu}>
              Full Menu
            </Link>
          </div>
        )}
      </div>
      <span className={styles.navbarLinksTitles}>
        <Link href="/bills" onClick={closeMenu}>
          BILLS
        </Link>
      </span>
      <span className={styles.navbarLinksTitles}>
        <Link href="/orders" onClick={closeMenu}>
          ORDERS
        </Link>
      </span>
    </div>
  );

  const renderStaffNavLinks = () => (
    <div className={styles.navbarLinks}>
      <span className={styles.navbarLinksTitles}>
        <Link href="/" onClick={closeMenu}>
          HOME
        </Link>
      </span>
      <span className={styles.navbarLinksTitles}>
        <Link href="/dashboard/sessions" onClick={closeMenu}>
          SESSIONS
        </Link>
      </span>
      <span className={styles.navbarLinksTitles}>
        <Link href="/static/tables.html" onClick={closeMenu}>
          TABLES
        </Link>
      </span>
    </div>
  );

  const renderAdminNavLinks = () => (
    <div className={styles.navbarLinks}>
      <span className={styles.navbarLinksTitles}>
        <Link href="/" onClick={closeMenu}>
          HOME
        </Link>
      </span>
      <span className={styles.navbarLinksTitles}>
        <Link href="/dashboard/sessions" onClick={closeMenu}>
          SESSIONS
        </Link>
      </span>
      <span className={styles.navbarLinksTitles}>
        <Link href="/static/tables.html" onClick={closeMenu}>
          TABLES
        </Link>
      </span>
      <span className={styles.navbarLinksTitles}>
        <Link href="/analytics/analytic-page" onClick={closeMenu}>
          ANALYTICS
        </Link>
      </span>
    </div>
  );

  return (
    <header className={styles.header}>
      <h1 className={styles.headerText} role="heading" aria-level="1">
        Sushi Toshi
      </h1>
      <nav className={styles.navbar} id="main-menu">
        <div className={styles.navbarLogo}>
          <Link href="/" passHref>
            <img
              src="/images/logo.png"
              alt="Sushi Toshi Logo"
              className={styles.logoImage}
              width="60"
              height="60"
            />
          </Link>
        </div>
        <div className={styles.navbarLinksContainer}>
          {isAuthenticated && (
            <>
              {userRole === "customer" && renderCustomerNavLinks()}
              {userRole === "staff" && renderStaffNavLinks()}
              {userRole === "admin" && renderAdminNavLinks()}
            </>
          )}
        </div>

        <div className={styles.navbarRightContainer}>
          <button className={styles.authButton} onClick={handleLogout}>
            {isAuthenticated ? "Logout" : "Login"}
          </button>
        </div>
      </nav>
    </header>
  );
};

export default Header;