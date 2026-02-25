using OIDC.Core.DAL.Entities;
using OIDC.Core.DAL.ViewModels.Services.MailService;

namespace OIDC.Core.Services.Interface;

public interface IMailService
{
    public Task SendToUserAsync(EmailViewModel viewModel, User user);

    public Task SendToEmailAsync(EmailViewModel viewModel);
}