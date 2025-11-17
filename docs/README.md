# CDAJT_C-Capstone

Continuing project for **Sushi Toshi** to provide a comprehensive digital restaurant management platform.

<details>
<summary>Table of Contents</summary>

1. [Tech Stack](#-tech-stack)
2. [Project Structure](#-project-structure)
3. [Getting Started](#-getting-started)
4. [Usage](#usage)
5. [Roles & Permissions](#-roles--permissions)
6. [Contributors](#contributing)

</details>

## 🛠️ Tech Stack
- **Backend**: .NET Core 9.0 API
- **Frontend**: Next.js 15.5.4 with React 19.1.0, Material-UI, CSS
- **Database**: MariaDB
- **Authentication**: OAuth 2.0 via Azure AD (Microsoft Entra ID), JWT Bearer tokens
- **QR Code Generation**: QRCoder + SkiaSharp
- **Email Service**: SendGrid
- **Deployment**: Docker
- **Testing**: xUnit, Moq, Jest, Playwright

## 📂 Project Structure

```
CDAJT_C-Capstone/
├── src/
│   ├── back-end/                    # .NET Core 9.0 API
│   │   ├── Controllers/             # 19 API controllers
│   │   │   ├── AdminQrCodeController.cs
│   │   │   ├── AnalyticsController.cs
│   │   │   ├── BillController.cs
│   │   │   ├── CategoryController.cs
│   │   │   ├── DashboardController.cs
│   │   │   ├── DiningSessionController.cs
│   │   │   ├── LocationController.cs
│   │   │   ├── MenuController.cs
│   │   │   ├── MenuItemController.cs
│   │   │   ├── OrderController.cs
│   │   │   ├── ServiceRequestController.cs
│   │   │   ├── SessionController.cs
│   │   │   ├── TableController.cs
│   │   │   └── ... (and more)
│   │   ├── domain/                  # Data layer
│   │   │   ├── ApplicationContext.cs  # EF Core DbContext
│   │   │   ├── entities/            # 16 database models
│   │   │   └── Seeders/             # Database seeding system
│   │   ├── DTO/                     # Data Transfer Objects
│   │   ├── Services/                # Business logic services
│   │   │   ├── QrGeneratorService.cs
│   │   │   ├── PricingService.cs
│   │   │   └── SendGridEmailServices.cs
│   │   ├── Helpers/
│   │   │   └── ClaimsHelpers.cs     # JWT claims extraction
│   │   ├── Program.cs               # Application entry point
│   │   ├── appsettings.json         # Configuration
│   │   └── back-end.csproj
│   │
│   ├── back-end.Tests/              # Backend unit tests (xUnit)
│   │   ├── Controllers/             # Controller tests
│   │   ├── Services/                # Service tests
│   │   └── Helpers/                 # Helper tests
│   │
│   ├── back-end.IntegrationTests/   # Backend integration tests
│   │
│   ├── front-end/                   # Next.js frontend
│   │   ├── src/
│   │   │   ├── app/                 # Next.js App Router
│   │   │   │   ├── admin/           # Admin pages
│   │   │   │   │   ├── menu-items/
│   │   │   │   │   ├── qr-codes/
│   │   │   │   │   └── staff/
│   │   │   │   ├── analytics/       # Analytics dashboard
│   │   │   │   │   ├── analytic-page/
│   │   │   │   │   ├── browsing-behavior/
│   │   │   │   │   ├── item-performance/
│   │   │   │   │   ├── order-timing/
│   │   │   │   │   └── table-turnover/
│   │   │   │   ├── auth/            # Authentication pages
│   │   │   │   ├── dashboard/       # Customer/Staff dashboard
│   │   │   │   ├── menu/            # Menu browsing
│   │   │   │   ├── staff/           # Staff operations
│   │   │   │   └── ...
│   │   │   ├── components/          # React components
│   │   │   │   ├── admin/
│   │   │   │   ├── analytics/
│   │   │   │   ├── customer/
│   │   │   │   ├── menu/
│   │   │   │   ├── staff/
│   │   │   │   └── common/
│   │   │   ├── config/              # Configuration
│   │   │   │   ├── msalInstance.js  # Azure AD MSAL
│   │   │   │   └── auth.js
│   │   │   ├── contexts/            # React contexts
│   │   │   ├── services/            # API services
│   │   │   └── utils/               # Utilities
│   │   ├── package.json
│   │   ├── next.config.js
│   │   └── tsconfig.json
│   │
│   └── maria-db/                    # Database Docker setup
│       └── dockerfile
│
├── docs/                            # Documentation
│   ├── README.md                    # This file
│   ├── TEAMNORMS.md                 # Team standards
│   ├── entra-admin-guide.md         # Azure AD setup
│   ├── TESTING_README.md            # Testing guide
│   └── WIFI_CONFIGURATION_GUIDE.md
│
├── .github/workflows/               # CI/CD pipelines
│   ├── automated-tests.yml          # Testing automation
│   └── main.yml                     # Deployment workflow
│
└── docker-compose.yml               # Docker orchestration
```

### Backend Architecture

**Layered Architecture:**
- **Controllers** → **DTOs** → **Services** → **Database (EF Core)**
- Clean separation of concerns with dependency injection
- Role-based authorization using JWT claims

**Key Features:**
- Entity Framework Core with MariaDB
- Automatic database migrations on startup
- Comprehensive seeding system for initial data
- Dynamic pricing based on day/time/holidays
- QR code generation for WiFi and session access
- Rate limiting (100 req/min general, 10 req/min auth)

### Frontend Architecture

**Next.js App Router:**
- File-based routing system
- Server and client components
- API routes for backend communication

**State Management:**
- React Context API (MenuContext)
- Azure AD MSAL for authentication state
- Component-level state with React hooks

**UI Framework:**
- Material-UI (MUI) components
- Responsive design
- Lucide React icons

### Database Schema

**Core Tables:**
- `menu`, `menu_item`, `category`, `tag` - Menu management
- `dining_session`, `session_participant` - Session tracking
- `session_order`, `order_item` - Order management
- `bill` - Billing for payments
- `table_entity`, `table_group` - Table management
- `location` - Multi-location support
- `service_requests` - Customer assistance

## 🚀 Getting Started

### Prerequisites

- **Node.js** 18+ and npm
- **.NET SDK** 9.0+
- **Docker** and Docker Compose
- **Git**

### Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/SiberianIntergrations/CDAJT_C-Capstone.git
cd CDAJT_C-Capstone
```

#### 2. Database Setup

Start the MariaDB container:

```bash
docker-compose up -d
```

This will create:
- **Container**: `mariadb-sushi-toshi`
- **Database**: `sushi_toshi_db`
- **Port**: `3307` (external) → `3306` (internal)
- **Credentials**: `app_user` / `app_password`

#### 3. Backend Setup

Navigate to the backend directory:

```bash
cd src/back-end
```

Restore NuGet packages:

```bash
dotnet restore
```

Update `appsettings.json` if needed (optional):
- Database connection string
- Azure AD credentials
- QR code settings
- WiFi configurations for locations

Run database migrations (automatic on first run):

```bash
dotnet ef database update
```

Start the backend:

```bash
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5264`
- **Swagger UI**: `http://localhost:5264/swagger`

#### 4. Frontend Setup

Open a new terminal and navigate to the frontend:

```bash
cd src/front-end
```

Install dependencies:

```bash
npm install
```

Configure Azure AD (if needed):
- Update `src/config/msalInstance.js` with your Azure AD tenant details
- Current default: `renovationstationexsm3943.ciamlogin.com`

Start the development server:

```bash
npm run dev
```

The frontend will be available at:
- **URL**: `http://localhost:3000`

### Running Tests

#### Backend Tests

```bash
# Unit tests
dotnet test src/back-end.Tests/back-end.Tests.csproj

# Integration tests
dotnet test src/back-end.IntegrationTests/back-end.IntegrationTests.csproj

# All tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

#### Frontend Tests

```bash
cd src/front-end

# Unit tests
npm test

# Watch mode
npm run test:watch

# Coverage report
npm run test:coverage

# E2E tests with Playwright
npm run test:e2e

# E2E with UI
npm run test:e2e:ui
```

### Environment Variables

#### Backend (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3307;Database=sushi_toshi_db;User=app_user;Password=app_password;"
  },
  "JwtSettings": {
    "Key": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  },
  "AzureAd": {
    "Instance": "https://renovationstationexsm3943.ciamlogin.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "99ffb099-af80-41d1-9c16-e844f8ed5308"
  }
}
```

#### Frontend (`.env.local`)

Create a `.env.local` file in `src/front-end/`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5264
NEXT_PUBLIC_AZURE_CLIENT_ID=99ffb099-af80-41d1-9c16-e844f8ed5308
NEXT_PUBLIC_AZURE_TENANT=renovationstationexsm3943.ciamlogin.com
```

## Usage

### For Developers

#### API Documentation

Access Swagger UI for interactive API documentation:
```
http://localhost:5264/swagger
```

#### Common Development Tasks

**Add a new database migration:**
```bash
cd src/back-end
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

**Build for production:**
```bash
# Backend
cd src/back-end
dotnet publish -c Release

# Frontend
cd src/front-end
npm run build
npm start
```

**Run with Docker:**
```bash
docker-compose up --build
```

### For Users

#### Customers

1. **Scan QR Code** at your table to access the menu
2. **Join Session** by entering the session code
3. **Browse Menu** and add items to your order
4. **Submit Orders** directly to the kitchen
5. **Request Assistance** from staff when needed
6. **Split Bills** among participants

#### Staff

1. **Login** with staff credentials
2. **Create Sessions** for new tables/parties
3. **Manage Tables** and table groups for large parties
4. **Monitor Orders** in real-time
5. **Respond to Requests** from customers
6. **Close Sessions** to process payments

#### Administrators

1. **Login** with admin credentials
2. **Manage Locations** - Add/edit restaurant locations
3. **Configure Menus** - Create menus, add items, set availability
4. **Generate QR Codes** - Create WiFi and session QR codes
5. **Manage Staff** - Add users, assign roles
6. **View Analytics** - Access business intelligence dashboard
   - Item performance metrics
   - Customer browsing behavior
   - Order timing analysis
   - Table turnover rates
   - SWOT analysis tools

## 🔐 Roles & Permissions

| Role | Permissions |
| ----- | ----------- |
| Customers | Browse menu, submit orders, request assistance, and manage billing. |
| Employees/Staff | Manage tables, sessions and customer requests. |
| Admins | Full access to table, sessions, location, staff, and menu management and analytics. |

### User Roles & Permissions

| Feature | Customer | Staff/Employee | Admin |
| ------- | :------: | :------------: | :---: |
| Browse menu | ✓ | ✓ | ✓ |
| Submit orders | ✓ | ✓ | ✓ |
| Request assistance | ✓ | ✓ | ✓ |
| Manage personal bills | ✓ | ✓ | ✓ |
| Manage table sessions | ✗ | ✓ | ✓ |
| Handle customer requests | ✗ | ✓ | ✓ |
| Close tables/sessions | ✗ | ✓ | ✓ |
| Manage locations | ✗ | ✗ | ✓ |
| Manage staff accounts | ✗ | ✗ | ✓ |
| Configure menus | ✗ | ✗ | ✓ |
| View analytics | ✗ | ✗ | ✓ |

---

## Key Features

### For Customers
- **QR Code Access**: Scan table QR code to access menu
- **Digital Menu**: Browse items with photos, descriptions, and pricing
- **Order Management**: Place orders, track status (pending/completed)
- **Bill Splitting**: Create and manage multiple bills per table
- **Server Requests**: Call for assistance with custom messages

### For Staff
- **Session Management**: Create, manage, and close table sessions
- **Multi-Table Sessions**: Combine tables for large parties
- **Order Tracking**: Monitor all orders in real-time
- **Customer Requests**: Receive and respond to assistance requests
- **Bill Management**: Create separate bills within sessions

### For Administrators
- **Location Management**: Manage multiple restaurant locations
- **Menu Configuration**: Create menus, add/edit items, manage availability
- **Staff Management**: Add users, assign roles, manage permissions
- **Tag System**: Organize items with dietary, feature, and allergen tags
- **Analytics Dashboard**:
  - Item performance (top/bottom sellers)
  - Customer browsing behavior
  - Order timing metrics
  - Table turnover analysis
  - SWOT analysis tool

---

## Contributing

### Team Members

**Team (CDAJT_C-Capstone):**
- Timothy Torpy - [TimothyTorpy](https://github.com/TimothyTorpy)
- Evan Gamble - [TheHolographicP](https://github.com/TheHolographicP)
- Josh Mantei - [jmantei](https://github.com/jmantei)
- David Rochefort - [bitroch](https://github.com/bitroch)
- Alex M - [Pewpy](https://github.com/Pewpy)
- Clarisse Buniel - [seclaris](https://github.com/seclaris)

**Previous Team (AYCE-CSharp):**
- Corey Fakeley - [fakeley](https://github.com/fakeley)
- Kyle Donnan - [slkrick87](https://github.com/slkrick87)
- Marc Allain - [uamallain](https://github.com/uamallain)
- Wei Li - [Leonlivy](https://github.com/Leonlivy)

---

## License

This project is developed as part of a capstone project for the University of Alberta. All rights reserved.

---

## Acknowledgments

Continuing the work of the AYCE-CSharp team to provide a comprehensive digital restaurant management platform for Sushi Toshi.

---

**Version**: 2.0.0
**Last Updated**: 2025-11-10
**Status**: Active Development

For questions or support, please contact the development team or submit an issue.
