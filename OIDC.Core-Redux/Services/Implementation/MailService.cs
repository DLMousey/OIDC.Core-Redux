using MailKit.Net.Smtp;
using MimeKit;
using OIDC.Core_Redux.DAL.Entities;
using OIDC.Core_Redux.DAL.ViewModels.Services.MailService;
using OIDC.Core_Redux.Services.Interface;
using RazorEngineCore;

namespace OIDC.Core_Redux.Services.Implementation;

public class MailService : IMailService
{
    private readonly IConfigurationSection _emailConfig;
    private readonly string _fromName;
    private readonly string _fromAddress;
    
    public MailService(IConfiguration configuration)
    {
        _emailConfig = configuration.GetSection("Mail");
        _fromName = _emailConfig.GetValue<string>("FromName", "OIDC.Core");
        _fromAddress = _emailConfig.GetValue<string>("FromAddress", "noreply@oidc.core");
    }

    public async Task SendToUserAsync(EmailViewModel viewModel, User user)
    {
        Tuple<string, string> templates = await BuildTemplateAsync(viewModel.Slug, viewModel.Data!);
        MimeMessage message = BuildMessage(
            new Tuple<string, string>(_fromName, _fromAddress),
            new Tuple<string, string>(user.Username, user.Email),
            templates,
            viewModel.Subject
        );
        
        SendAsync(message);
    }

    public async Task SendToEmailAsync(EmailViewModel viewModel)
    {
        Tuple<string, string> templates = await BuildTemplateAsync(viewModel.Slug, viewModel.Data!);
        MimeMessage message = BuildMessage(
            new Tuple<string, string>(_fromName, _fromAddress),
            new Tuple<string, string>(viewModel.ToName, viewModel.ToAddress),
            templates,
            viewModel.Subject
        );

        SendAsync(message);
    }

    private async Task<Tuple<string, string>> BuildTemplateAsync(string slug, Dictionary<string, string> data)
    {
        string workingDirectory = Environment.CurrentDirectory;
        string templatePath = Path.Join(workingDirectory, "Resources", "Email", slug);

        string htmlPart = await File.ReadAllTextAsync(Path.Join(templatePath, "template-html.cshtml"));
        string textPart = await File.ReadAllTextAsync(Path.Join(templatePath, "template-text.cshtml"));

        IRazorEngine razorEngine = new RazorEngine();

        IRazorEngineCompiledTemplate<RazorEngineTemplateBase<Dictionary<string, string>>> htmlTemplate;
        IRazorEngineCompiledTemplate<RazorEngineTemplateBase<Dictionary<string, string>>> textTemplate;
        
        htmlTemplate = await razorEngine.CompileAsync<RazorEngineTemplateBase<Dictionary<string, string>>>(htmlPart);
        textTemplate = await razorEngine.CompileAsync<RazorEngineTemplateBase<Dictionary<string, string>>>(textPart);

        string htmlResult = await htmlTemplate.RunAsync(instance => instance.Model = data);
        string textResult = await textTemplate.RunAsync(instance => instance.Model = data);

        return new Tuple<string, string>(htmlResult, textResult);
    }

    private MimeMessage BuildMessage(Tuple<string, string> from, Tuple<string, string> to,
        Tuple<string, string> templates, string subject)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(from.Item1, from.Item2));
        message.To.Add(new MailboxAddress(to.Item1, to.Item2));
        message.Subject = subject;

        BodyBuilder bodyBuilder = new BodyBuilder
        {
            HtmlBody = templates.Item1,
            TextBody = templates.Item2
        };
        
        message.Body = bodyBuilder.ToMessageBody();
        
        return message;
    }

    private async void SendAsync(MimeMessage message)
    {
        SmtpClient client = new SmtpClient();
        await client.ConnectAsync(
            _emailConfig.GetValue<string>("SmtpHost", "127.0.0.1"), 
            _emailConfig.GetValue<int>("SmtpPort", 25)
        );
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        client.Dispose();
        message.Dispose();
    }
}