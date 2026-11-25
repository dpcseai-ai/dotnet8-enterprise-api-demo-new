using EnterpriseDemo.Api.Contracts;
using EnterpriseDemo.Application.Customers;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController(CustomerService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await service.SearchAsync(search, pageNumber, pageSize, ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetAsync(id, ct);
        return result.Succeeded ? Ok(result.Value) : NotFound(new ProblemDetails { Title = result.Error, Status = result.StatusCode });
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateCustomerHttpRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(new(request.FirstName, request.LastName, request.Email), ct);
        if (!result.Succeeded) return StatusCode(result.StatusCode, new ProblemDetails { Title = result.Error, Status = result.StatusCode });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerHttpRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, new(request.FirstName, request.LastName, request.Email), ct);
        return result.Succeeded ? Ok(result.Value) : StatusCode(result.StatusCode, new ProblemDetails { Title = result.Error, Status = result.StatusCode });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.Succeeded ? NoContent() : NotFound(new ProblemDetails { Title = result.Error, Status = result.StatusCode });
    }
}
