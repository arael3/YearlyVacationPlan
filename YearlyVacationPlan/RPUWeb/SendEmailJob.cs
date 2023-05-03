using RPU.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity.UI.Services;
using RPU.Models;
using RPU.Utility;
using System.Globalization;

namespace RPU.RPUWeb;

public class SendEmailJob : ISendEmailJob
{
    private readonly ILogger<SendEmailJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _config;

    public SendEmailJob(ILogger<SendEmailJob> logger, IUnitOfWork unitOfWork, IEmailSender emailSender, IConfiguration config)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _config = config;
    }

    public async Task SendEmailNotification()
    {
        _logger.LogInformation("Before sending emails");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        List<string> emailsAddresses = new List<string>();

        var now = DateTime.Now;
        List<DateTime> listOfSelectedDays = new();
        List<VacationPlan> vacationPlans = new();


        if (now.AddDays(SD.NumberOfDaysBeforeVacation).Year != now.Year)
        {
            vacationPlans = _unitOfWork.VacationPlan.GetAll(vp => vp.Year == now.Year || vp.Year == now.Year + 1).ToList();
        }
        else
        {
            vacationPlans = _unitOfWork.VacationPlan.GetAll(vp => vp.Year == now.Year).ToList();
        }

        if (vacationPlans is not null)
        {
            foreach (var vacationPlan in vacationPlans)
            {
                if (vacationPlan.VacationDays is not null)
                {
                    List<DateTime> vacationDays = vacationPlan.VacationDays.Split(',')
                        .Select(day => DateTime.ParseExact(day, SD.VacationDayDateFormat, CultureInfo.InvariantCulture))
                        .ToList();

                    for (int i = 0; i < vacationDays.Count; i++)
                    {
                        if (now.AddDays(SD.NumberOfDaysBeforeVacation).Date == vacationDays[i].Date)
                        {
                            if (i == 0)
                            {
                                string emailAdres = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == vacationPlan.ApplicationUserId).Email;
                                emailsAddresses.Add(emailAdres);
                                break;
                            }
                            else
                            {
                                if (now.AddDays(SD.NumberOfDaysBeforeVacation - 1).Date == vacationDays[i - 1].Date)
                                {
                                    break;
                                }
                                else
                                {
                                    string emailAdres = _unitOfWork.IdentityUser.GetFirstOrDefault(u => u.Id == vacationPlan.ApplicationUserId).Email;
                                    emailsAddresses.Add(emailAdres);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (var emailAdres in emailsAddresses)
        {
            // Szczegóły wiadomości mailowej
            string recipient = emailAdres;
            string subject = "Przypomnienie o terminie urlopu";
            var domain = _config["ApplicationInfo:Domain"];
            string body = $"Za {SD.NumberOfDaysBeforeVacation} dni rozpoczyna się Twój planowany urlop. <a href='https://{domain}/Identity/Account/Login'>Zaloguj się na portalu</a>, aby sprawdzić szczegóły.";
            // Wyślij wiadomość mailową
            await _emailSender.SendEmailAsync(recipient, subject, body);
        }

        _logger.LogInformation("After sending emails");
    }
}
