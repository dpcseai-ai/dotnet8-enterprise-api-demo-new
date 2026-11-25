namespace EnterpriseDemo.Application.Customers;

public sealed record CreateCustomerRequest(string FirstName, string LastName, string Email);
public sealed record UpdateCustomerRequest(string FirstName, string LastName, string Email);
public sealed record CustomerResponse(Guid Id, string FirstName, string LastName, string FullName, string Email, DateTime CreatedUtc);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
