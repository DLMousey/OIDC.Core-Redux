using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.DAL.ViewModels.Controllers.ApplicationController;
using OIDC.Core_Redux.Services.Interface;

namespace OIDC.Core_Redux.Controllers;

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

    [HttpGet("authorised")]
    public async Task<IActionResult> GetAuthorisedAsync()
    {
        try
        {
            User user = await userService.GetFromContextAsync(User);
            IList<Application> applications = await applicationService.GetAuthorisedAsync(user);
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id)
    {
        return Ok();
    }
}