using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Controllers.UserController;
using OIDC.Core_Minimal.DAL.ViewModels.Services.MailService;
using OIDC.Core_Minimal.Services.Interface;

namespace OIDC.Core_Minimal.Services.Implementation;

public class UserService(
    OIDCCoreMinimalDbContext context,
    IMailService mailService
) : IUserService
{
    public async Task<User> CreateAsync(CreateAsyncViewModel vm)
    {
        User? duplicate = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals(vm.Email));
        if (duplicate != null)
        {
            throw new ArgumentException("Email already exists");
        }
        
        User user = new User
        {
            Email = vm.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
            Username = vm.Username,
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        string userJson = JsonSerializer.Serialize(user);
        Dictionary<string, string> emailData = new Dictionary<string, string>();
        emailData.Add("User", userJson);

        try
        {
            await mailService.SendToUserAsync(new EmailViewModel
            {
                Slug = "platform-welcome",
                Subject = "Welcome to OIDC.Core",
                ToName = user.Username,
                ToAddress = user.Email,
                User = user,
                Data = emailData
            }, user);
        }
        catch
        {
            // ignored
        }
        
        return user;
    }

    public async Task<User?> FindByEmailAsync(string email) => 
        await context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email.Equals(email));

    public async Task<User?> FindByIdAsync(Guid userId) => 
        await context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id.Equals(userId));

    public async Task<User> GetFromContextAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity == null)
        {
            throw new ArgumentException("Principal identity is null");
        }

        Claim? emailClaim = principal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));
        if (emailClaim == null)
        {
            throw new ArgumentException("Email claim missing from claim principal");
        }

        User? user = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
        if (user == null)
        {
            throw new ArgumentException("No user found matching email claim");
        }
        
        return user;
    }

    public bool ValidateCredentials(User user, string password) => BCrypt.Net.BCrypt.Verify(password, user.Password);

    // @TODO - consider how this will perform with a huge amount of users. Run some load testing
    public async Task<IList<User>> GetListAsync()
    {
        return await context.Users.ToListAsync();
    }
}