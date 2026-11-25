using EnterpriseDemo.Domain.Entities;

namespace EnterpriseDemo.Application.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<(IReadOnlyList<Customer> Items, int TotalCount)> SearchAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Remove(Customer customer);
}
