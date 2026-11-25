# Architecture notes

## Request flow

`CustomersController` handles HTTP concerns and maps transport contracts to application requests. `CustomerService` owns use-case orchestration and validation. `ICustomerRepository` is the application boundary. `CustomerRepository` implements that boundary with EF Core. `Customer` protects basic invariants and normalizes values.

## Error handling

Expected outcomes are returned as `ServiceResult<T>` and mapped to HTTP status codes by the controller. Unexpected exceptions are logged and converted to a Problem Details response by `ExceptionHandlingMiddleware`.

## Production hardening checklist

- Use EF Core migrations rather than `EnsureCreated`.
- Add authentication and authorization through an OIDC provider (for example Microsoft Entra ID).
- Apply rate limiting, request-size limits, CORS policy, and security headers.
- Add integration tests against a real SQL Server-compatible database.
- Store secrets in a managed secret store; never commit credentials.
- Add observability (structured logs, metrics, distributed tracing) and readiness/liveness probes.
