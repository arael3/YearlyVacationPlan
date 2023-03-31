using System.ComponentModel.DataAnnotations;

namespace RPU.Models;

public class YearlyHolidaySchedule
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Rok")]
    public int Year { get; set; }

    public string? HolidaySchedule { get; set; }
}
