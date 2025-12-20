namespace Funifest.Domain.Models;
    public class Skydiver : Person
    {
        public float? Weight { get; set; }
        public string LicenseLevel { get; set; } = ""; // A/B/C/D
    }
