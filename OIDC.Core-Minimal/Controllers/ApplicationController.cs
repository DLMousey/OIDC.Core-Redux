using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.ApplicationController;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Controllers;

[ApiController]
[Route("/applications")]
[Authorize]
public class ApplicationController(
    IApplicationService applicationService,
    IUserService userService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPublishedAsync()
    {
        try
        {
            User user = await userService.GetFromContextAsync(User);
            IList<Application> applications = await applicationService.GetPublishedAsync(user);
            return Ok(applications);
        }
        catch
        {
            return new JsonResult("An error occurred, please try again later")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAsyncViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            User user = await userService.GetFromContextAsync(User);
            Application application = await applicationService.CreateAsync(vm, user);

            string secret = application.ClientSecret;
            return Created("/applications/" + application.Id, new {
                application,
                clientSecret = secret
            });
        }
        catch
        {
            return new JsonResult("An error occurred, please try again later")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}