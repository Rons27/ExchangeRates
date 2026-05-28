# Exchange Rates Application

A production-quality full-stack application that proxies and displays daily exchange rates published by the **Czech National Bank (CNB)**. Built as a senior-level take-home technical assessment.

---

## Architecture Overview

```
exchange-rates/
├── backend/                       # .NET 8 Clean-Architecture solution
│   ├── ExchangeRates.sln
│   ├── src/
│   │   ├── ExchangeRates.Domain/          # Entities, domain models, exceptions
│   │   ├── ExchangeRates.Application/     # Interfaces, services, DTOs, queries
│   │   ├── ExchangeRates.Infrastructure/  # CNB HTTP client, Polly, DI wiring
│   │   └── ExchangeRates.Api/             # Controllers, middleware, Program.cs
│   └── tests/
│       └── ExchangeRates.Tests.Unit/      # xUnit + Moq + FluentAssertions
│
├── frontend/                      # Angular 17 standalone-component app
│   └── src/app/
│       ├── core/                  # Services, interceptors, models
│       ├── shared/                # Reusable UI components
│       └── features/exchange-rates/  # Page component + SCSS
│
└── README.md
```

### Layering (Dependency Rule)

```
Api  →  Application  →  Domain
         ↑
  Infrastructure  ───────────┘
```

- **Domain** — pure C# records and entities; zero external dependencies.
- **Application** — use-case logic, DTOs, interfaces. Depends only on Domain.
- **Infrastructure** — CNB HTTP client (typed `HttpClient`), Polly retry/timeout, DI registration. Depends on Application.
- **Api** — ASP.NET Core controllers, global exception middleware, Swagger. Composes Application + Infrastructure.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend runtime | .NET 8 |
| Web framework | ASP.NET Core Web API |
| HTTP resiliency | Polly (retry + timeout) |
| Serialisation | System.Text.Json |
| API docs | Swashbuckle (Swagger UI) |
| Unit tests | xUnit · Moq · FluentAssertions |
| Frontend | Angular 17 (standalone components) |
| Styling | SCSS + CSS custom properties |
| HTTP client | Angular `HttpClient` with functional interceptors |
| Reactivity | RxJS Observables |

---

## Prerequisites

| Tool | Minimum version |
|---|---|
| [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0 |
| [Node.js](https://nodejs.org/) | 18 LTS (20 LTS recommended) |
| [Angular CLI](https://angular.io/cli) | 17 (`npm i -g @angular/cli`) |

> **Note:** The machine that generated this project had .NET 5 SDK installed. The backend targets `net8.0`. Install the .NET 8 SDK before running the backend.

---

## Running the Backend

```bash
# 1 – Restore packages
cd backend
dotnet restore

# 2 – Run in development mode (listens on http://localhost:5000 by default)
dotnet run --project src/ExchangeRates.Api

# Swagger UI is available at:
#   http://localhost:5000  (root)
#   http://localhost:5000/swagger/v1/swagger.json
```

### Configuration

All settings live in `src/ExchangeRates.Api/appsettings.json`. Override with environment variables at deployment time:

| Key | Default | Description |
|---|---|---|
| `CnbApi__BaseUrl` | `https://api.cnb.cz` | CNB API base URL |
| `CnbApi__DailyRatesPath` | `/cnbapi/exrates/daily` | Relative endpoint path |
| `CnbApi__TimeoutSeconds` | `30` | Per-request HTTP timeout |
| `CnbApi__RetryCount` | `3` | Polly retry attempts |
| `Cors__AllowedOrigins__0` | `http://localhost:4200` | Angular dev-server origin |

---

## Running the Frontend

```bash
cd frontend

# Install dependencies (skip if already done by ng new)
npm install

# Start dev server with backend proxy (forwards /api → http://localhost:5000)
ng serve

# Open http://localhost:4200
```

The Angular dev-server proxies all `/api` requests to the backend via `proxy.conf.json`, so no CORS configuration is needed during development.

### Production build

```bash
ng build --configuration=production
# Output: frontend/dist/frontend/
```

---

## Running the Tests

```bash
cd backend

# Run all unit tests
dotnet test

# With coverage report
dotnet test --collect:"XPlat Code Coverage"
```

**Test coverage includes:**

| Test file | What is tested |
|---|---|
| `ExchangeRateServiceTests` | Filtering, sorting, DTO mapping, date forwarding |
| `ExchangeRatesControllerTests` | Input validation (date format, future dates, bad currency codes), response shape |
| `ExchangeRateProviderTests` | JSON parsing, empty list handling, HTTP error wrapping, date param forwarding |

---

## API Reference

### `GET /api/exchange-rates`

Returns CNB daily exchange rates.

**Query parameters**

| Parameter | Type | Example | Description |
|---|---|---|---|
| `date` | string | `2024-01-15` | Optional. Historical date (yyyy-MM-dd). Defaults to today. |
| `currency` | string | `EUR` | Optional. ISO 4217 code. Returns only that currency row. |

**Examples**

```
GET /api/exchange-rates
GET /api/exchange-rates?date=2024-01-15
GET /api/exchange-rates?currency=EUR
GET /api/exchange-rates?date=2024-01-15&currency=USD
```

**Success response — 200 OK**

```json
{
  "date": "2024-01-15",
  "baseCurrency": "CZK",
  "rates": [
    {
      "currencyCode": "EUR",
      "currency": "euro",
      "country": "Eurozone",
      "amount": 1,
      "rate": 25.34
    }
  ]
}
```

**Error responses**

| Status | Title | When |
|---|---|---|
| 400 | Invalid Date Parameter | Date is not `yyyy-MM-dd` or is in the future |
| 400 | Invalid Currency Parameter | Currency code is not exactly 3 characters |
| 502 | Upstream Provider Error | CNB API is unreachable or returns an error |
| 504 | Gateway Timeout | CNB API exceeds the configured timeout |
| 500 | Internal Server Error | Unexpected unhandled exception |

All errors follow RFC 7807 `ProblemDetails`:

```json
{
  "title": "Invalid Date Parameter",
  "detail": "'abc' is not a valid date. Expected format: yyyy-MM-dd.",
  "status": 400,
  "instance": "/api/exchange-rates"
}
```

---

## Docker Setup (Optional)

### Backend

```dockerfile
# backend/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ExchangeRates.sln
RUN dotnet publish src/ExchangeRates.Api -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ExchangeRates.Api.dll"]
```

### Frontend

```dockerfile
# frontend/Dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npx ng build --configuration=production

FROM nginx:alpine
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 80
```

### docker-compose.yml

```yaml
version: '3.9'
services:
  backend:
    build: ./backend
    ports: ['5000:8080']
    environment:
      - CnbApi__BaseUrl=https://api.cnb.cz
      - Cors__AllowedOrigins__0=http://localhost:4200

  frontend:
    build: ./frontend
    ports: ['4200:80']
    depends_on: [backend]
```

---

## Architectural Decisions

### Clean Architecture layers
The strict layer boundary (Domain ← Application ← Infrastructure → Api) means the business logic in `ExchangeRateService` is testable without any HTTP stack or database. The `IExchangeRateProvider` abstraction lets us swap or mock the CNB data source without touching Application code.

### Functional HTTP interceptor (Angular)
Angular 17 recommends functional interceptors (`HttpInterceptorFn`) over class-based ones. The `errorInterceptor` normalises all `HttpErrorResponse` objects into a typed `ApiError` record before they reach the component, keeping component code free of HTTP-plumbing concerns.

### Standalone components
Every Angular component is declared with `standalone: true`. There are no NgModules. This follows Angular 17 recommended style and allows tree-shaking at the component level.

### Polly retry + timeout
Transient CNB API failures are retried with exponential back-off (2^n seconds). A separate Polly timeout policy guards against slow responses. Both are configurable via `appsettings.json` so they can be tuned per environment.

### ProblemDetails everywhere
The global exception middleware converts all unhandled exceptions to RFC 7807 `ProblemDetails` JSON. The frontend's error interceptor extracts the `detail` field for display. This gives a consistent, consumer-friendly error contract.

### Lazy-loaded exchange rates module
The route configuration uses `loadComponent()` so the exchange rates bundle is only downloaded on navigation. This keeps the initial JS payload small.

---

## Assumptions

1. The CNB API is treated as an external dependency — responses are cached only in memory for the lifetime of a request (no persistent cache is added, though adding a short-lived `IMemoryCache` would be a natural next step).
2. The CNB only publishes rates on working days. Requests for weekends or public holidays silently return the last published rates — this is the API's own behaviour and is not masked.
3. The `amount` field on some currencies (e.g. Hungarian Forint — 100 HUF) is preserved in the UI so users understand the rate is per `amount` units, not per 1 unit.
4. HTTPS redirection is enabled in production; during local development HTTP is used to avoid certificate prompts.
