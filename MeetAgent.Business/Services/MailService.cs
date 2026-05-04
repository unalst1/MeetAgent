using MailKit.Net.Smtp;
using MimeKit;
using MeetAgent.DTOs;
using MeetAgent.Business.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MeetAgent.Business.Services;

public class MailService : IMailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendMail(MailDto dto)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _config["MailSettings:FromName"],
            _config["MailSettings:Username"]
        ));

        foreach (var email in dto.ToEmails)
            message.To.Add(MailboxAddress.Parse(email));

        message.Subject = dto.Subject;
        message.Body = new TextPart("html") { Text = dto.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["MailSettings:Host"],
            int.Parse(_config["MailSettings:Port"]!),
            MailKit.Security.SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(
            _config["MailSettings:Username"],
            _config["MailSettings:Password"]
        );
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendMeetingSummary(string toEmail, string meetingTitle, string summary, List<string> decisions, List<string> tasks)
    {
        var decisionsHtml = string.Join("", decisions.Select(d => $"<li>{d}</li>"));
        var tasksHtml = string.Join("", tasks.Select(t => $"<li>{t}</li>"));

        var body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Toplantı Özeti: {meetingTitle}</h2>
            <h3>Özet</h3>
            <p>{summary}</p>
            <h3>Karar Maddeleri</h3>
            <ul>{decisionsHtml}</ul>
            <h3>Görevler</h3>
            <ul>{tasksHtml}</ul>
            <hr/>
            <p style='color: gray; font-size: 12px;'>Meeting Assistant tarafından gönderildi.</p>
        </body>
        </html>";

        await SendMail(new MailDto
        {
            ToEmails = new List<string> { toEmail },
            Subject = $"Toplantı Özeti - {meetingTitle}",
            Body = body
        });
    }
}