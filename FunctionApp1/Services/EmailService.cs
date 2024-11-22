
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace FunctionApp1.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;

    public EmailService(IConfiguration configuration)
    {
        _smtpServer = configuration["SmtpServer"];
        _smtpPort = int.Parse(configuration["SmtpPort"]);
        _senderEmail = configuration["SenderEmail"];
        _senderPassword = configuration["SenderPassword"];
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage(_senderEmail, toEmail)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        var smtpClient = new SmtpClient(_smtpServer)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_senderEmail, _senderPassword),
            EnableSsl = true
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error sending email", ex);
        }
    }
}
