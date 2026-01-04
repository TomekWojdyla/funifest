using Funifest.Domain.Models;
using Funifest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Funifest.Infrastructure.Seeder;

public static class FunifestDbSeeder
{
    public static async Task SeedAsync(FunifestContext db)
    {
        if (await db.Skydivers.AnyAsync())
            return;

        var parachutes = new List<Parachute>
        {
            new() { Model = "Navigator", Size = 260, Type = "Student" },
            new() { Model = "Navigator", Size = 240, Type = "Student" },
            new() { Model = "Pilot", Size = 188, Type = "Sport" },
            new() { Model = "Safire 3", Size = 169, Type = "Sport" },
            new() { Model = "Crossfire 3", Size = 149, Type = "Sport" }
        };

        db.Parachutes.AddRange(parachutes);
        await db.SaveChangesAsync();

        var skydivers = new List<Skydiver>
        {
            new()
            {
                FirstName = "Kuba",
                LastName = "Nowak",
                Weight = 82,
                LicenseLevel = "D",
                Role = SkydiverRole.Instructor,
                IsAFFInstructor = true,
                IsTandemInstructor = true,
                ParachuteId = parachutes[2].Id
            },
            new()
            {
                FirstName = "Ola",
                LastName = "Kowalska",
                Weight = 68,
                LicenseLevel = "C",
                Role = SkydiverRole.Instructor,
                IsAFFInstructor = true,
                IsTandemInstructor = false,
                ParachuteId = parachutes[3].Id
            },
            new()
            {
                FirstName = "Michał",
                LastName = "Wiśniewski",
                Weight = 90,
                LicenseLevel = "B",
                Role = SkydiverRole.FunJumper,
                ParachuteId = parachutes[4].Id
            },
            new()
            {
                FirstName = "Ania",
                LastName = "Zielińska",
                Weight = 62,
                LicenseLevel = "A",
                Role = SkydiverRole.StudentAffEntry
            }
        };

        db.Skydivers.AddRange(skydivers);

        db.Passengers.AddRange(
            new Passenger { FirstName = "Jan", LastName = "Kowal", Weight = 92 },
            new Passenger { FirstName = "Ewa", LastName = "Lis", Weight = 70 }
        );

        await db.SaveChangesAsync();
    }
}
