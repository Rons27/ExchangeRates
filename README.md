# Exchange Rates Application

Full-stack application that displays daily exchange rates from the Czech National Bank (CNB).

---

## Architecture Overview

```
exchange-rates/
├── backend/                       # .NET 6 Clean-Architecture solution
│   ├── ExchangeRates.sln
│   └── src/
│       ├── ExchangeRates.Domain/          # Entities, domain models, exceptions
│       ├── ExchangeRates.Application/     # Interfaces, services, DTOs, queries
│       ├── ExchangeRates.Infrastructure/  # CNB HTTP client, DI wiring
│       └── ExchangeRates.Api/             # Controllers, middleware, Startup
│
├── frontend/                      # Angular 17 standalone-component app
│   └── src/app/
│       ├── core/                  # Services, interceptors, models
│       ├── shared/                # Reusable UI components
│       └── features/exchange-rates/  # Page component + SCSS
│
└── README.md
```


## Backend

### Option 1 — Visual Studio 2022

1. Install **.NET 6 SDK** from https://dotnet.microsoft.com/download/dotnet/6.0
2. Open `backend/ExchangeRates.sln` in **Visual Studio 2022**
3. Set `ExchangeRates.Api` as the startup project
4. Press **F5** or click **Start**

> Use Visual Studio 2022 .

### Option 2 — VS Code / Terminal

From the `backend` folder:

```bash
dotnet restore
dotnet run --project src/ExchangeRates.Api/ExchangeRates.Api.csproj
```

The API starts on:
- `http://localhost:5000`
- `https://localhost:5001`

---

## Frontend

From the `frontend` folder:

```bash
npm install
npm start
```

The app runs on `http://localhost:4200`.
