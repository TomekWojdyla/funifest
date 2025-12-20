using Funifest.Application.DTO.ExitPlan;
using Funifest.Application.Services.Interfaces;
using Funifest.Domain.Models;
using Funifest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Funifest.Application.Services.Implementations;

public class ExitPlanService : IExitPlanService
{
    private readonly FunifestContext _context;

    public ExitPlanService(FunifestContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ExitPlan>> GetAllAsync()
    {
        return await _context.ExitPlans
            .Include(p => p.Slots)
            .ToListAsync();
    }

    public async Task<ExitPlan?> GetByIdAsync(int id)
    {
        return await _context.ExitPlans
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ExitPlan> CreateAsync(CreateExitPlanRequest request)
    {
        if (request.Slots == null || request.Slots.Count == 0)
            throw new ArgumentException("Plan must contain at least one slot.", nameof(request));

        // 1. Unikalność numerów slotów
        if (request.Slots.Select(s => s.SlotNumber).Distinct().Count() != request.Slots.Count)
            throw new InvalidOperationException("Duplicate slot numbers detected.");

        // 2. Zbieramy ID do walidacji
        var skydiverIds = request.Slots
            .Where(s => s.PersonType == "Skydiver")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var passengerIds = request.Slots
            .Where(s => s.PersonType == "Passenger")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var parachuteIds = request.Slots
            .Select(s => s.ParachuteId)
            .Distinct()
            .ToList();

        // 3. Pobieramy istniejące encje z bazy
        var skydivers = await _context.Skydivers
            .Where(s => skydiverIds.Contains(s.Id))
            .ToListAsync();

        var passengers = await _context.Passengers
            .Where(p => passengerIds.Contains(p.Id))
            .ToListAsync();

        var parachutes = await _context.Parachutes
            .Where(p => parachuteIds.Contains(p.Id))
            .ToListAsync();

        // 4. Walidacja istnienia
        if (skydivers.Count != skydiverIds.Count)
            throw new InvalidOperationException("Some Skydivers used in slots do not exist.");

        if (passengers.Count != passengerIds.Count)
            throw new InvalidOperationException("Some Passengers used in slots do not exist.");

        if (parachutes.Count != parachuteIds.Count)
            throw new InvalidOperationException("Some Parachutes used in slots do not exist.");

        // 5. Tworzymy ExitPlan
        var plan = new ExitPlan
        {
            Date = request.Date,
            Aircraft = request.Aircraft,
            Slots = new List<ExitSlot>()
        };

        // 6. Tworzymy sloty – tylko ID + typ, bez Person
        foreach (var slotDto in request.Slots.OrderBy(s => s.SlotNumber))
        {
            if (slotDto.PersonType != "Skydiver" && slotDto.PersonType != "Passenger")
                throw new InvalidOperationException($"Invalid PersonType '{slotDto.PersonType}' in slot {slotDto.SlotNumber}.");

            var slot = new ExitSlot
            {
                SlotNumber = slotDto.SlotNumber,
                PersonId = slotDto.PersonId,
                PersonType = slotDto.PersonType,
                ParachuteId = slotDto.ParachuteId
                // ExitPlanId ustawi EF po zapisaniu planu
            };

            plan.Slots.Add(slot);
        }

        // 7. Zapis transakcyjny
        using var transaction = await _context.Database.BeginTransactionAsync();

        _context.ExitPlans.Add(plan);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return plan;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var plan = await _context.ExitPlans.FindAsync(id);
        if (plan == null)
            return false;

        _context.ExitPlans.Remove(plan);
        await _context.SaveChangesAsync();
        return true;
    }
}
