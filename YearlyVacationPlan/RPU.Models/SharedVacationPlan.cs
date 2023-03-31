using RPU.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPU.Models;

public class SharedVacationPlan
{
    [Key]
    public int Id { get; set; }

    public string? SenderApplicationUserId { get; set; }

    public string? ReceiverApplicationUserId { get; set; }

    [Required]
    public int SharedVacationPlanYear { get; set; }

    [Required]
    public SharedVacationPlanStatuses Status { get; set; }

    [ForeignKey("SenderApplicationUserId")]
    public IdentityUser SenderApplicationUser { get; set; }

    [ForeignKey("ReceiverApplicationUserId")]
    public IdentityUser? ReceiverApplicationUser { get; set; }

}
