using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RPU.Utility;

// Klasa EmailSender obecnie służy jako zaślepka (fake implementation) - jest dodana, bo jest wymagana przez klasę RegisterModel
// Zaślepka, czyli nic nie robi (nie wysyła żadnych maili)
public class EmailSender : IEmailSender
{
    private readonly IOptionsMonitor<SmtpAccount> _smtp;

    public EmailSender(IOptionsMonitor<SmtpAccount> smtp)
    {
        _smtp = smtp;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailToSend = new MimeKit.MimeMessage();
        emailToSend.From.Add(MailboxAddress.Parse(_smtp.CurrentValue.From));
        emailToSend.To.Add(MailboxAddress.Parse(email));
        emailToSend.Subject = subject;
        emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

        //send email
        using (var emailClient = new SmtpClient())
        {
            emailClient.Connect(_smtp.CurrentValue.Host, _smtp.CurrentValue.Port, MailKit.Security.SecureSocketOptions.StartTls);
            emailClient.Authenticate(_smtp.CurrentValue.Username, _smtp.CurrentValue.Password);
            emailClient.Send(emailToSend);
            emailClient.Disconnect(true);
        }

        return Task.CompletedTask;
    }
}
