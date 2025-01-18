using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.ScopeController;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/scopes")]
[AllowAnonymous]
public class ScopeController(IScopeService scopeService, ILogger<ScopeController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        logger.LogInformation("Retrieving scopes");
        IList<Scope> scopes = await scopeService.FindAllAsync();
        return Ok(scopes);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetAsync(string name)
    {
        Scope? scope = await scopeService.FindAsync(name);
        if (scope == null)
        {
            return NotFound($"No scope found matching name: {name}");
        }

        return Ok(scope);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAsyncViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Created($"/scopes/{vm.Name}", await scopeService.CreateAsync(vm.Name));
    }

    [HttpDelete("{name}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync(string name)
    {
        scopeService.Delete(name);
        return NoContent();
    }
}