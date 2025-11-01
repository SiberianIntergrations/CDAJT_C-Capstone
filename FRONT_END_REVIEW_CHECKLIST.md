# Front-End Components and Pages Review Checklist

**Project:** CDAJT C-Capstone - Sushi Toshi Restaurant Management System
**Last Updated:** 2025-10-30
**Status:** Ready for systematic review and testing

---

## Overview

This checklist covers **79+ components and pages** across the front-end application. Use this document to systematically review, test, and verify all functionality is working correctly.

**Legend:**
- ⬜ Not Started
- 🔄 In Progress
- ✅ Completed & Working
- ⚠️ Issues Found (needs fixing)
- ❌ Not Working
- 🚧 Under Development
- 📝 Needs Documentation

---

## 🔐 AUTHENTICATION SYSTEM (5 Components + 4 Pages)

### Auth Pages

- [ ] **Login Page** - [src/app/auth/login/page.jsx](src/app/auth/login/page.jsx)
  - [ ] Page loads without errors
  - [ ] LoginForm component renders
  - [ ] Proper routing from root or other pages

- [ ] **Register Page** - [src/app/auth/register/page.jsx](src/app/auth/register/page.jsx)
  - [ ] Page loads without errors
  - [ ] RegisterForm component renders
  - [ ] Navigation from login page works

- [ ] **Forgot Password Page** - [src/app/auth/forgot-password/page.jsx](src/app/auth/forgot-password/page.jsx)
  - [ ] Page loads without errors
  - [ ] ForgotPasswordForm component renders
  - [ ] Navigation from login page works

- [ ] **Reset Password Page** - [src/app/auth/reset-password/page.jsx](src/app/auth/reset-password/page.jsx)
  - [ ] Page loads with token parameter
  - [ ] ResetPasswordForm component renders
  - [ ] Token extraction from URL works

### Auth Components

- [ ] **LoginForm** - [src/components/auth/LoginForm.jsx](src/components/auth/LoginForm.jsx)
  - [ ] Email input field works
  - [ ] Password input field works (hidden characters)
  - [ ] Form validation on submit
  - [ ] Invalid credentials show error message
  - [ ] Valid credentials redirect to dashboard
  - [ ] JWT token stored in localStorage
  - [ ] Loading state shows during authentication
  - [ ] "Forgot Password" link navigates correctly
  - [ ] "Register" link navigates correctly
  - [ ] Default credentials pre-filled (dev mode)
  - [ ] Error messages displayed in snackbar/alert

- [ ] **RegisterForm** - [src/components/auth/RegisterForm.jsx](src/components/auth/RegisterForm.jsx)
  - [ ] All fields render (email, password, confirm password, first name, last name)
  - [ ] Email validation (format check)
  - [ ] Password strength requirements enforced
  - [ ] Password confirmation matches
  - [ ] All fields required validation
  - [ ] Successful registration redirects to login
  - [ ] Error handling for duplicate email
  - [ ] Loading state during submission
  - [ ] Success message on registration

- [ ] **ForgotPasswordForm** - [src/components/auth/ForgotPasswordForm.jsx](src/components/auth/ForgotPasswordForm.jsx)
  - [ ] Email input field works
  - [ ] Email validation
  - [ ] Request submission works
  - [ ] Success message displays
  - [ ] Error handling for invalid email
  - [ ] Instructions for checking email displayed

- [ ] **ResetPasswordForm** - [src/components/auth/ResetPasswordForm.jsx](src/components/auth/ResetPasswordForm.jsx)
  - [ ] Token extracted from URL correctly
  - [ ] New password input field works
  - [ ] Confirm password input field works
  - [ ] Password match validation
  - [ ] Password strength requirements enforced
  - [ ] Successful reset redirects to login
  - [ ] Invalid/expired token shows error
  - [ ] Success message displays

### Auth Utilities & Hooks

- [ ] **useAuth Hook** - [src/hooks/useAuth.js](src/hooks/useAuth.js)
  - [ ] isAuthenticated returns correct value
  - [ ] userRole correctly extracted from token
  - [ ] userId correctly extracted from token
  - [ ] userEmail correctly extracted from token
  - [ ] hasRole(role) function works
  - [ ] hasAnyRole(roles) function works
  - [ ] Auto-logout on token expiration
  - [ ] Cross-tab synchronization works
  - [ ] Loading state during initialization
  - [ ] Event listeners clean up properly

- [ ] **auth.js Utilities** - [src/utils/auth.js](src/utils/auth.js)
  - [ ] loginUser() API call works
  - [ ] logoutUser() clears tokens
  - [ ] registerUser() API call works
  - [ ] requestPasswordReset() API call works
  - [ ] resetPassword() API call works
  - [ ] Error handling for network failures
  - [ ] Token stored after successful login

- [ ] **token.js Utilities** - [src/utils/token.js](src/utils/token.js)
  - [ ] getAccessToken() retrieves token
  - [ ] setAuthTokens() stores tokens
  - [ ] clearAuthTokens() removes tokens
  - [ ] getUserRole() decodes correctly
  - [ ] getUserId() decodes correctly
  - [ ] getUserEmail() decodes correctly
  - [ ] isTokenExpired() checks expiration
  - [ ] AUTH_EVENT dispatched correctly

---

## 👨‍💼 ADMIN SYSTEM (12 Components + 3 Pages)

### Admin Pages

- [ ] **Menu Items Management Page** - [src/app/admin/menu-items/page.jsx](src/app/admin/menu-items/page.jsx)
  - [ ] Admin-only access enforced
  - [ ] Redirects non-admin users
  - [ ] MenuItemManagement component loads
  - [ ] Loading state displays
  - [ ] Token validation works

- [ ] **QR Code Management Page** - [src/app/admin/qr-codes/page.jsx](src/app/admin/qr-codes/page.jsx)
  - [ ] Admin/Staff access allowed
  - [ ] Redirects unauthorized users
  - [ ] QRCodeManagement component loads
  - [ ] LocalStorage token check works

- [ ] **Staff Management Page** - [src/app/admin/staff/page.jsx](src/app/admin/staff/page.jsx)
  - [ ] Admin-only access enforced
  - [ ] JWT token verified
  - [ ] StaffManagementPage component loads
  - [ ] Loading state displays
  - [ ] Invalid token redirects

### Admin Components - Menu Items

- [ ] **MenuItemManagement** - [src/components/admin/MenuItemManagement.jsx](src/components/admin/MenuItemManagement.jsx)
  - [ ] DataGrid displays all menu items
  - [ ] Create new menu item button works
  - [ ] Edit menu item opens form with pre-filled data
  - [ ] Delete menu item shows confirmation
  - [ ] Delete removes item from list and database
  - [ ] Image upload functionality works
  - [ ] Image preview displays correctly
  - [ ] Supported image formats (jpg, png, webp) only
  - [ ] Tag assignment opens tag dialog
  - [ ] Tags display with colors on items
  - [ ] Remove tag from item works
  - [ ] Category filter dropdown works
  - [ ] Menu assignment workflow works
  - [ ] DataGrid pagination works
  - [ ] DataGrid sorting works
  - [ ] DataGrid search/filter works
  - [ ] Form validation (name, price, description required)
  - [ ] Price validation (must be positive number)
  - [ ] Add-on toggle works correctly
  - [ ] Success snackbar on create/update/delete
  - [ ] Error snackbar on failures
  - [ ] Loading states during operations

- [ ] **CategoryManagementDialog** - [src/components/admin/CategoryManagementDialog.jsx](src/components/admin/CategoryManagementDialog.jsx)
  - [ ] Dialog opens/closes correctly
  - [ ] Create category form works
  - [ ] Edit category form pre-fills
  - [ ] Form validation works
  - [ ] Submit creates/updates category
  - [ ] Cancel closes without saving

- [ ] **MenuAssignmentDialog** - [src/components/admin/MenuAssignmentDialog.jsx](src/components/admin/MenuAssignmentDialog.jsx)
  - [ ] Dialog opens/closes correctly
  - [ ] Menu dropdown populates
  - [ ] Menu selection works
  - [ ] Assignment submit works
  - [ ] Success callback triggered

- [ ] **TagsDialog** - [src/components/admin/TagsDialog.jsx](src/components/admin/TagsDialog.jsx)
  - [ ] Dialog opens with selected item
  - [ ] Available tags display with colors
  - [ ] Tag selection works
  - [ ] Add tag to item works
  - [ ] Tags saved correctly
  - [ ] Dialog closes on save

### Admin Components - QR Codes

- [ ] **QRCodeManagement** - [src/components/admin/QRCodeManagement.jsx](src/components/admin/QRCodeManagement.jsx)
  - [ ] Location dropdown populates from API
  - [ ] Location selection works
  - [ ] Table number input accepts numbers only
  - [ ] WiFi QR code generation works
  - [ ] Session QR code generation works
  - [ ] QR code preview displays in dialog
  - [ ] Download WiFi QR code works (PNG file)
  - [ ] Download session QR code works (PNG file)
  - [ ] Filename format correct (e.g., WiFi_Location1_Table5.png)
  - [ ] Bulk generation form works
  - [ ] Bulk table count validation (1-1000)
  - [ ] Bulk generation creates correct number of codes
  - [ ] Clear cache button works
  - [ ] Success messages display
  - [ ] Error handling for invalid location
  - [ ] Error handling for invalid table number
  - [ ] Loading states during generation
  - [ ] API calls use correct endpoints
  - [ ] Blob download triggers correctly

### Admin Components - Staff Management

- [ ] **StaffManagementPage** - [src/components/admin/StaffManagementPage.jsx](src/components/admin/StaffManagementPage.jsx)
  - [ ] Staff list displays correctly
  - [ ] Create new staff button opens form
  - [ ] Edit staff button opens form with data
  - [ ] Change password button opens dialog
  - [ ] Toggle status button works
  - [ ] Current user cannot deactivate self
  - [ ] Success notifications display
  - [ ] Error notifications display
  - [ ] Loading states during operations
  - [ ] LocalStorage token retrieval works
  - [ ] Staff data refreshes after operations

- [ ] **StaffList** - [src/components/admin/StaffList.jsx](src/components/admin/StaffList.jsx)
  - [ ] DataGrid renders staff members
  - [ ] Role chips display with correct colors
  - [ ] Status chips display (active/inactive)
  - [ ] Edit button triggers callback
  - [ ] Change Password button triggers callback
  - [ ] Toggle Status button triggers callback
  - [ ] Current user protected from self-deactivation
  - [ ] Tooltips display on action buttons
  - [ ] Empty state displays when no staff
  - [ ] Loading state displays
  - [ ] Error state displays
  - [ ] Sortable columns work
  - [ ] Filterable columns work

- [ ] **StaffManagementForm** - [src/components/admin/StaffManagementForm.jsx](src/components/admin/StaffManagementForm.jsx)
  - [ ] Form renders in create mode
  - [ ] Form renders in edit mode
  - [ ] Email field validation (format)
  - [ ] Email field disabled in edit mode
  - [ ] Password field shows in create mode only
  - [ ] Password validation (8+ characters)
  - [ ] First name field required
  - [ ] Last name field required
  - [ ] Role dropdown populates (staff, admin)
  - [ ] Location dropdown populates from API
  - [ ] Submit button disabled when invalid
  - [ ] Submit triggers create/update callback
  - [ ] Cancel button works
  - [ ] Loading state during location fetch
  - [ ] Form validation errors display

- [ ] **PasswordChangeDialog** - [src/components/admin/PasswordChangeDialog.jsx](src/components/admin/PasswordChangeDialog.jsx)
  - [ ] Dialog opens/closes correctly
  - [ ] Staff member name displays
  - [ ] New password input field works
  - [ ] Password validation (8+ characters)
  - [ ] Submit button triggers callback
  - [ ] Cancel button closes dialog
  - [ ] Loading state during submission
  - [ ] Error message displays
  - [ ] Success closes dialog

---

## 👨‍🍳 STAFF SYSTEM (30+ Components + 6 Pages)

### Staff Pages

- [ ] **Staff Orders Page** - [src/app/staff/orders/page.jsx](src/app/staff/orders/page.jsx)
  - [ ] Page loads without errors
  - [ ] OrderDashboard component renders
  - [ ] Proper routing works

- [ ] **Sessions Dashboard Page** - [src/app/dashboard/sessions/page.jsx](src/app/dashboard/sessions/page.jsx)
  - [ ] Page loads without errors
  - [ ] SessionDashboard component renders
  - [ ] Proper routing works

- [ ] **Tables Dashboard Page** - [src/app/dashboard/tables/page.jsx](src/app/dashboard/tables/page.jsx)
  - [ ] Page loads without errors
  - [ ] TableDashboard component renders
  - [ ] Proper routing works

- [ ] **Orders Dashboard Page** - [src/app/dashboard/orders/page.jsx](src/app/dashboard/orders/page.jsx)
  - [ ] Page loads without errors
  - [ ] Component renders
  - [ ] Order list displays

- [ ] **Bills Dashboard Page** - [src/app/dashboard/bills/page.jsx](src/app/dashboard/bills/page.jsx)
  - [ ] Page loads without errors
  - [ ] Component renders
  - [ ] Bill management interface works

- [ ] **Call Server Page** - [src/app/dashboard/call-server/page.jsx](src/app/dashboard/call-server/page.jsx)
  - [ ] Page loads without errors
  - [ ] Service requests display
  - [ ] Real-time updates work

### Staff Components - Order Management

- [ ] **OrderDashboard (Staff)** - [src/components/staff/OrderDashboard.jsx](src/components/staff/OrderDashboard.jsx)
  - [ ] Pending orders load on mount
  - [ ] Auto-refresh every 30 seconds works
  - [ ] Orders grouped by status (PENDING, APPROVED, DELIVERED)
  - [ ] Quantity editing for pending orders works
  - [ ] Increment/decrement buttons work
  - [ ] Remove item from order works
  - [ ] Removal confirmation dialog displays
  - [ ] Approve order button works
  - [ ] Approve sends updated quantities to API
  - [ ] Mark as delivered button works (approved orders)
  - [ ] Status chips color-coded correctly
  - [ ] Empty state displays when no orders
  - [ ] Loading state displays during fetch
  - [ ] Error handling for failed API calls
  - [ ] Success messages display
  - [ ] Order list refreshes after actions

### Staff Components - Session Management

- [ ] **SessionDashboard (Main)** - [src/components/staff/SessionDashboard/index.jsx](src/components/staff/SessionDashboard/index.jsx)
  - [ ] SessionProvider wraps components
  - [ ] Auto-refresh every 30 seconds works
  - [ ] Active session count displays
  - [ ] Visual alert (page blink) for pending orders works
  - [ ] Context data propagates to children
  - [ ] Loading state on initial load
  - [ ] Error handling for fetch failures

- [ ] **DashboardSummary** - [src/components/staff/SessionDashboard/components/DashboardSummary.jsx](src/components/staff/SessionDashboard/components/DashboardSummary.jsx)
  - [ ] Active session count accurate
  - [ ] "New Session" button opens dialog
  - [ ] Summary statistics display correctly

- [ ] **SessionList** - [src/components/staff/SessionDashboard/components/SessionList.jsx](src/components/staff/SessionDashboard/components/SessionList.jsx)
  - [ ] Session cards render for all active sessions
  - [ ] Empty state displays when no sessions
  - [ ] Scroll functionality works
  - [ ] Sessions ordered correctly

- [ ] **SessionCard (Main)** - [src/components/staff/SessionDashboard/components/SessionCard/](src/components/staff/SessionDashboard/components/SessionCard/)
  - [ ] SessionCard renders with correct data
  - [ ] Card layout displays all sections
  - [ ] Expandable sections work

- [ ] **SessionHeader** - [SessionCard/SessionHeader.jsx](src/components/staff/SessionDashboard/components/SessionCard/SessionHeader.jsx)
  - [ ] Session ID displays
  - [ ] Session status displays
  - [ ] Created timestamp displays
  - [ ] Status indicator color correct

- [ ] **BillSection** - [SessionCard/BillSection.jsx](src/components/staff/SessionDashboard/components/SessionCard/BillSection.jsx)
  - [ ] Bills for session display
  - [ ] Bill status shows correctly
  - [ ] "New Bill" button opens dialog
  - [ ] Bill totals calculated correctly
  - [ ] Empty state when no bills

- [ ] **SwipeableBillCard** - [SessionCard/SwipeableBillCard.jsx](src/components/staff/SessionDashboard/components/SessionCard/SwipeableBillCard.jsx)
  - [ ] Bill card displays bill info
  - [ ] Swipe gesture (left) works
  - [ ] Swipe gesture (right) works
  - [ ] Swipe actions trigger callbacks
  - [ ] Touch events handled correctly
  - [ ] Swipe threshold appropriate

- [ ] **TableSection** - [SessionCard/TableSection.jsx](src/components/staff/SessionDashboard/components/SessionCard/TableSection.jsx)
  - [ ] Tables assigned to session display
  - [ ] Table numbers/names show
  - [ ] "Add Table" button opens dialog
  - [ ] Remove table functionality works
  - [ ] Empty state when no tables

- [ ] **SessionActions** - [SessionCard/SessionActions.jsx](src/components/staff/SessionDashboard/components/SessionCard/SessionActions.jsx)
  - [ ] "End Session" button displays
  - [ ] "Add Table" button displays
  - [ ] "Add Bill" button displays
  - [ ] Buttons trigger correct actions
  - [ ] Disabled states when appropriate

- [ ] **ServiceRequests** - [SessionCard/ServiceRequests.jsx](src/components/staff/SessionDashboard/components/SessionCard/ServiceRequests.jsx)
  - [ ] Service requests display for session
  - [ ] Request count badge shows
  - [ ] Mark as resolved works
  - [ ] Request types display correctly
  - [ ] Empty state when no requests

#### Session Dialogs

- [ ] **NewSessionDialog** - [dialogs/NewSessionDialog.jsx](src/components/staff/SessionDashboard/components/dialogs/NewSessionDialog.jsx)
  - [ ] Dialog opens/closes
  - [ ] Form fields render
  - [ ] Form validation works
  - [ ] Submit creates session
  - [ ] Success callback triggered
  - [ ] Error handling

- [ ] **NewBillDialog** - [dialogs/NewBillDialog.jsx](src/components/staff/SessionDashboard/components/dialogs/NewBillDialog.jsx)
  - [ ] Dialog opens/closes
  - [ ] Session ID passed correctly
  - [ ] Form submission works
  - [ ] Bill created successfully
  - [ ] Success callback triggered

- [ ] **AddTableDialog** - [dialogs/AddTableDialog.jsx](src/components/staff/SessionDashboard/components/dialogs/AddTableDialog.jsx)
  - [ ] Dialog opens/closes
  - [ ] Available tables list displays
  - [ ] Table selection works
  - [ ] Table added to session
  - [ ] Success callback triggered
  - [ ] Table availability check works

- [ ] **DialogContainer** - [dialogs/DialogContainer.jsx](src/components/staff/SessionDashboard/components/dialogs/DialogContainer.jsx)
  - [ ] Manages all dialog states
  - [ ] Opens correct dialog based on type
  - [ ] Closes dialogs correctly
  - [ ] Passes data to dialogs

#### Session Hooks

- [ ] **useSessionData** - [hooks/useSessionData.js](src/components/staff/SessionDashboard/hooks/useSessionData.js)
  - [ ] Fetches session data on mount
  - [ ] Refresh function works
  - [ ] Error state managed
  - [ ] Loading state managed
  - [ ] Data returned correctly

- [ ] **useSessionActions** - [hooks/useSessionActions.js](src/components/staff/SessionDashboard/hooks/useSessionActions.js)
  - [ ] Create session action works
  - [ ] End session action works
  - [ ] Add table action works
  - [ ] Remove table action works
  - [ ] Actions trigger API calls
  - [ ] Success/error handling

- [ ] **useDialogState** - [hooks/useDialogState.js](src/components/staff/SessionDashboard/hooks/useDialogState.js)
  - [ ] Opens dialogs with correct type
  - [ ] Closes dialogs
  - [ ] Passes data to dialogs
  - [ ] State management works

#### Session Context & Utils

- [ ] **SessionContext** - [context/SessionContext.jsx](src/components/staff/SessionDashboard/context/SessionContext.jsx)
  - [ ] Context provider works
  - [ ] State shared across components
  - [ ] Updates propagate correctly
  - [ ] Consumer components access state

- [ ] **sessionHelpers** - [utils/sessionHelpers.js](src/components/staff/SessionDashboard/utils/sessionHelpers.js)
  - [ ] Helper functions work correctly
  - [ ] Edge cases handled
  - [ ] Utility functions accurate

### Staff Components - Table Management

- [ ] **TableDashboard (Main)** - [src/components/staff/TableDashboard/index.jsx](src/components/staff/TableDashboard/index.jsx)
  - [ ] TableProvider wraps components
  - [ ] Context initialized
  - [ ] Component renders

- [ ] **TableDashboardInner** - [components/TableDashboardInner.jsx](src/components/staff/TableDashboard/components/TableDashboardInner.jsx)
  - [ ] Main content renders
  - [ ] Child components display
  - [ ] Layout responsive

- [ ] **TableSummary** - [components/TableSummary.jsx](src/components/staff/TableDashboard/components/TableSummary.jsx)
  - [ ] Table count displays
  - [ ] Group count displays
  - [ ] "Add Table" button opens dialog
  - [ ] "Add Group" button opens dialog
  - [ ] Statistics accurate

- [ ] **TableList** - [components/TableList.jsx](src/components/staff/TableDashboard/components/TableList.jsx)
  - [ ] All tables display
  - [ ] Filter functionality works
  - [ ] Sort functionality works
  - [ ] Empty state displays
  - [ ] Pagination works (if any)

- [ ] **TableCard** - [components/TableCard.jsx](src/components/staff/TableDashboard/components/TableCard.jsx)
  - [ ] Table information displays
  - [ ] Table number/name shows
  - [ ] Edit button opens edit dialog
  - [ ] Delete button shows confirmation
  - [ ] Status indicators work
  - [ ] Group membership shows (if applicable)

- [ ] **TableGroupCard** - [components/TableGroupCard.jsx](src/components/staff/TableDashboard/components/TableGroupCard.jsx)
  - [ ] Group information displays
  - [ ] Member tables list
  - [ ] Edit button opens edit dialog
  - [ ] Delete button shows confirmation
  - [ ] Add table to group works

#### Table Dialogs

- [ ] **NewTableDialog** - [dialogs/NewTableDialog.jsx](src/components/staff/TableDashboard/components/dialogs/NewTableDialog.jsx)
  - [ ] Dialog opens/closes
  - [ ] Form fields render
  - [ ] Table number validation
  - [ ] Uniqueness check works
  - [ ] Submit creates table
  - [ ] Success callback triggered

- [ ] **EditTableDialog** - [dialogs/EditTableDialog.jsx](src/components/staff/TableDashboard/components/dialogs/EditTableDialog.jsx)
  - [ ] Dialog opens with table data
  - [ ] Form pre-filled
  - [ ] Update submission works
  - [ ] Validation works
  - [ ] Success callback triggered

- [ ] **NewTableGroupDialog** - [dialogs/NewTableGroupDialog.jsx](src/components/staff/TableDashboard/components/dialogs/NewTableGroupDialog.jsx)
  - [ ] Dialog opens/closes
  - [ ] Group name validation
  - [ ] Table selection works
  - [ ] Group created successfully
  - [ ] Success callback triggered

- [ ] **EditTableGroupDialog** - [dialogs/EditTableGroupDialog.jsx](src/components/staff/TableDashboard/components/dialogs/EditTableGroupDialog.jsx)
  - [ ] Dialog opens with group data
  - [ ] Form pre-filled
  - [ ] Member management works
  - [ ] Delete group works
  - [ ] Update submission works

- [ ] **AddTableToGroupDialog** - [dialogs/AddTableToGroupDialog.jsx](src/components/staff/TableDashboard/components/dialogs/AddTableToGroupDialog.jsx)
  - [ ] Dialog opens/closes
  - [ ] Available tables list
  - [ ] Table selection works
  - [ ] Add to group works
  - [ ] Success callback triggered

- [ ] **DialogContainer (Tables)** - [dialogs/DialogContainer.jsx](src/components/staff/TableDashboard/components/dialogs/DialogContainer.jsx)
  - [ ] Manages all table dialogs
  - [ ] Opens correct dialog
  - [ ] Closes dialogs
  - [ ] Passes data correctly

#### Table Context & Hooks

- [ ] **TableContext** - [context/TableContext.jsx](src/components/staff/TableDashboard/context/TableContext.jsx)
  - [ ] Context provider works
  - [ ] State shared across components
  - [ ] Updates propagate

- [ ] **useTableData** - [hooks/useTableData.js](src/components/staff/TableDashboard/hooks/useTableData.js)
  - [ ] Fetches table data
  - [ ] Refresh function works
  - [ ] Error handling
  - [ ] Loading states

### Staff Components - Service Notifications

- [ ] **ServiceNotifications** - [src/components/staff/components/ServiceNotifications.jsx](src/components/staff/components/ServiceNotifications.jsx)
  - [ ] Service requests display
  - [ ] Badge count accurate
  - [ ] Mark as resolved works
  - [ ] Real-time updates work
  - [ ] Notification sound/alert (if any)

---

## 👥 CUSTOMER SYSTEM (7 Components + 4 Pages)

### Customer Pages

- [ ] **Full Menu Page** - [src/app/menu/full-menu/page.jsx](src/app/menu/full-menu/page.jsx)
  - [ ] Page loads without errors
  - [ ] Active session detection works
  - [ ] Redirects if no active session
  - [ ] Menu displays correctly
  - [ ] Order submission works

- [ ] **Join Session Page** - [src/app/join/page.jsx](src/app/join/page.jsx)
  - [ ] Page loads without errors
  - [ ] Session ID input works
  - [ ] Join session workflow works
  - [ ] Validation for session ID

- [ ] **Start Session Page** - [src/app/start-session/page.jsx](src/app/start-session/page.jsx)
  - [ ] Page loads without errors
  - [ ] QR scan parameter handling
  - [ ] Session initialization works
  - [ ] Table assignment works

- [ ] **Orders Page (Customer)** - [src/app/orders/page.jsx](src/app/orders/page.jsx)
  - [ ] Page loads without errors
  - [ ] Order status display works
  - [ ] Order list fetching works

### Customer Components - Menu & Ordering

- [ ] **FullMenu** - [src/app/menu/full-menu/page.jsx](src/app/menu/full-menu/page.jsx)
  - [ ] Active session detected via API
  - [ ] Bill selection dropdown displays
  - [ ] Bill selection required before ordering
  - [ ] Categories display as accordions
  - [ ] Category expansion/collapse works
  - [ ] Menu items load per category
  - [ ] Item images display (or fallback)
  - [ ] Item names, descriptions, prices display
  - [ ] Tags display with colors
  - [ ] Add-on vs included pricing shown
  - [ ] Search bar filters items (fuzzy search)
  - [ ] Tag filtering works
  - [ ] Swipe left increases quantity
  - [ ] Swipe right decreases quantity
  - [ ] +/- buttons adjust quantity
  - [ ] Order summary shows selected items
  - [ ] Order total calculates correctly
  - [ ] Submit order button works
  - [ ] Order sent to correct bill
  - [ ] Success message on order submission
  - [ ] Error handling for failed order
  - [ ] Empty state when no items
  - [ ] Loading states during data fetch

- [ ] **OrderDashboard (Customer)** - [src/components/customer/OrderDashboard/index.jsx](src/components/customer/OrderDashboard/index.jsx)
  - [ ] Order ID input field works
  - [ ] Load order button works
  - [ ] Order loads by ID
  - [ ] Pending items accordion displays
  - [ ] Error handling for invalid order ID

- [ ] **BillSelect** - [components/BillSelect.jsx](src/components/customer/OrderDashboard/components/BillSelect.jsx)
  - [ ] Dropdown populates with bills
  - [ ] Bill selection works
  - [ ] onChange callback triggered
  - [ ] Empty state when no bills
  - [ ] Loading state during fetch
  - [ ] Message prop displays

- [ ] **OrderSummaryItem** - [components/OrderSummaryItem.jsx](src/components/customer/OrderDashboard/components/OrderSummaryItem.jsx)
  - [ ] Item counts display correctly
  - [ ] Categories grouping works
  - [ ] Quantities accurate
  - [ ] onQuantityChange callback works
  - [ ] Item names display

- [ ] **PendingItemsAccordion** - [components/PendingItemsAccordion.jsx](src/components/customer/OrderDashboard/components/PendingItemsAccordion.jsx)
  - [ ] Accordion expands/collapses
  - [ ] Pending items load for order
  - [ ] Item list displays
  - [ ] Status indicators show
  - [ ] Empty state when no pending items

- [ ] **ServiceRequestForm** - [components/ServiceRequestForm.jsx](src/components/customer/OrderDashboard/components/ServiceRequestForm.jsx)
  - [ ] Form renders
  - [ ] Request type selection works
  - [ ] Submit button works
  - [ ] Success notification displays
  - [ ] Error handling

---

## 📊 ANALYTICS SYSTEM (6 Components + 5 Pages)

### Analytics Pages

- [ ] **Analytics Main Page** - [src/app/analytics/analytic-page/page.jsx](src/app/analytics/analytic-page/page.jsx)
  - [ ] Page loads without errors
  - [ ] Dashboard overview displays
  - [ ] Navigation to sub-pages works

- [ ] **Browsing Behavior Page** - [src/app/analytics/browsing-behavior/page.jsx](src/app/analytics/browsing-behavior/page.jsx)
  - [ ] Page loads without errors
  - [ ] BrowsingBehavior component renders
  - [ ] Data fetching works

- [ ] **Item Performance Page** - [src/app/analytics/item-performance/page.jsx](src/app/analytics/item-performance/page.jsx)
  - [ ] Page loads without errors
  - [ ] ItemPerformance component renders
  - [ ] Performance data displays

- [ ] **Order Timing Page** - [src/app/analytics/order-timing/page.jsx](src/app/analytics/order-timing/page.jsx)
  - [ ] Page loads without errors
  - [ ] OrderTiming component renders
  - [ ] Timing data visualization works

- [ ] **Table Turnover Page** - [src/app/analytics/table-turnover/page.jsx](src/app/analytics/table-turnover/page.jsx)
  - [ ] Page loads without errors
  - [ ] TableTurnover component renders
  - [ ] Turnover calculations display

### Analytics Components

- [ ] **BrowsingBehavior** - [src/components/analytics/BrowsingBehavior.jsx](src/components/analytics/BrowsingBehavior.jsx)
  - [ ] Auth token checked
  - [ ] Redirects if not authenticated
  - [ ] Data table displays browsing data
  - [ ] View counts per item accurate
  - [ ] Total view time displays
  - [ ] Loading state during fetch
  - [ ] Error handling

- [ ] **ItemPerformance** - [src/components/analytics/ItemPerformance.jsx](src/components/analytics/ItemPerformance.jsx)
  - [ ] Performance metrics load
  - [ ] Chart/graph displays
  - [ ] Data accurate
  - [ ] Filters work (if any)
  - [ ] Error handling

- [ ] **OrderTiming** - [src/components/analytics/OrderTiming.jsx](src/components/analytics/OrderTiming.jsx)
  - [ ] Timing data loads
  - [ ] Charts display timing info
  - [ ] Peak hours identified
  - [ ] Time range filtering works
  - [ ] Error handling

- [ ] **TableTurnover** - [src/components/analytics/TableTurnover.jsx](src/components/analytics/TableTurnover.jsx)
  - [ ] Turnover data loads
  - [ ] Calculations accurate
  - [ ] Table-specific metrics show
  - [ ] Date range filtering works
  - [ ] Error handling

- [ ] **Insight** - [src/components/analytics/insight.jsx](src/components/analytics/insight.jsx)
  - [ ] Card renders correctly
  - [ ] Icon displays
  - [ ] Value formatting correct
  - [ ] Styling consistent

- [ ] **insightStyles.js** - [src/components/analytics/insightStyles.js](src/components/analytics/insightStyles.js)
  - [ ] Styles applied correctly
  - [ ] Responsive behavior works
  - [ ] Consistency across components

---

## 📍 LOCATION & MENU SYSTEM (5 Components + 1 Page)

### Location & Menu Pages

- [ ] **Location Menu Page** - [src/app/location/menu-location/page.jsx](src/app/location/menu-location/page.jsx)
  - [ ] Page loads without errors
  - [ ] LocationManagement component renders
  - [ ] MenuManagement component renders
  - [ ] Tab switching works (if any)

### Location & Menu Components

- [ ] **LocationManagement** - [src/components/location/LocationManagement.jsx](src/components/location/LocationManagement.jsx)
  - [ ] DataGrid displays all locations
  - [ ] Create location button opens form
  - [ ] Edit location opens form with data
  - [ ] Delete location shows confirmation
  - [ ] Delete removes from list and database
  - [ ] Field normalization works (backend inconsistencies handled)
  - [ ] Menu assignment per location works
  - [ ] DataGrid pagination works
  - [ ] DataGrid sorting works
  - [ ] Success/error messages display
  - [ ] Loading states during operations

- [ ] **LocationForm** - [src/components/location/LocationForm.jsx](src/components/location/LocationForm.jsx)
  - [ ] Form renders in create mode
  - [ ] Form renders in edit mode
  - [ ] Address fields work (address_one, address_two)
  - [ ] City field works
  - [ ] Province field works
  - [ ] Postal code field works
  - [ ] Phone number field works
  - [ ] Form validation works
  - [ ] Required fields enforced
  - [ ] Phone number format validation
  - [ ] Postal code format validation
  - [ ] Submit triggers callback
  - [ ] Cancel button works

- [ ] **MenuManagement** - [src/components/location/MenuManagement.jsx](src/components/location/MenuManagement.jsx)
  - [ ] Menu list displays
  - [ ] Create menu button works
  - [ ] Edit menu button works
  - [ ] Delete menu shows confirmation
  - [ ] Menu item assignment works
  - [ ] Success/error messages display

- [ ] **MenuForm** - [src/components/location/MenuForm.jsx](src/components/location/MenuForm.jsx)
  - [ ] Form renders
  - [ ] Menu name validation
  - [ ] Description input works
  - [ ] Form submission works
  - [ ] Cancel button works

- [ ] **MenuAssignmentForm** - [src/components/location/MenuAssignmentForm.jsx](src/components/location/MenuAssignmentForm.jsx)
  - [ ] Menu selection dropdown works
  - [ ] Location displays
  - [ ] Assignment submission works
  - [ ] Success callback triggered

---

## 🏷️ TAG SYSTEM (3 Components + 1 Page)

### Tag Pages

- [ ] **Tag Management Page** - [src/app/tags/tag-management/page.jsx](src/app/tags/tag-management/page.jsx)
  - [ ] Page loads without errors
  - [ ] TagManagement component renders
  - [ ] Proper routing works

### Tag Components

- [ ] **TagManagement** - [src/components/tags/TagManagement.jsx](src/components/tags/TagManagement.jsx)
  - [ ] DataGrid displays all tags
  - [ ] Tags display with colors
  - [ ] Create tag button opens form
  - [ ] Auto-generated color on create
  - [ ] Color generation algorithm works (pastel)
  - [ ] Color uniqueness validation (min difference 75)
  - [ ] Color refresh button works
  - [ ] Edit tag opens form with data
  - [ ] Delete tag shows confirmation
  - [ ] Delete shows item count warning
  - [ ] Delete removes from database
  - [ ] View items with tag button works
  - [ ] Items list dialog displays
  - [ ] Remove tag from item works
  - [ ] DataGrid pagination works
  - [ ] Success/error messages display
  - [ ] Tag preview rendering correct
  - [ ] Color contrast readable

- [ ] **TagChip** - [src/components/tags/TagChip.jsx](src/components/tags/TagChip.jsx)
  - [ ] Chip renders with tag name
  - [ ] Background color applies correctly
  - [ ] Size prop works (s, m, l, xl)
  - [ ] isActionChip prop shows X button
  - [ ] X button triggers onToggle callback
  - [ ] Text contrast readable on all colors
  - [ ] Responsive sizing works
  - [ ] isUsedFor prop displays context (if applicable)

- [ ] **Tags (Menu Item Tags)** - [src/components/Tags.jsx](src/components/Tags.jsx)
  - [ ] Fetches tags for item_id
  - [ ] Multiple tags display
  - [ ] Size prop passed to TagChip
  - [ ] Empty state when no tags
  - [ ] Loading state during fetch
  - [ ] Error handling

---

## 🔧 SHARED/COMMON COMPONENTS (5 Components)

- [ ] **Layout** - [src/components/Layout.jsx](src/components/Layout.jsx)
  - [ ] Layout structure correct
  - [ ] Header included
  - [ ] Children render correctly
  - [ ] Footer (if any) displays
  - [ ] Responsive behavior works

- [ ] **Header** - [src/components/Header.jsx](src/components/Header.jsx)
  - [ ] Logo displays
  - [ ] Navigation menu works
  - [ ] User profile/menu displays
  - [ ] Logout button works
  - [ ] Authentication state checked
  - [ ] Mobile responsive menu works
  - [ ] Navigation links work
  - [ ] Role-based menu items display

- [ ] **AppBarWithTitle** - [src/components/AppBarWithTitle.jsx](src/components/AppBarWithTitle.jsx)
  - [ ] AppBar renders
  - [ ] Title displays correctly
  - [ ] Back button (if any) works
  - [ ] Styling consistent

- [ ] **BillDialogAdapter** - [src/components/BillDialogAdapter.jsx](src/components/BillDialogAdapter.jsx)
  - [ ] Dialog opens/closes
  - [ ] Data passed correctly
  - [ ] Callbacks work
  - [ ] Adapter pattern works

- [ ] **QRScanner** - [src/components/QRScanner.jsx](src/components/QRScanner.jsx)
  - [ ] Camera permission requested
  - [ ] Camera access works
  - [ ] QR code detection works
  - [ ] Session/table extracted correctly
  - [ ] Data callback triggered
  - [ ] Error handling (no camera)
  - [ ] Scanner stops after detection

---

## 🛠️ UTILITIES & CONFIGURATION

### API Configuration

- [ ] **api.js** - [src/config/api.js](src/config/api.js)
  - [ ] Axios instance created
  - [ ] Base URL configured correctly
  - [ ] Request interceptor adds auth token
  - [ ] Response interceptor handles 401
  - [ ] Token refresh flow works
  - [ ] Error propagation works
  - [ ] Timeout configured (if any)

### Storage Utilities

- [ ] **storage.js** - [src/utils/storage.js](src/utils/storage.js)
  - [ ] Get/set functions work
  - [ ] JSON parsing works
  - [ ] Error handling for invalid JSON
  - [ ] LocalStorage access works

### Menu Search Utilities

- [ ] **searchUtils.js** - [src/components/menu/searchUtils.js](src/components/menu/searchUtils.js)
  - [ ] useMenuSearch hook works
  - [ ] Fuzzy search accuracy
  - [ ] Search by name works
  - [ ] Search by description works
  - [ ] Search by tags works
  - [ ] Real-time filtering works
  - [ ] Performance with large datasets
  - [ ] Multi-word search works

---

## 🌐 CONTEXT PROVIDERS

- [ ] **SessionContext** - [src/components/staff/SessionDashboard/context/SessionContext.jsx](src/components/staff/SessionDashboard/context/SessionContext.jsx)
  - [ ] Context provider works
  - [ ] State shared correctly
  - [ ] Updates propagate

- [ ] **TableContext** - [src/components/staff/TableDashboard/context/TableContext.jsx](src/components/staff/TableDashboard/context/TableContext.jsx)
  - [ ] Context provider works
  - [ ] State shared correctly
  - [ ] Updates propagate

- [ ] **MenuContext** - [src/contexts/MenuContext.js](src/contexts/MenuContext.js)
  - [ ] Context provider works
  - [ ] Menu data shared
  - [ ] State updates work

---

## 📄 ROOT FILES & ERROR PAGES

- [ ] **Root Layout** - [src/app/layout.jsx](src/app/layout.jsx)
  - [ ] HTML structure correct
  - [ ] Head metadata present
  - [ ] Global providers wrap children
  - [ ] Theme provider works
  - [ ] Global styles applied

- [ ] **Home Page** - [src/app/page.jsx](src/app/page.jsx)
  - [ ] Landing page loads
  - [ ] Splash design displays
  - [ ] Navigation to menu works
  - [ ] QR scan handling works (if applicable)

- [ ] **Error Page** - [src/app/error.js](src/app/error.js)
  - [ ] Error boundary works
  - [ ] User-friendly error message
  - [ ] Retry button (if any)
  - [ ] Navigation back to safe page

- [ ] **404 Not Found** - [src/app/not-found.js](src/app/not-found.js)
  - [ ] 404 page displays for invalid routes
  - [ ] User-friendly message
  - [ ] Link back to home
  - [ ] Navigation works

- [ ] **Unauthorized Page** - [src/app/unauthorized.js](src/app/unauthorized.js)
  - [ ] Displays when user lacks permissions
  - [ ] Clear message about unauthorized access
  - [ ] Back to login link works
  - [ ] Navigation works

- [ ] **Health Check Route** - [src/app/health/route.ts](src/app/health/route.ts)
  - [ ] Health check endpoint responds
  - [ ] Status code correct (200)
  - [ ] Response format correct

---

## 🧪 EXISTING TESTS

- [ ] **QRCodeManagement.test.jsx** - [src/components/admin/__tests__/QRCodeManagement.test.jsx](src/components/admin/__tests__/QRCodeManagement.test.jsx)
  - [ ] Tests run without errors
  - [ ] All test cases pass
  - [ ] Coverage adequate
  - [ ] Mock setup correct

---

## 🔍 CROSS-CUTTING CONCERNS

### Authentication & Authorization

- [ ] Login redirects to appropriate dashboard based on role
- [ ] Logout clears all tokens and redirects to login
- [ ] Token expiration auto-logs out user
- [ ] Protected routes redirect unauthenticated users
- [ ] Role-based access control enforced on all admin routes
- [ ] Role-based access control enforced on all staff routes
- [ ] Customer routes accessible without login (or session-based)
- [ ] Cross-tab logout synchronization works

### Error Handling

- [ ] Network errors display user-friendly messages
- [ ] API errors display appropriate messages
- [ ] 404 errors redirect to not-found page
- [ ] 401 errors redirect to login
- [ ] 403 errors redirect to unauthorized page
- [ ] Form validation errors display clearly
- [ ] Snackbar/toast notifications work for all operations
- [ ] Error boundaries catch component errors

### Loading States

- [ ] Initial page load shows loading indicator
- [ ] Data fetching shows loading spinner
- [ ] Button clicks show loading state (disabled + spinner)
- [ ] Form submissions show loading state
- [ ] Skeleton loaders for content (if implemented)
- [ ] Loading states don't block entire UI unnecessarily

### Responsive Design

- [ ] All pages responsive on mobile (320px+)
- [ ] All pages responsive on tablet (768px+)
- [ ] All pages responsive on desktop (1024px+)
- [ ] Touch gestures work on mobile (swipe, tap)
- [ ] Navigation menu responsive
- [ ] DataGrids responsive (scrollable or adaptive)
- [ ] Dialogs responsive and mobile-friendly
- [ ] Forms responsive on all screen sizes

### Accessibility

- [ ] All buttons have accessible labels
- [ ] All form inputs have labels
- [ ] Keyboard navigation works
- [ ] Focus indicators visible
- [ ] Screen reader compatibility (aria-labels)
- [ ] Color contrast sufficient (WCAG AA)
- [ ] Error messages announced to screen readers

### Performance

- [ ] Initial page load time acceptable (<3s)
- [ ] Images optimized and lazy-loaded
- [ ] API calls not duplicated unnecessarily
- [ ] Auto-refresh intervals appropriate (30s)
- [ ] No memory leaks from intervals/listeners
- [ ] Event listeners cleaned up on unmount
- [ ] Large lists paginated or virtualized

### Security

- [ ] XSS prevention (input sanitization)
- [ ] CSRF tokens (if required by backend)
- [ ] Sensitive data not logged to console
- [ ] Auth tokens stored securely (httpOnly if possible)
- [ ] No credentials hardcoded in production
- [ ] API calls use HTTPS
- [ ] File uploads validated (type, size)

---

## 📋 TESTING WORKFLOW

### Manual Testing Steps

1. **Authentication Flow**
   - Test login → logout → login again
   - Test registration → email verification (if any) → login
   - Test forgot password → reset password
   - Test invalid credentials
   - Test token expiration

2. **Admin Workflows**
   - Create/edit/delete menu items
   - Upload images to menu items
   - Assign tags to menu items
   - Create/manage staff accounts
   - Generate QR codes (WiFi and Session)
   - Bulk QR code generation
   - Manage categories

3. **Staff Workflows**
   - View pending orders
   - Approve orders with quantity adjustments
   - Mark orders as delivered
   - Create new dining sessions
   - Add tables to sessions
   - Create bills for sessions
   - End sessions
   - Manage table configurations
   - Create table groups
   - View service requests
   - Mark service requests as resolved

4. **Customer Workflows**
   - Scan QR code to start session
   - Join existing session
   - Browse menu
   - Search for menu items
   - Filter by tags
   - Select bill for order
   - Add items to order (swipe or +/-)
   - Submit order
   - View order status
   - Request service (call server)

5. **Analytics Review**
   - View browsing behavior
   - Check item performance metrics
   - Analyze order timing
   - Review table turnover rates

6. **Location & Menu Management**
   - Create/edit/delete locations
   - Assign menus to locations
   - Create/edit menus
   - Assign menu items to menus

7. **Tag Management**
   - Create tags with auto-generated colors
   - Edit tags
   - Delete tags (with item count warning)
   - View items with specific tag
   - Remove tag from items
   - Refresh colors

### Automated Testing

- [ ] Run all existing tests: `npm test`
- [ ] Check test coverage: `npm run test:coverage`
- [ ] Run E2E tests: `npm run test:e2e`
- [ ] Ensure coverage meets thresholds (70%)

### Browser Testing

- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)
- [ ] Mobile Chrome (Android)
- [ ] Mobile Safari (iOS)

### Device Testing

- [ ] Desktop (Windows/Mac/Linux)
- [ ] Tablet (iPad, Android tablet)
- [ ] Mobile (iPhone, Android phone)
- [ ] Different screen sizes (320px, 768px, 1024px, 1920px)

---

## 📊 SUMMARY STATISTICS

### Component Counts

- **Total Components & Pages:** 79+
- **Admin Components:** 12
- **Staff Components:** 30+
- **Customer Components:** 7
- **Analytics Components:** 6
- **Auth Components:** 5
- **Location/Menu Components:** 5
- **Tag Components:** 3
- **Shared Components:** 5
- **Utilities & Hooks:** 6
- **Context Providers:** 3

### API Endpoints Used

- **Authentication:** 4 endpoints
- **Menu Items:** 10+ endpoints
- **Categories:** 1 endpoint
- **Menus:** 1+ endpoints
- **Tags:** 6 endpoints
- **Staff:** 4 endpoints
- **Locations:** 3+ endpoints
- **Dining Sessions:** 2+ endpoints
- **Orders:** 6 endpoints
- **QR Codes:** 4 endpoints
- **Analytics:** 4 endpoints

### Testing Priorities

**High Priority (Core Functionality):**
1. Authentication flow
2. Menu display and ordering
3. Order management (staff)
4. Session management
5. Menu item CRUD
6. Staff management
7. QR code generation

**Medium Priority (Important Features):**
1. Tag management
2. Location management
3. Table management
4. Service requests
5. Analytics display
6. Bill management

**Low Priority (Nice to Have):**
1. Search optimization
2. Color generation algorithm
3. Swipe gestures
4. Real-time refresh optimization

---

## 🎯 NEXT STEPS

1. **Systematically work through this checklist** section by section
2. **Mark items as you test** them using the checkboxes
3. **Document any issues found** in a separate issues tracking document
4. **Create bug reports** for any failures
5. **Update this checklist** as components are added/removed
6. **Rerun automated tests** after fixing issues
7. **Perform regression testing** on critical paths

---

## 📝 NOTES

- This checklist should be updated whenever new components are added
- Use this in conjunction with automated test suites
- Prioritize high-priority items first
- Document any deviations from expected behavior
- Keep track of browser/device-specific issues
- Update the status document when sections are completed

---

**Document Version:** 1.0
**Created:** 2025-10-30
**Last Reviewed:** 2025-10-30
