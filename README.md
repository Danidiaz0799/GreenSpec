# 🏭 Industrial Automation Alert System

Full-stack real-time monitoring system for industrial sensors with instant notifications via SignalR/WebSockets.
![alt text](image.png)

## 🎯 Features

- 🔐 **JWT Authentication** - Secure login system
- 📊 **Real-time Dashboard** - Automatic updates with SignalR
- ⚙️ **Dynamic Configuration** - Temperature and humidity threshold adjustment
- 🌡️ **Sensor Simulation** - Automatic data generation every 4 seconds
- 📈 **Alert Management** - Filtering, pagination, and acknowledgment
- 🔴 **Live updates** - No need to reload the page

## 🏗️ Tech Stack

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core 9.0.10
- PostgreSQL 15
- JWT Bearer Authentication
- SignalR for WebSockets
- Clean Architecture (Domain → Infrastructure → Api)

### Frontend
- Next.js 16.0.0 (App Router + Turbopack)
- TypeScript
- TailwindCSS
- TanStack Query (React Query)
- @microsoft/signalr

## 📁 Project Structure

```
GreenSpec/
├── backend/                          # ASP.NET Core Backend
│   ├── AlertService.Domain/          # Entities and interfaces
│   ├── AlertService.Infrastructure/  # EF Core, Repositories, Services
│   ├── AlertService.Api/             # Controllers, Hubs, DTOs
│   └── AlertService.sln              # Visual Studio Solution
│
├── frontend/                         # Next.js Frontend
│   ├── app/                          # Pages (login, dashboard)
│   ├── components/                   # React Components
│   ├── lib/                          # API client and hooks
│   └── types/                        # TypeScript Definitions
│
└── README.md                         # This file
```

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended for PostgreSQL)

### 1️⃣ Database with Docker

```bash
# Create and run PostgreSQL container
docker run --name greenspec-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=alertservice \
  -p 5432:5432 \
  -d postgres:15

# Verify it's running
docker ps

# Start in the future (if already exists)
docker start greenspec-postgres

# Stop
docker stop greenspec-postgres
```

### 2️⃣ Backend

```bash
# Navigate to backend folder
cd backend/AlertService.Api

# Apply migrations (create tables)
dotnet ef database update --project ../AlertService.Infrastructure

# Run the backend
dotnet run
```

Backend will be available at: **http://localhost:5046**

### 3️⃣ Frontend

**Open new terminal:**

```bash
# Navigate to frontend folder
cd frontend

# Install dependencies (first time only)
npm install

# Run in development mode
npm run dev
```

Frontend will be available at: **http://localhost:3000**

### 4️⃣ Access the Application

1. Open http://localhost:3000
2. Login with:
   - **Username:** `demo`
   - **Password:** `demo`
   
   > ⚠️ **Note:** Credentials are hardcoded in `backend/AlertService.Api/Controllers/AuthController.cs` for demonstration purposes only. The `frontend/.env.local` file is included in the repository to facilitate testing.

3. Done! You will see the dashboard with real-time alerts

## 📊 Main Features

### Dashboard
- **Live Indicator:** Shows SignalR connection status (🟢 Live / 🟡 Connecting / 🔴 Disconnected)
- **Total alerts:** Real-time counter
- **Filters:** All / Open / Acknowledged
- **Pagination:** 10 alerts per page with smart navigation

### Threshold Configuration
- **Maximum Temperature:** Configurable in °C
- **Maximum Humidity:** Configurable in %
- **Live update:** Changes apply immediately to the simulator

### Alerts Table
Columns:
- Alert ID
- Type (Temperature 🌡️ / Humidity 💧)
- Detected value
- Configured threshold
- Excess (value and percentage)
- Date and time
- Status (Open 🔴 / Acknowledged ✅)
- Action ("Acknowledge" button)

## 🔧 API Endpoints

### Authentication
```http
POST http://localhost:5046/auth/login
Content-Type: application/json

{
  "username": "demo",
  "password": "demo"
}
```

### Configuration
```http
# Get configuration
GET http://localhost:5046/config
Authorization: Bearer {token}

# Update thresholds
PUT http://localhost:5046/config
Authorization: Bearer {token}
Content-Type: application/json

{
  "tempMax": 60.0,
  "humidityMax": 75.0
}
```

### Alerts
```http
# List alerts
GET http://localhost:5046/alerts
Authorization: Bearer {token}

# Acknowledge alert
POST http://localhost:5046/alerts/123/acknowledge
Authorization: Bearer {token}
```

### SignalR Hub
- **Endpoint:** `ws://localhost:5046/hubs/alerts`
- **Event:** `ReceiveNewAlert`
- **Transport:** WebSockets with Long Polling fallback

## 🧪 Tests

The project includes **33 unit tests** covering:

### Controllers
- **AuthController**: 5 tests (valid login, invalid credentials, validations)
- **ConfigController**: 5 tests (get config, threshold validations)
- **AlertsController**: 7 tests (list, get by ID, acknowledge, validations)

### Repositories
- **ConfigRepository**: 3 tests (CRUD operations with InMemory DB)
- **AlertRepository**: 5 tests (create, list sorted, get, update status)

### Entities
- **Alert and Config**: 8 tests (properties, constants, default values)

### Run Tests

```bash
cd backend
dotnet test
```

**Expected result**: `33 tests passed` ✅

---

## 🐛 Troubleshooting

### Backend won't start
- Verify PostgreSQL is running: `docker ps`
- Verify port 5046 is not in use

### Frontend doesn't connect to SignalR
- Verify the backend is running
- Check browser console (F12)
- Initial negotiation error is normal, it will reconnect automatically

### Error "Failed to connect to database"
- Verify connection string in `backend/AlertService.Api/appsettings.json`
- Make sure PostgreSQL is running on port 5432

### No alerts appear
- The simulator generates alerts every 4 seconds
- Verify thresholds are configured (default: Temp 50°C, Humidity 70%)
- Random values must exceed thresholds to generate alerts

## 🛠️ Useful Commands

### Docker
```bash
# View running containers
docker ps

# View PostgreSQL logs
docker logs greenspec-postgres

# Restart PostgreSQL
docker restart greenspec-postgres

# Remove container
docker rm -f greenspec-postgres
```

### Backend
```bash
cd backend/AlertService.Api

# Build without running
dotnet build

# Create new migration
dotnet ef migrations add MigrationName --project ../AlertService.Infrastructure

# Revert last migration
dotnet ef database update PreviousMigrationName --project ../AlertService.Infrastructure
```

### Frontend
```bash
cd frontend

# Build for production
npm run build

# Start production version
npm start

# Clean cache
npm run clean
```

## 📦 Main Dependencies

### Backend
- `Microsoft.EntityFrameworkCore` 9.0.10
- `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.4
- `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.0
- `Microsoft.AspNetCore.SignalR` (included in ASP.NET Core)

### Frontend
- `next` 16.0.0
- `react` 19.0.0
- `@tanstack/react-query` 5.62.8
- `@microsoft/signalr` 8.0.7
- `axios` 1.7.9
- `tailwindcss` 3.4.17

## 📝 Development Notes

- **Demo credentials:** Hardcoded (`demo`/`demo`) in `backend/AlertService.Api/Controllers/AuthController.cs` for demonstration only. In production, implement a database-backed user system.
- **JWT SecretKey:** Configured in `appsettings.json`. In production, move to Azure Key Vault or environment variables.
- **Database:** PostgreSQL password is in `appsettings.json`. In production, use a secrets manager.
- **Simulator:** Generates random values between 30-70°C and 50-100% every 4 seconds.
- **SignalR:** Notifies all connected clients when an alert is generated.
- **Pagination:** 10 items per page, configurable in `frontend/components/AlertsTable.tsx`.

---

## 👨‍💻 Author

**Daniel Steven Diaz**

Industrial monitoring system developed with ASP.NET Core, Next.js, and SignalR.
