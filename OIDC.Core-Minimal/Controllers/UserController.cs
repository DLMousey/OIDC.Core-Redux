using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.UserController;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/users")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAsyncViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            DAL.Entities.User user = await userService.CreateAsync(vm);
            return Created("/users", user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("private")]
    public async Task<IActionResult> AuthTest()
    {
        DAL.Entities.User user = await userService.GetFromContextAsync(User);
        return Ok(user);
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicTest()
    {
        return Ok("Hello from public endpoint");
    }
}