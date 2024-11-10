using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/scopes")]
[AllowAnonymous]
public class ScopeController(IScopeService scopeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
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
    public async Task<IActionResult> CreateAsync([FromBody] string name)
    {
        return Created($"/scopes/{name}", await scopeService.CreateAsync(name));
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteAsync(string name)
    {
        scopeService.Delete(name);
        return NoContent();
    }
}