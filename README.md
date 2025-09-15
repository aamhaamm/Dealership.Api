# Car Dealership API

A simple dealership management system built with **.NET 9 Web API**, using **Entity Framework Core (SQLite)**, **JWT Authentication**, and **OTP-based security**.

---

## How to Run

1. Clone the repo or extract the project.
2. Make sure you have **.NET 9 SDK** installed.
3. Set up `appsettings.json` (already has default SQLite + JWT config).
4. From the project root, run:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
5. The API will be available at:
   - Swagger UI → `https://localhost:7118/swagger`

---

## Available Endpoints

### Authentication

- `POST /api/Auth/request-otp` → Request OTP (register/login).
- `POST /api/Auth/register` → Register new customer (OTP required).
- `POST /api/Auth/login` → Login (OTP required, returns JWT).

### 👨Admin Use Cases

- `POST /api/Vehicles/add` → Add new vehicle (Admin only).
- `PUT /api/Vehicles/update` → Update vehicle details (Admin + OTP).
- `GET /api/Admin/customers` → List all registered customers (Admin only).

### Customer Use Cases

- `GET /api/Vehicles/browse` → Browse available vehicles.
- `GET /api/Vehicles/{id}` → View vehicle details.
- `POST /api/Purchases/request` → Purchase a vehicle (Customer + OTP).
- `GET /api/Purchases/history` → View purchase history.

---

## ⚙️ Assumptions & Design Decisions

- **OTP Simulation**: OTPs are printed in the console instead of sending SMS/Email.
- **Roles**:
  - Admin → seeded by default in the database (`admin@demo.local` / `P@ssw0rd!`).
  - Customer → created on registration.
- **Database**: SQLite used for simplicity, auto-created & seeded on first run.
- **Security**:
  - JWT authentication with role-based authorization.
  - OTP required for critical actions (Register, Login, Purchase, Update Vehicle).
- **Structure**:
  - Controllers → handle API requests.
  - DTOs → clean request/response models.
  - Services → encapsulate business logic (JWT, OTP).
  - EF Core → data access & persistence.

---

## Example Flow

1. `POST /api/Auth/request-otp` (register).
2. `POST /api/Auth/register` → creates new Customer.
3. Login with OTP → receive JWT.
4. Browse vehicles → `GET /api/Vehicles/browse`.
5. Request OTP for purchase → `POST /api/Otp/request`.
6. Complete purchase → `POST /api/Purchases/request`.
7. Check purchase history → `GET /api/Purchases/history`.
