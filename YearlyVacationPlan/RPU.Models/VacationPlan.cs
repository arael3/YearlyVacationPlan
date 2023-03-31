using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPU.Models;

public class VacationPlan
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Rok")]
    [Range(2023, 2222, ErrorMessage = "Podaj rok")]
    public int Year { get; set; }

    [Display(Name = "Wymiar urlopu")]
    public int MaxVacationDaysNumber { get; set; }

    [Display(Name = "Liczba zaznaczonych dni roboczych")]
    public string? VacationDays { get; set; }

    [Required]
    [Display(Name = "Nazwa użytkownika")]
    public string ApplicationUserId { get; set; }

    [Display(Name = "Znajomy 1")]
    public string? Deputy1ApplicationUserId { get; set; }

    [Display(Name = "Znajomy 2")]
    public string? Deputy2ApplicationUserId { get; set; }


    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public IdentityUser ApplicationUser { get; set; }

    [ForeignKey("Deputy1ApplicationUserId")]
    [ValidateNever]
    public IdentityUser? Deputy1ApplicationUser { get; set; }

    [ForeignKey("Deputy2ApplicationUserId")]
    [ValidateNever]
    public IdentityUser? Deputy2ApplicationUser { get; set; }
}
