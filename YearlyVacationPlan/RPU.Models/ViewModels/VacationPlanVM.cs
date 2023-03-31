using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RPU.Models.ViewModels;

public class VacationPlanVM
{
    public VacationPlan? VacationPlan { get; set; }

    [ValidateNever]
    public string? HolidaySchedule { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem>? AvailableYears { get; set; }

    [ValidateNever]
    [Display(Name = "Nazwa użytkownika")]
    public string? Username { get; set; }

    [ValidateNever]
    public List<SelectListItem>? ApplicationUsersList { get; set; }

    [ValidateNever]
    public string? Deputy1VacationDays { get; set; }

    [ValidateNever]
    public string? Deputy2VacationDays { get; set; }

    //public string? InvitedUserNameOrEmail { get; set; }

    [ValidateNever]
    public List<SharedVacationPlanVM>? AcceptedSharedVacationPlansVM { get; set; }

    [ValidateNever]
    public List<SharedVacationPlanVM>? ReceivedSharedVacationPlansVM { get; set; }

    [ValidateNever]
    public List<SharedVacationPlanVM>? SentSharedVacationPlansVM { get; set; }
}
