using System.Text.Json.Serialization;

namespace Funifest.Domain.Models;

public class ExitSlot
{
    public int Id { get; set; }

    public int SlotNumber { get; set; }

    public int ExitPlanId { get; set; }

    [JsonIgnore]                 // 👈 DODAJ TO
    public ExitPlan? ExitPlan { get; set; }

    public int PersonId { get; set; }
    public string PersonType { get; set; } = "";

    public int ParachuteId { get; set; }
    public Parachute? Parachute { get; set; }
}
