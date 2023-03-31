namespace RPU.Models.ViewModels;

public class SharedVacationPlanVM
{
    public SharedVacationPlan? SharedVacationPlan { get; set; }
    public string? SenderApplicationUsername { get; set; }
    public string? ReceiverApplicationUsername { get; set; }
    public string? AcceptedShareApplicationUsername { get; set; }
}
