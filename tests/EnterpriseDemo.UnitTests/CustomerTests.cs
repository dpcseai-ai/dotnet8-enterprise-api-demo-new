using Xunit;
using EnterpriseDemo.Application.Abstractions;
using EnterpriseDemo.Application.Customers;
using EnterpriseDemo.Domain.Entities;

namespace EnterpriseDemo.UnitTests;

public sealed class CustomerTests
{
    [Fact]
    public void Constructor_TrimsValuesAndNormalizesEmail()
    {
        var customer = new Customer(" Asha ", " Patel ", " ASHA@Example.com ");
        Assert.Equal("Asha", customer.FirstName);
        Assert.Equal("Patel", customer.LastName);
        Assert.Equal("asha@example.com", customer.Email);
        Assert.Equal("Asha Patel", customer.FullName);
    }

    [Fact]
    public void Update_RejectsBlankFirstName()
    {
        var customer = new Customer("Asha", "Patel", "asha@example.com");
        Assert.Throws<ArgumentException>(() => customer.Update(" ", "Patel", "asha@example.com"));
    }

    [Fact]
    public async Task CreateAsync_RejectsInvalidEmail()
    {
        var service = new CustomerService(new FakeCustomerRepository());
        var result = await service.CreateAsync(new("Asha", "Patel", "not-an-email"), CancellationToken.None);
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateEmail()
    {
        var repository = new FakeCustomerRepository();
        await repository.AddAsync(new Customer("Asha", "Patel", "asha@example.com"), CancellationToken.None);
        var service = new CustomerService(repository);
        var result = await service.CreateAsync(new("Asha", "Shah", "ASHA@example.com"), CancellationToken.None);
        Assert.False(result.Succeeded);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task SearchAsync_ClampsPageSizeToMaximum100()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);
        var result = await service.SearchAsync(null, 0, 500, CancellationToken.None);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(100, result.PageSize);
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = [];
        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(_customers.FirstOrDefault(x => x.Id == id));
        public Task<Customer?> GetByEmailAsync(string email, CancellationToken ct) => Task.FromResult(_customers.FirstOrDefault(x => x.Email == email));
        public Task<(IReadOnlyList<Customer> Items, int TotalCount)> SearchAsync(string? search, int pageNumber, int pageSize, CancellationToken ct)
        {
            IEnumerable<Customer> query = _customers;
            if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) || x.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            var total = query.Count();
            return Task.FromResult(((IReadOnlyList<Customer>)query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray(), total));
        }
        public Task AddAsync(Customer customer, CancellationToken ct) { _customers.Add(customer); return Task.CompletedTask; }
        public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
        public void Remove(Customer customer) => _customers.Remove(customer);
    }
}
