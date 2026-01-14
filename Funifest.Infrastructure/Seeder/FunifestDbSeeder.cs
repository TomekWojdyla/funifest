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
            new()
            {
                Model = "Navigator",
                CustomName = "Nav 260 - 1",
                Size = 260,
                Type = "Student"
            },
            new()
            {
                Model = "Navigator",
                CustomName = "Nav 260 - 2",
                Size = 260,
                Type = "Student"
            },
            new()
            {
                Model = "Navigator",
                CustomName = "Nav 230 - 3",
                Size = 230,
                Type = "Student"
            },
            new()
            {
                Model = "Pilot",
                CustomName = "Pilot 188 Blue",
                Size = 188,
                Type = "Sport"
            },
            new()
            {
                Model = "Safire 3",
                CustomName = "SF3 169 Red",
                Size = 169,
                Type = "Sport"
            },
            new()
            {
                Model = "Crossfire 3",
                CustomName = "CS3 149 White",
                Size = 149,
                Type = "Sport"
            },
            new()
            {
                Model = "Petra",
                CustomName = "Petra-DB",
                Size = 87,
                Type = "Sport"
            },
            new()
            {
                Model = "Leia",
                CustomName = "Leia 99",
                Size = 99,
                Type = "Sport"
            },
            new()
            {
                Model = "Sigma",
                CustomName = "Sigma 360 - S1",
                Size = 360,
                Type = "Tandem"
            },
            new()
            {
                Model = "Sigma",
                CustomName = "Sigma 360 - S2",
                Size = 360,
                Type = "Tandem"
            },
            new()
            {
                Model = "Sigma",
                CustomName = "Sigma 330 - S3",
                Size = 330,
                Type = "Tandem"
            }
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
            },
            new()
            {
                FirstName = "Ola",
                LastName = "Kowalska",
                Weight = 68,
                LicenseLevel = "D",
                Role = SkydiverRole.Instructor,
                IsAFFInstructor = true,
                IsTandemInstructor = false,
            },
            new()
            {
                FirstName = "Andrzej",
                LastName = "Chudy",
                Weight = 76,
                LicenseLevel = "D",
                Role = SkydiverRole.Instructor,
                IsAFFInstructor = false,
                IsTandemInstructor = true,
            },
            new()
            {
                FirstName = "Michal",
                LastName = "Wisniewski",
                Weight = 90,
                LicenseLevel = "B",
                Role = SkydiverRole.FunJumper,
                ParachuteId = parachutes[5].Id
            },
            new()
            {
                FirstName = "Dominik",
                LastName = "Braun",
                Weight = 84,
                LicenseLevel = "C",
                Role = SkydiverRole.FunJumper,
                ParachuteId = parachutes[6].Id
            },
   
            new()
            {
                FirstName = "Ania",
                LastName = "Zielinska",
                Weight = 62,
                LicenseLevel = "A",
                Role = SkydiverRole.StudentAffEntry
            },
            new()
            {
                FirstName = "Tomasz",
                LastName = "Parys",
                Weight = 77,
                LicenseLevel = "A",
                Role = SkydiverRole.Student
            }
        };

        db.Skydivers.AddRange(skydivers);

        db.Passengers.AddRange(
            new Passenger { FirstName = "Jan", LastName = "Kowal", Weight = 92 },
            new Passenger { FirstName = "Ewa", LastName = "Lis", Weight = 70 },
            new Passenger { FirstName = "Pawel", LastName = "Borkowski", Weight = 102 }
        );

        await db.SaveChangesAsync();
    }
}
