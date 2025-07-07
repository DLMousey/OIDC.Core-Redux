using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.DAL.ViewModels.Services.MailService;

namespace OIDC.Core_Redux.Services.Interface;

public interface IMailService
{
    public Task SendToUserAsync(EmailViewModel viewModel, User user);

    public Task SendToEmailAsync(EmailViewModel viewModel);
}