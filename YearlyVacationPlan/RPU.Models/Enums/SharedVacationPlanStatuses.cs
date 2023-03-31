using System.ComponentModel.DataAnnotations;

namespace RPU.Models.Enums;

public enum SharedVacationPlanStatuses
{
    [Display(Name = "Oczekuje")]
    Pending,

    [Display(Name = "Zaakceptowano")]
    Accepted
}
