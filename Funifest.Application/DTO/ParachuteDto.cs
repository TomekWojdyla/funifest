namespace Funifest.Application.DTO;

public class ParachuteDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string? CustomName { get; set; }
    public int Size { get; set; }
    public string Type { get; set; } = string.Empty;

    public bool ManualBlocked { get; set; }
    public int? ManualBlockedByExitPlanId { get; set; }
    public int? AssignedExitPlanId { get; set; }
}
