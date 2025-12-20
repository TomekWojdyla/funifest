namespace Funifest.Application.DTO.ExitPlan;

public class CreateExitPlanRequest
{
    public DateTime Date { get; set; }
    public string Aircraft { get; set; } = "";
    public List<ExitSlotDto> Slots { get; set; } = new();
}

