namespace Funifest.Domain.Models;

public class ExitPlan
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Aircraft { get; set; } = "";

    public List<ExitSlot> Slots { get; set; } = new();
}