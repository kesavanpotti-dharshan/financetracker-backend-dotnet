using System.Security.Claims;
using FinanceTracker.Application.Statements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/statements")]
public class StatementsController(StatementHandlers handlers) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost("upload/{accountId}")]
    [RequestSizeLimit(10_000_000)] // 10MB cap on statement PDFs
    public async Task<IActionResult> Upload(Guid accountId, IFormFile file)
    {
        if (file.Length == 0) return BadRequest("Empty file");
        var result = await handlers.UploadAsync(accountId, UserId, file.OpenReadStream(), file.FileName);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var statement = await handlers.GetByIdAsync(id, UserId);
        return statement is null ? NotFound() : Ok(statement);
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, ConfirmStatementCommand cmd)
    {
        if (id != cmd.StatementId) return BadRequest();
        var balance = await handlers.ConfirmAsync(cmd, UserId);
        return balance is null ? NotFound() : Ok(balance);
    }
}