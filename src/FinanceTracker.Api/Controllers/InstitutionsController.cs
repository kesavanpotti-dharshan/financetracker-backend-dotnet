using FinanceTracker.Application.Institutions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/institutions")]
public class InstitutionsController(InstitutionHandlers handlers) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await handlers.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateInstitutionCommand cmd) => Ok(await handlers.CreateAsync(cmd));
}