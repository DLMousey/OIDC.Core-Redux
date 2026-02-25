using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core.DAL.Entities;
using OIDC.Core.DAL.ViewModels.Controllers.AuthenticationController;
using OIDC.Core.Services.Interface;
using OIDC.Core.Util.Metrics;

namespace OIDC.Core.Controllers;

[ApiController]
[Route("/authentication")]
[AllowAnonymous]
public class AuthenticationController(
    IUserService userService, 
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    APIEvents apiEvents,
    ILogger<AuthenticationController> logger,
    IAuthenticationEventService authenticationEventService
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
            await authenticationEventService.RecordAuthenticationEvent(false, null, HttpContext.Connection.RemoteIpAddress);
            return BadRequest("Invalid credentials provided");
        }

        RefreshToken? token = await refreshTokenService.FindCurrentAsync(user);
        if (token == null)
        {
            token = await refreshTokenService.CreateAsync(user);
        }
        
        if (userService.ValidateCredentials(user, vm.Password))
        {
            logger.LogInformation("Recorded successful authentication attempt");
            await authenticationEventService.RecordAuthenticationEvent(true, user, HttpContext.Connection.RemoteIpAddress);
            apiEvents.RecordAuthenticationAttempt(user);
            return Ok(new
            {
                access_token = jwtService.GenerateJwt(user),
                expires_in = 300,
                refresh_token = token.Code
            });
        }

        logger.LogInformation("Recorded failed authentication attempt");
        apiEvents.RecordAuthenticationAttempt(user, "credentials", false);
        await authenticationEventService.RecordAuthenticationEvent(false, user, HttpContext.Connection.RemoteIpAddress);
        return BadRequest("Invalid credentials provided");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshAsyncViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            apiEvents.RecordAuthenticationAttempt(null, "refresh", false);
            return BadRequest(ModelState);
        }

        RefreshToken? token = await refreshTokenService.FindAsync(vm.RefreshToken);
        if (token == null)
        {
            apiEvents.RecordAuthenticationAttempt(null, "refresh", false);
            return BadRequest("Invalid refresh token provided");
        }

        await refreshTokenService.RecordUse(token);
        apiEvents.RecordAuthenticationAttempt(token.User, "refresh");
        
        return Ok(new {
            access_token = jwtService.GenerateJwt(token.User),
            expires_in = 300,
        });
    }
    
}