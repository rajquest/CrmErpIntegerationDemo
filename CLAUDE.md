# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Solution Overview

This is a CRM/ERP integration demo with two projects:
- **`API/`** — ASP.NET Core Web API (.NET 10) bridging Salesforce (CRM) and Infor ERP
- **`NgUi/`** — Angular 21 frontend with Angular Material, Bootstrap 5, and SSR

## Commands

### API (run from `API/` directory)
```bash
dotnet build
dotnet run
dotnet test          # no test project exists yet
```

### Angular (run from `NgUi/` directory)
```bash
npm start            # ng serve → http://localhost:4200
npm run build        # ng build (production)
npm run watch        # ng build --watch (dev mode)
npm test             # ng test (Vitest)
```

## Environment Configuration

Copy `API/local.env.example` to `API/local.env` and fill in values. `local.env` is gitignored. In Docker, env vars are injected by the container runtime.

Required env vars use `__` as the section separator (e.g., `InforErp__TokenUrl`). All settings are accessed through `AppSettingsManager` (singleton), which reads `appsettings.json` and overlays from `local.env`/environment variables via `IConfiguration`.

The Angular API base URL is configured in `NgUi/src/environments/environment.ts` (`apiBaseUrl`).

## Architecture

### Authentication Flow

Authentication is two-step for Infor ERP:
1. **OAuth token** — `GET /InforErp/GetToken` → Infor SSO client-credentials grant → stored in `sessionStorage` as `infor_access_token`
2. **ERP API token** — `POST /InforErp/GetApiToken` with the OAuth token → exchanges for an IDO session token → stored as `infor_api_access_token`

Salesforce uses a single OAuth client-credentials token stored as `sf_access_token`.

The Angular `TokenService` (`NgUi/src/app/Services/token.service.ts`) manages all token retrieval and sessionStorage reads.

### Backend Request Pattern

Most controller endpoints follow this pattern:
- `[RequiresBearerToken]` action filter validates that `bearerToken` is non-empty before the action runs
- Bearer token is passed as `[FromBody] string bearerToken` (the ERP API token, not the OAuth token)
- The filter is in `API/Filters/RequiresBearerTokenAttribute.cs`

Controllers are organized by domain:
- `Controllers/Auth/` — token endpoints (Infor, Salesforce)
- `Controllers/Infor/` — customer, tax, ERP explorer
- `Controllers/Lookup/` — dropdown values
- Root-level controllers — inventory, invoice orders, job variance, Salesforce data

### Backend Service Layer

`InforErpService` is the generic IDO data accessor — it calls the Infor IDO REST API (`/csi/IDORequestService/ido/load/{idoName}`) with property lists and optional filter strings. Domain services (e.g., `ItemLotLocationService`, `CustomerService`) compose on top of it.

`IInforTokenService` has three token methods:
- `GetClientCredentialTokenAsync()` — client-credentials OAuth
- `GetAccessTokenAsync()` — password-grant OAuth
- `GetErpApiTokenAsync(bearerToken)` — IDO session token exchange

The `"InforApi"` named `HttpClient` is configured with optional SSL bypass (controlled by `InforErp:DisableSslValidation` in config).

### Frontend Service Pattern

All Angular services extend `BaseService` (`NgUi/src/app/Services/base.service.ts`), which wraps `HttpClient` with `catchError`. API calls use `environment.apiBaseUrl` as the base.

Routes use lazy-loaded standalone components. Each feature has a corresponding service in `NgUi/src/app/Services/`.

### Key Libraries

- **Backend**: `NetCoreForce.Client` (Salesforce), `InforIonClientLibrary` (Infor IDO), `AutoMapper`, `DotNetEnv`, `Swashbuckle`
- **Frontend**: Angular Material, Bootstrap 5, `xlsx` (Excel export)
