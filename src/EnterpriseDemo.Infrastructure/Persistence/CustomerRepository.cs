using EnterpriseDemo.Application.Abstractions;
using EnterpriseDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseDemo.Infrastructure.Persistence;

public sealed class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct) => db.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<Customer?> GetByEmailAsync(string email, CancellationToken ct) => db.Customers.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> SearchAsync(string? search, int pageNumber, int pageSize, CancellationToken ct)
    {
        IQueryable<Customer> query = db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.Email.Contains(search));
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await db.Customers.AddAsync(customer, ct);
    }
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    public void Remove(Customer customer) => db.Customers.Remove(customer);
}
