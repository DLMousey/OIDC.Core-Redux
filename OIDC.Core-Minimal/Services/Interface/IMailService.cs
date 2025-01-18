using OIDC.Core_Minimal.DAL.Entities;
using OIDC.Core_Minimal.DAL.ViewModels.Services.MailService;

namespace OIDC.Core_Minimal.Services.Interface;

public interface IMailService
{
    public Task SendToUserAsync(EmailViewModel viewModel, User user);

    public Task SendToEmailAsync(EmailViewModel viewModel);
}