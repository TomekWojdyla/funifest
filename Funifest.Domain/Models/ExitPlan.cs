namespace Funifest.Domain.Models;

public class ExitPlan
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Aircraft { get; set; } = "";

    public ExitPlanStatus Status { get; set; } = ExitPlanStatus.Draft;
    public DateTime? DispatchedAt { get; set; }

    public List<ExitSlot> Slots { get; set; } = new();
}
