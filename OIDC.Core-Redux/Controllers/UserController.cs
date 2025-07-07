using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.UserController;
using OIDC.Core_Minimal.Services.Interface;
using OIDC.Core_Minimal.Util.Annotations;

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
    
    /// <summary>
    /// This will need protecting by an RBAC attribute in the very near future, should be the next thing
    /// that's implemented on the backend.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [RequireRole("Administrator")]
    public async Task<IActionResult> GetListAsync()
    {
        IList<User> users = await userService.GetListAsync();
        return Ok(users);
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