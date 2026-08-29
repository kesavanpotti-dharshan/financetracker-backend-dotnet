using FinanceTracker.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RegisterHandler register, LoginHandler login, RefreshHandler refresh, LogoutHandler logout) : ControllerBase
{
    private const string CookieName = "refreshToken";

    private void SetRefreshCookie(string token, DateTime expiresAt)
    {
        Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,          // requires HTTPS — fine on Railway/Vercel
            SameSite = SameSiteMode.None, // frontend and backend are different domains
            Expires = expiresAt
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand cmd)
    {
        var (result, raw, expiresAt) = await register.HandleAsync(cmd);
        SetRefreshCookie(raw, expiresAt);
        return Ok(new { accessToken = result.AccessToken, email = result.User.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand cmd)
    {
        var (result, raw, expiresAt) = await login.HandleAsync(cmd);
        SetRefreshCookie(raw, expiresAt);
        return Ok(new { accessToken = result.AccessToken, email = result.User.Email });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var raw) || string.IsNullOrEmpty(raw))
            return Unauthorized();

        var (result, newRaw, expiresAt) = await refresh.HandleAsync(raw);
        SetRefreshCookie(newRaw, expiresAt);
        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(CookieName, out var raw))
        {
            await logout.HandleAsync(raw);
            Response.Cookies.Delete(CookieName);
        }
        return Ok();
    }
}