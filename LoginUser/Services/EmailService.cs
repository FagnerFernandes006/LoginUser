using System.Net;
using System.Net.Mail;

namespace LoginUser.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body)
    {
        using var client =
            new SmtpClient(
                _config["Email:Host"],
                int.Parse(_config["Email:Port"]!));

        client.Credentials =
            new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]);

        client.EnableSsl = true;

        var mail =
            new MailMessage(
                _config["Email:Username"]!,
                to,
                subject,
                body);

        await client.SendMailAsync(mail);
    }
}