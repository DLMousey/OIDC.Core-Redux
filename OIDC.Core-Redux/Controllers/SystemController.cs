using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/system")]
[AllowAnonymous]
public class SystemController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}