using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using ToDoApp.Application.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var smtp = new SmtpClient
        {
            Host = _config["Email:SmtpHost"],
            Port = int.Parse(_config["Email:SmtpPort"]!),
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]
            )
        };

        var message = new MailMessage
        {
            From = new MailAddress(_config["Email:From"]!),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await smtp.SendMailAsync(message);
    }
}
