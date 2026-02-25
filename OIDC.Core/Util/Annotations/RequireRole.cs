using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OIDC.Core.Util.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRole : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public RequireRole(string? roles = null)
    {
        _roles = [];
        if (roles == null)
        {
            return;
        }

        string[] roleNames = roles.Split(",");
        for (int i = 0; i < roleNames.Length; i++)
        {
            roleNames[i] = roleNames[i].Trim().ToLower();
        }

        _roles = roles.Split(", ");
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ClaimsPrincipal identity = context.HttpContext.User;
        Claim? roleClaim = identity.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role);
        
        // No roles required, doesn't matter what roles the user has, return early without action
        if (_roles.Length == 0)
        {
            return;
        }

        if (roleClaim == null)
        {
            context.Result = new JsonResult(new
            {
                status = 403,
                message = "You do not have permission to access this resource"
            }) { StatusCode = StatusCodes.Status403Forbidden };
            return; // Explicit redundant return
        }
        
        bool hasRoles = true;
        foreach(string roleName in _roles)
        {
            if (!roleClaim.Value.Split(", ").ToArray().Contains(roleName))
            {
                hasRoles = false;
            }
        }

        if (hasRoles == false)
        {
            context.Result = new JsonResult(new
            {
                status = 403,
                message = "You do not have permission to access this resource"
            }) { StatusCode = StatusCodes.Status403Forbidden };
            return; // Explicit redundant return
        }

        // The user has all the roles if they made it here without tripping the hasRoles catch
        // no action is required.
    }
}