namespace Funifest.Domain.Models;

public class Parachute
{
    public int Id { get; set; }
    public string Name { get; set; } = "";        // nazwa / model
    public int Size { get; set; }                 // w stopach np. 170, 210
    public string Type { get; set; } = "";        // np. sport, student, tandem
}
