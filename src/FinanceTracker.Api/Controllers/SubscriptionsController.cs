using System.Security.Claims;
using FinanceTracker.Application.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/subscriptions")]
public class SubscriptionsController(SubscriptionHandlers handlers) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await handlers.GetAllAsync(UserId));

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int days = 30) => Ok(await handlers.GetUpcomingAsync(UserId, days));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary() => Ok(await handlers.GetSummaryAsync(UserId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var subscription = await handlers.GetByIdAsync(id, UserId);
        return subscription is null ? NotFound() : Ok(subscription);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionCommand cmd)
    {
        var created = await handlers.CreateAsync(cmd, UserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSubscriptionCommand cmd)
    {
        if (id != cmd.Id) return BadRequest();
        var updated = await handlers.UpdateAsync(cmd, UserId);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var success = await handlers.CancelAsync(id, UserId);
        return success ? NoContent() : NotFound();
    }
}
