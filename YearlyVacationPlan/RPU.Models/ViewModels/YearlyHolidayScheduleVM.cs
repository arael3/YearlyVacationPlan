using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPU.Models.ViewModels;

public class YearlyHolidayScheduleVM
{
    public YearlyHolidaySchedule? YearlyHolidaySchedule { get; set; }

    [ValidateNever]
    public List<DateTime>? YearlyHolidayScheduleDaysList { get; set; }
}
