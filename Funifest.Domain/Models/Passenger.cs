namespace Funifest.Domain.Models;

public class Passenger : Person
{
    public float? Weight { get; set; }

    public bool ManualBlocked { get; set; }
    public int? ManualBlockedByExitPlanId { get; set; }
    public int? AssignedExitPlanId { get; set; }
}
