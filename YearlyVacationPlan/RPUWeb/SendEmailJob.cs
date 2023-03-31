using RPU.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace RPU.RPUWeb;

public class SendEmailJob : ISendEmailJob
{
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(ILogger<SendEmailJob> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailNotification()
    {
        _logger.LogInformation("Wysłano maile");
    }
}
