# Car Dealership API

A dealership management system built with **.NET 9 Web API**, using **Entity Framework Core (SQLite)**, **JWT Authentication**, and **OTP-based security**.

---

## How to Run

1. Clone the repo or extract the project.
2. Make sure you have **.NET 9 SDK** installed.
3. Update `appsettings.json` if needed (default SQLite + JWT config works).
4. From the project root, run:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
5. Open Swagger UI:
   - `http://localhost:5118/swagger/index.html`

---

## Default Credentials

- **Admin**
  - Email: `admin@demo.local`
  - Password: `P@ssw0rd!`

---

## API Endpoints

### Authentication

- `POST /api/Auth/request-otp` → Request OTP (register/login)
- `POST /api/Auth/register` → Register new customer (OTP required)
- `POST /api/Auth/login` → Login (OTP required, returns JWT)

### Admin

- `POST /api/Vehicles/add` → Add vehicle
- `PUT /api/Vehicles/update` → Update vehicle (OTP required)
- `GET /api/Admin/customers` → List all customers
- `POST /api/Admin/process-sale/{purchaseId}?approve=true|false` → Approve/Decline purchase

### Customer

- `GET /api/Vehicles/browse` → Browse available vehicles
- `GET /api/Vehicles/{id}` → Vehicle details
- `POST /api/Purchases/request` → Request purchase (OTP required)
- `GET /api/Purchases/history` → View purchase history

---

## Assumptions & Design

- **OTP**: Console-based simulation (printed when generated).
- **Roles**: Admin is seeded; Customers created via register.
- **Security**:
  - JWT with role-based authorization.
  - OTP required for critical actions: Register, Login, Purchase, Update Vehicle.
- **Architecture**:
  - **Controllers** → API endpoints.
  - **Services** → JWT & OTP logic.
  - **DTOs** → Request/response contracts.
  - **EF Core** → Data persistence with SQLite.

---

## ✨ Bonus Features

- ✅ Swagger/OpenAPI with JWT support
- ✅ Logging with `ILogger<T>`
- ✅ Input validation using DataAnnotations
- ✅ Configuration via `appsettings.json`
- 🐳 Docker-ready (`Dockerfile` included)

---

## Example Flow

1. Request OTP for registration → `POST /api/Auth/request-otp`
2. Register → `POST /api/Auth/register`
3. Request OTP for login → `POST /api/Auth/request-otp`
4. Login → `POST /api/Auth/login`
5. Use JWT token in Swagger (Authorize button)
6. Browse cars → `GET /api/Vehicles/browse`
7. Request OTP for purchase → `POST /api/Otp/request`
8. Submit purchase → `POST /api/Purchases/request`
9. Admin processes purchase → `POST /api/Admin/process-sale/{id}`

---
