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

> The Angular dev server proxies all `/api` requests to the backend via `proxy.conf.json`.

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
