
# Exchange Rates App

Full-stack application for displaying CNB exchange rates.

## Project Structure

- `backend/` - .NET API
- `frontend/` - Angular UI

## Prerequisites

- .NET SDK 6.0 installed locally for the backend
- Node.js 18+ and npm

## Backend

The backend is the API layer that calls the CNB exchange rates endpoint and returns normalized JSON to the frontend.

### Build

From the `backend` folder:

```bash
dotnet build ExchangeRates.sln
```

If the build fails with an SDK version error, install the .NET 6 SDK first. The repository is pinned to .NET 6 through `global.json`.

### Run


## Backend
From the `backend` folder:

```bash
dotnet restore
dotnet run --project src/ExchangeRates.Api/ExchangeRates.Api.csproj
```

The API is configured through backend app settings and exposes the exchange rates endpoint used by the frontend.

## Frontend

The frontend is an Angular 17 app that loads data from the backend API and renders it in a table.

### Install dependencies

From the `frontend` folder:

```bash
npm install
```

### Build

```bash
npm run build
```

### Run

```bash
npm start
```

The app runs on `http://localhost:4200` during development.

## Development Notes

- The frontend does not call the CNB API directly; it calls the backend API instead.
- `filteredRates` is the array used by the Angular table to render rows.
- The `th` elements define table headers and the `td` elements render each row value.
- Environment/configuration values should be changed in the backend `appsettings` files and the frontend `environment` files, not hardcoded in components.

## Assumptions

- The backend API is running before opening the frontend in the browser.
- Local development may use a proxy or local environment URL to connect Angular to the backend.
- The API response shape includes a `rates` array that the UI can render and filter.
