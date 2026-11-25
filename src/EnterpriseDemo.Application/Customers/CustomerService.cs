using System.Net.Mail;
using EnterpriseDemo.Application.Abstractions;
using EnterpriseDemo.Application.Common;
using EnterpriseDemo.Domain.Entities;

namespace EnterpriseDemo.Application.Customers;

public sealed class CustomerService(ICustomerRepository repository)
{
    public async Task<PagedResponse<CustomerResponse>> SearchAsync(string? search, int pageNumber, int pageSize, CancellationToken ct)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var (items, total) = await repository.SearchAsync(search?.Trim(), pageNumber, pageSize, ct);
        return new(items.Select(Map).ToArray(), pageNumber, pageSize, total);
    }

    public async Task<ServiceResult<CustomerResponse>> GetAsync(Guid id, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(id, ct);
        return customer is null
            ? ServiceResult<CustomerResponse>.Failure("Customer was not found.", 404)
            : ServiceResult<CustomerResponse>.Success(Map(customer));
    }

    public async Task<ServiceResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        var validation = Validate(request.FirstName, request.LastName, request.Email);
        if (validation is not null) return ServiceResult<CustomerResponse>.Failure(validation, 400);
        var email = request.Email.Trim().ToLowerInvariant();
        if (await repository.GetByEmailAsync(email, ct) is not null)
            return ServiceResult<CustomerResponse>.Failure("A customer with this email already exists.", 409);

        Customer customer;
        try { customer = new(request.FirstName, request.LastName, email); }
        catch (ArgumentException ex) { return ServiceResult<CustomerResponse>.Failure(ex.Message, 400); }
        await repository.AddAsync(customer, ct);
        await repository.SaveChangesAsync(ct);
        return ServiceResult<CustomerResponse>.Success(Map(customer), 201);
    }

    public async Task<ServiceResult<CustomerResponse>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken ct)
    {
        var validation = Validate(request.FirstName, request.LastName, request.Email);
        if (validation is not null) return ServiceResult<CustomerResponse>.Failure(validation, 400);
        var customer = await repository.GetByIdAsync(id, ct);
        if (customer is null) return ServiceResult<CustomerResponse>.Failure("Customer was not found.", 404);
        var email = request.Email.Trim().ToLowerInvariant();
        var existing = await repository.GetByEmailAsync(email, ct);
        if (existing is not null && existing.Id != id)
            return ServiceResult<CustomerResponse>.Failure("A customer with this email already exists.", 409);
        try { customer.Update(request.FirstName, request.LastName, email); }
        catch (ArgumentException ex) { return ServiceResult<CustomerResponse>.Failure(ex.Message, 400); }
        await repository.SaveChangesAsync(ct);
        return ServiceResult<CustomerResponse>.Success(Map(customer));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(id, ct);
        if (customer is null) return ServiceResult<bool>.Failure("Customer was not found.", 404);
        repository.Remove(customer);
        await repository.SaveChangesAsync(ct);
        return ServiceResult<bool>.Success(true, 204);
    }

    private static string? Validate(string first, string last, string email)
    {
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last) || string.IsNullOrWhiteSpace(email))
            return "First name, last name, and email are required.";
        if (first.Trim().Length > 80 || last.Trim().Length > 80 || email.Trim().Length > 254)
            return "One or more fields exceed the allowed length.";
        try { _ = new MailAddress(email.Trim()); }
        catch (FormatException) { return "A valid email address is required."; }
        return null;
    }

    private static CustomerResponse Map(Customer c) => new(c.Id, c.FirstName, c.LastName, c.FullName, c.Email, c.CreatedUtc);
}
