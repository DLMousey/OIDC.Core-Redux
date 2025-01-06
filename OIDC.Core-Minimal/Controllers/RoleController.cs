using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.RolesController;
using OIDC.Core_Minimal.Services.Interface;
using OIDC.Core_Minimal.Util.Annotations;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/roles")]
[Authorize]
public class RoleController(
    IRoleService roleService,
    IUserService userService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        return Ok(await roleService.ListRolesAsync());
    }

    [HttpPost("attach")]
    [RequireRole("Administrator")]
    public async Task<IActionResult> AttachRoleAsync([FromBody] ModifyUserRoleViewModel vm)
    {
        User? user = await userService.FindByIdAsync(vm.UserId);
        if (user == null)
        {
            return BadRequest("Invalid user id provided");
        }

        Role? role = await roleService.FindAsync(vm.RoleId);
        if (role == null)
        {
            return BadRequest("Invalid role id provided");
        }

        await roleService.AttachRoleAsync(user, role);
        return NoContent();
    }

    [HttpPost("detach")]
    [RequireRole("Administrator")]
    public async Task<IActionResult> DetachRoleAsync([FromBody] ModifyUserRoleViewModel vm)
    {
        User? user = await userService.FindByIdAsync(vm.UserId);
        if (user == null)
        {
            return BadRequest("Invalid user id provided");
        }

        Role? role = await roleService.FindAsync(vm.RoleId);
        if (role == null)
        {
            return BadRequest("Invalid role id provided");
        }

        await roleService.DetachRoleAsync(user, role);
        return NoContent();
    }
}