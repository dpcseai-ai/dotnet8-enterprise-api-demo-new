using System.ComponentModel.DataAnnotations;

namespace EnterpriseDemo.Api.Contracts;

public sealed record CreateCustomerHttpRequest(
    [property: Required, StringLength(80, MinimumLength = 1)] string FirstName,
    [property: Required, StringLength(80, MinimumLength = 1)] string LastName,
    [property: Required, EmailAddress, StringLength(254)] string Email);

public sealed record UpdateCustomerHttpRequest(
    [property: Required, StringLength(80, MinimumLength = 1)] string FirstName,
    [property: Required, StringLength(80, MinimumLength = 1)] string LastName,
    [property: Required, EmailAddress, StringLength(254)] string Email);
