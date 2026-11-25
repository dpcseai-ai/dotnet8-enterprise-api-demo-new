# .NET 8 Enterprise API Demo

A portfolio-ready ASP.NET Core Web API demonstrating a maintainable layered architecture, customer CRUD, validation, centralized error handling, SQL Server persistence with EF Core, OpenAPI, unit tests, and GitHub Actions CI.

> **Demo project:** Review the security and deployment notes before using this as a production template. The API intentionally keeps authentication out of scope so the sample can focus on API design and maintainability; do not expose demo data or connection strings in a public deployment.

## Tech stack

- .NET 8 / C# 12 / ASP.NET Core Web API
- Entity Framework Core 8 + SQL Server
- Swagger / OpenAPI
- xUnit
- GitHub Actions

## Architecture

```text
src/
  EnterpriseDemo.Api/             HTTP endpoints, middleware, composition root
  EnterpriseDemo.Application/     Use cases, DTOs, validation, abstractions
  EnterpriseDemo.Domain/          Domain entities and business rules
  EnterpriseDemo.Infrastructure/  EF Core DbContext and persistence
 tests/
  EnterpriseDemo.UnitTests/       Domain and application unit tests
```

The dependency direction is inward: API and Infrastructure depend on Application; Application depends on Domain; Domain has no project dependencies.

## Features

- Create, read, update, and delete customers
- Search by name/email with page number and page size
- Email and required-field validation
- Duplicate email protection
- Consistent `ProblemDetails` error responses
- EF Core SQL Server persistence
- Development-only Swagger UI
- Unit tests for domain rules and service behavior
- CI workflow that restores, builds, and tests on pushes and pull requests

## Quick start

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, SQL Server Express, or a container)

### Configure the connection string

The default connection string is in `src/EnterpriseDemo.Api/appsettings.json`. Override it using the environment variable `ConnectionStrings__DefaultConnection` rather than committing credentials.

Example for local SQL Server:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=EnterpriseDemo;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Run

From the repository root:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet run --project src/EnterpriseDemo.Api
```

Open the Swagger URL printed by the application (typically `https://localhost:7xxx/swagger`). The database is created automatically on startup for this demo using `EnsureCreated`. For a production application, replace this with reviewed EF Core migrations and a controlled deployment migration step.

## API endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/customers?pageNumber=1&pageSize=20&search=` | Paged customer list |
| GET | `/api/customers/{id}` | Get customer |
| POST | `/api/customers` | Create customer |
| PUT | `/api/customers/{id}` | Update customer |
| DELETE | `/api/customers/{id}` | Delete customer |

### Example request

```http
POST /api/customers
Content-Type: application/json

{
  "firstName": "Asha",
  "lastName": "Patel",
  "email": "asha@example.com"
}
```

Successful creation returns `201 Created` with a `Location` header. Invalid requests return `400`; missing customers return `404`; duplicate email addresses return `409`.

## Example cURL

```bash
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Asha","lastName":"Patel","email":"asha@example.com"}'

curl "http://localhost:5000/api/customers?pageNumber=1&pageSize=10&search=asha"
```

## Tests

```bash
dotnet test
```

Unit tests use an in-memory fake repository, so they do not require SQL Server. Add integration tests with `WebApplicationFactory` and a disposable SQL Server/Testcontainers instance before production use.

## Publish as a public GitHub repository

1. Sign in to GitHub as `dpcseai-ai`.
2. Create a new repository named **`dotnet8-enterprise-api-demo`** and select **Public**. Do not initialize it with a README, license, or `.gitignore` because those files are already included here.
3. In this folder, run:

```bash
git init
git add .
git commit -m "Initial commit: .NET 8 enterprise API demo"
git branch -M main
git remote add origin https://github.com/dpcseai-ai/dotnet8-enterprise-api-demo.git
git push -u origin main
```

4. Confirm the repository is public and that the Actions workflow passes.

If Git asks for credentials, use Git Credential Manager or GitHub's browser-based sign-in. Do not put a personal access token in the remote URL or commit it to the repository.

## Suggested next improvements

- Replace `EnsureCreated` with EF Core migrations.
- Add JWT/OIDC authentication and role-based authorization using a real identity provider.
- Add integration tests, structured logging, health checks, and rate limiting.
- Add Docker Compose for API + SQL Server and deployment workflows for your hosting platform.
- Add a license only if you are comfortable granting reuse rights; MIT is a common choice for demo repositories.

## License

No license has been selected. Unless a license is added, standard copyright restrictions apply even though the repository is public.
