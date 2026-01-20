namespace Funifest.Domain.Models;

public class Parachute
{
    public int Id { get; set; }

    // np. Navigator, Safire 3, Crossfire 3
    public string Model { get; set; } = string.Empty;

    // np. "DZ1", "Sabre Blue"
    public string? CustomName { get; set; }

    // np. 260, 169
    public int Size { get; set; }

    // Student / Sport / Tandem itd.
    public string Type { get; set; } = string.Empty;

    public bool ManualBlocked { get; set; }
    public int? ManualBlockedByExitPlanId { get; set; }
    public int? AssignedExitPlanId { get; set; }
}
