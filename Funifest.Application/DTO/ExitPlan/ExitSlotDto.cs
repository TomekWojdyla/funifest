namespace Funifest.Application.DTO.ExitPlan;

public class ExitSlotDto
{
    public int SlotNumber { get; set; }
    public int PersonId { get; set; }
    public string PersonType { get; set; } = ""; // "Skydiver" lub "Passenger"
    public int ParachuteId { get; set; }
}

