using System.Security.Claims;
using FinanceTracker.Application.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts/{accountId}/transactions")]
public class TransactionsController(TransactionHandlers handlers) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid accountId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to) =>
        Ok(await handlers.GetByAccountAsync(accountId, UserId, from, to));

    [HttpPost]
    public async Task<IActionResult> Create(Guid accountId, [FromBody] CreateTransactionCommand cmd)
    {
        if (accountId != cmd.AccountId) return BadRequest();
        var created = await handlers.CreateAsync(cmd, UserId);
        return created is null ? NotFound() : Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid accountId, Guid id, UpdateTransactionCommand cmd)
    {
        if (id != cmd.Id) return BadRequest();
        var updated = await handlers.UpdateAsync(cmd, UserId);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid accountId, Guid id)
    {
        var success = await handlers.DeleteAsync(id, UserId);
        return success ? NoContent() : NotFound();
    }
}