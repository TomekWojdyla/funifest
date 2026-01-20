namespace Funifest.Domain.Models;
public class Skydiver : Person
{
    public float? Weight { get; set; }
    public string LicenseLevel { get; set; } = ""; // A/B/C/D

    // Rola do logiki we froncie
    public SkydiverRole Role { get; set; } = SkydiverRole.FunJumper;

    // Uprawnienia (mogą być niezależne od roli)
    public bool IsAFFInstructor { get; set; }
    public bool IsTandemInstructor { get; set; }

    // Domyślny spadochron (Student może nie mieć)
    public int? ParachuteId { get; set; }

    public bool ManualBlocked { get; set; }
    public int? ManualBlockedByExitPlanId { get; set; }
    public int? AssignedExitPlanId { get; set; }
}
