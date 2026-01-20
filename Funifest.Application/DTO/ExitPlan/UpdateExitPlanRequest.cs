namespace Funifest.Application.DTO.ExitPlan;

public class UpdateExitPlanRequest
{
    public DateTime Date { get; set; }
    public string Aircraft { get; set; } = "";
    public List<ExitSlotDto> Slots { get; set; } = new();
}
