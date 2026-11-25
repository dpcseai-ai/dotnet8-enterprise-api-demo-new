namespace EnterpriseDemo.Domain.Entities;

public sealed class Customer
{
    private Customer() { } // EF Core

    public Customer(string firstName, string lastName, string email)
    {
        Id = Guid.NewGuid();
        Update(firstName, lastName, email);
        CreatedUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime CreatedUtc { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = Required(firstName, nameof(firstName), 80);
        LastName = Required(lastName, nameof(lastName), 80);
        Email = Required(email, nameof(email), 254).ToLowerInvariant();
    }

    private static string Required(string value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{field} is required.", field);
        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentException($"{field} cannot exceed {maxLength} characters.", field);
        return normalized;
    }
}
