using System.Security.Claims;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users/me")]
public class UserSettingsController(UserSettingsHandlers handlers, IExchangeRateService fx) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await handlers.GetAsync(UserId);
        return settings is null ? NotFound() : Ok(settings);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(UpdateSecondaryCurrencyCommand cmd)
    {
        var settings = await handlers.UpdateSecondaryCurrencyAsync(UserId, cmd);
        return settings is null ? NotFound() : Ok(settings);
    }

    [HttpGet("fx-rate")]
    public async Task<IActionResult> GetRate([FromQuery] string from, [FromQuery] string to)
    {
        var rate = await fx.GetRateAsync(from, to);
        return rate is null ? NotFound() : Ok(new { from, to, rate });
    }
}