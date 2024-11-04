using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.AuthenticationController;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/authentication")]
[AllowAnonymous]
public class AuthenticationController(
    IUserService userService, 
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService
) : ControllerBase
{
    [HttpPost("")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateAsyncViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        User? user = await userService.FindByEmailAsync(vm.Email);
        if (user == null)
        {
            return BadRequest("Invalid credentials provided");
        }

        RefreshToken? token = await refreshTokenService.FindCurrentAsync(user);
        if (token == null)
        {
            token = await refreshTokenService.CreateAsync(user);
        }
        
        if (userService.ValidateCredentials(user, vm.Password))
        {
            return Ok(new
            {
                access_token = jwtService.GenerateJwt(user),
                expires_in = 300,
                refresh_token = token.Code
            });
        }

        return BadRequest("Invalid credentials provided");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshAsyncViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        RefreshToken? token = await refreshTokenService.FindAsync(vm.RefreshToken);
        if (token == null)
        {
            return BadRequest("Invalid refresh token provided");
        }

        await refreshTokenService.RecordUse(token);

        return Ok(new {
            access_token = jwtService.GenerateJwt(token.User),
            expires_in = 300,
        });
    }
    
}