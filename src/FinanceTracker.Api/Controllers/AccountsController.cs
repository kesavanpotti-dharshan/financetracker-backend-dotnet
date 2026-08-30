using System.Security.Claims;
using FinanceTracker.Application.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public class AccountsController(AccountHandlers handlers) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await handlers.GetAllAsync(UserId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await handlers.GetByIdAsync(id, UserId);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountCommand cmd)
    {
        var created = await handlers.CreateAsync(cmd, UserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAccountCommand cmd)
    {
        if (id != cmd.Id) return BadRequest();
        var updated = await handlers.UpdateAsync(cmd, UserId);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var success = await handlers.ArchiveAsync(id, UserId);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("{id}/balance")]
    public async Task<IActionResult> UpdateBalance(Guid id, UpdateBalanceCommand cmd)
    {
        if (id != cmd.AccountId) return BadRequest();
        var updated = await handlers.UpdateBalanceAsync(cmd, UserId);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet("{id}/credit-card")]
    public async Task<IActionResult> GetCreditCardDetails(Guid id)
    {
        var details = await handlers.GetCreditCardDetailsAsync(id, UserId);
        return details is null ? NotFound() : Ok(details);
    }

    [HttpPut("{id}/credit-card")]
    public async Task<IActionResult> SetCreditCardDetails(Guid id, SetCreditCardDetailsCommand cmd)
    {
        if (id != cmd.AccountId) return BadRequest();
        var details = await handlers.SetCreditCardDetailsAsync(cmd, UserId);
        return details is null ? NotFound() : Ok(details);
    }
}