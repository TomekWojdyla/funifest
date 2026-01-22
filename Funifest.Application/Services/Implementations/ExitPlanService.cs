using Funifest.Application.DTO.ExitPlan;
using Funifest.Application.Exceptions;
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
        EnsureSlots(request.Slots);

        var slotDtos = request.Slots.OrderBy(s => s.SlotNumber).ToList();
        EnsurePersonTypes(slotDtos);

        var skydiverIds = slotDtos
            .Where(s => s.PersonType == "Skydiver")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var passengerIds = slotDtos
            .Where(s => s.PersonType == "Passenger")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var parachuteIds = slotDtos
            .Select(s => s.ParachuteId)
            .Distinct()
            .ToList();

        var skydivers = await _context.Skydivers
            .Where(s => skydiverIds.Contains(s.Id))
            .ToListAsync();

        var passengers = await _context.Passengers
            .Where(p => passengerIds.Contains(p.Id))
            .ToListAsync();

        var parachutes = await _context.Parachutes
            .Where(p => parachuteIds.Contains(p.Id))
            .ToListAsync();

        EnsureExistence(skydiverIds.Count, skydivers.Count, "Skydivers");
        EnsureExistence(passengerIds.Count, passengers.Count, "Passengers");
        EnsureExistence(parachuteIds.Count, parachutes.Count, "Parachutes");

        EnsureNotBlockedAndNotAssignedForCreate(skydivers, passengers, parachutes);

        var plan = new ExitPlan
        {
            Date = request.Date,
            Aircraft = request.Aircraft,
            Status = ExitPlanStatus.Draft,
            DispatchedAt = null,
            Slots = new List<ExitSlot>()
        };

        foreach (var slotDto in slotDtos)
        {
            plan.Slots.Add(new ExitSlot
            {
                SlotNumber = slotDto.SlotNumber,
                PersonId = slotDto.PersonId,
                PersonType = slotDto.PersonType,
                ParachuteId = slotDto.ParachuteId
            });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        _context.ExitPlans.Add(plan);
        await _context.SaveChangesAsync();

        var newPlanId = plan.Id;

        foreach (var s in skydivers)
            s.AssignedExitPlanId = newPlanId;

        foreach (var p in passengers)
            p.AssignedExitPlanId = newPlanId;

        foreach (var pr in parachutes)
            pr.AssignedExitPlanId = newPlanId;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return plan;
    }

    public async Task<ExitPlan?> UpdateAsync(int id, UpdateExitPlanRequest request)
    {
        var plan = await _context.ExitPlans
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan is null)
            return null;

        if (plan.Status != ExitPlanStatus.Draft)
            throw new ConflictException("Exit plan is dispatched and cannot be edited.");

        EnsureSlots(request.Slots);

        var slotDtos = request.Slots.OrderBy(s => s.SlotNumber).ToList();
        EnsurePersonTypes(slotDtos);

        var newSkydiverIds = slotDtos
            .Where(s => s.PersonType == "Skydiver")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var newPassengerIds = slotDtos
            .Where(s => s.PersonType == "Passenger")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var newParachuteIds = slotDtos
            .Select(s => s.ParachuteId)
            .Distinct()
            .ToList();

        var skydivers = await _context.Skydivers
            .Where(s => newSkydiverIds.Contains(s.Id))
            .ToListAsync();

        var passengers = await _context.Passengers
            .Where(p => newPassengerIds.Contains(p.Id))
            .ToListAsync();

        var parachutes = await _context.Parachutes
            .Where(p => newParachuteIds.Contains(p.Id))
            .ToListAsync();

        EnsureExistence(newSkydiverIds.Count, skydivers.Count, "Skydivers");
        EnsureExistence(newPassengerIds.Count, passengers.Count, "Passengers");
        EnsureExistence(newParachuteIds.Count, parachutes.Count, "Parachutes");

        EnsureNotBlockedAndNotAssignedForUpdate(id, skydivers, passengers, parachutes);

        var oldSkydiverIds = plan.Slots
            .Where(s => s.PersonType == "Skydiver")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var oldPassengerIds = plan.Slots
            .Where(s => s.PersonType == "Passenger")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var oldParachuteIds = plan.Slots
            .Select(s => s.ParachuteId)
            .Distinct()
            .ToList();

        var newSlots = slotDtos.Select(slotDto => new ExitSlot
        {
            SlotNumber = slotDto.SlotNumber,
            PersonId = slotDto.PersonId,
            PersonType = slotDto.PersonType,
            ParachuteId = slotDto.ParachuteId,
            ExitPlanId = id
        }).ToList();

        using var transaction = await _context.Database.BeginTransactionAsync();

        plan.Date = request.Date;
        plan.Aircraft = request.Aircraft;

        _context.ExitSlots.RemoveRange(plan.Slots);
        plan.Slots = newSlots;

        await ReleaseAssignedLocks(
            id,
            oldSkydiverIds,
            oldPassengerIds,
            oldParachuteIds,
            newSkydiverIds,
            newPassengerIds,
            newParachuteIds);

        await AssignLocksToPlan(id, skydivers, passengers, parachutes);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetByIdAsync(id);
    }

    public async Task<ExitPlan?> DispatchAsync(int id)
    {
        var plan = await _context.ExitPlans
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan is null)
            return null;

        if (plan.Status != ExitPlanStatus.Draft)
            throw new ConflictException("Exit plan is already dispatched.");

        var skydiverIds = plan.Slots
            .Where(s => s.PersonType == "Skydiver")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var passengerIds = plan.Slots
            .Where(s => s.PersonType == "Passenger")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var parachuteIds = plan.Slots
            .Select(s => s.ParachuteId)
            .Distinct()
            .ToList();

        var skydivers = await _context.Skydivers
            .Where(s => skydiverIds.Contains(s.Id))
            .ToListAsync();

        var passengers = await _context.Passengers
            .Where(p => passengerIds.Contains(p.Id))
            .ToListAsync();

        var parachutes = await _context.Parachutes
            .Where(p => parachuteIds.Contains(p.Id))
            .ToListAsync();

        EnsureExistence(skydiverIds.Count, skydivers.Count, "Skydivers");
        EnsureExistence(passengerIds.Count, passengers.Count, "Passengers");
        EnsureExistence(parachuteIds.Count, parachutes.Count, "Parachutes");

        using var transaction = await _context.Database.BeginTransactionAsync();

        plan.Status = ExitPlanStatus.Dispatched;
        plan.DispatchedAt = DateTime.UtcNow;

        foreach (var s in skydivers)
        {
            if (s.AssignedExitPlanId == id)
                s.AssignedExitPlanId = null;
        }

        foreach (var p in passengers)
        {
            if (p.AssignedExitPlanId == id)
                p.AssignedExitPlanId = null;
        }

        foreach (var pr in parachutes)
        {
            if (pr.AssignedExitPlanId == id)
                pr.AssignedExitPlanId = null;
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> UndoDispatchAsync(int id)
    {
        var plan = await _context.ExitPlans
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan is null)
            return false;

        if (plan.Status != ExitPlanStatus.Dispatched)
            throw new ConflictException("Exit plan is not dispatched.");

        var skydiverIds = plan.Slots
            .Where(s => s.PersonType == "Skydiver")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var passengerIds = plan.Slots
            .Where(s => s.PersonType == "Passenger")
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var parachuteIds = plan.Slots
            .Select(s => s.ParachuteId)
            .Distinct()
            .ToList();

        using var transaction = await _context.Database.BeginTransactionAsync();

        if (skydiverIds.Count > 0)
        {
            var list = await _context.Skydivers.Where(s => skydiverIds.Contains(s.Id)).ToListAsync();
            foreach (var s in list)
            {
                if (s.ManualBlockedByExitPlanId == id)
                {
                    s.ManualBlocked = false;
                    s.ManualBlockedByExitPlanId = null;
                }
            }
        }

        if (passengerIds.Count > 0)
        {
            var list = await _context.Passengers.Where(p => passengerIds.Contains(p.Id)).ToListAsync();
            foreach (var p in list)
            {
                if (p.ManualBlockedByExitPlanId == id)
                {
                    p.ManualBlocked = false;
                    p.ManualBlockedByExitPlanId = null;
                }
            }
        }

        if (parachuteIds.Count > 0)
        {
            var list = await _context.Parachutes.Where(p => parachuteIds.Contains(p.Id)).ToListAsync();
            foreach (var pr in list)
            {
                if (pr.ManualBlockedByExitPlanId == id)
                {
                    pr.ManualBlocked = false;
                    pr.ManualBlockedByExitPlanId = null;
                }
            }
        }

        _context.ExitPlans.Remove(plan);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var plan = await _context.ExitPlans
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan is null)
            return false;

        using var transaction = await _context.Database.BeginTransactionAsync();

        if (plan.Status == ExitPlanStatus.Draft)
        {
            var skydiverIds = plan.Slots
                .Where(s => s.PersonType == "Skydiver")
                .Select(s => s.PersonId)
                .Distinct()
                .ToList();

            var passengerIds = plan.Slots
                .Where(s => s.PersonType == "Passenger")
                .Select(s => s.PersonId)
                .Distinct()
                .ToList();

            var parachuteIds = plan.Slots
                .Select(s => s.ParachuteId)
                .Distinct()
                .ToList();

            await ClearAssignedLocksForPlan(id, skydiverIds, passengerIds, parachuteIds);
        }

        _context.ExitPlans.Remove(plan);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }

    private static void EnsureSlots(List<ExitSlotDto>? slots)
    {
        if (slots == null || slots.Count == 0)
            throw new ArgumentException("Plan must contain at least one slot.");

        if (slots.Select(s => s.SlotNumber).Distinct().Count() != slots.Count)
            throw new InvalidOperationException("Duplicate slot numbers detected.");
    }

    private static void EnsurePersonTypes(List<ExitSlotDto> slots)
    {
        foreach (var s in slots)
        {
            if (s.PersonType != "Skydiver" && s.PersonType != "Passenger")
                throw new InvalidOperationException($"Invalid PersonType '{s.PersonType}' in slot {s.SlotNumber}.");
        }
    }

    private static void EnsureExistence(int expected, int actual, string entityName)
    {
        if (expected != actual)
            throw new InvalidOperationException($"Some {entityName} used in slots do not exist.");
    }

    private static void EnsureNotBlockedAndNotAssignedForCreate(
        List<Skydiver> skydivers,
        List<Passenger> passengers,
        List<Parachute> parachutes)
    {
        foreach (var s in skydivers)
        {
            if (s.ManualBlocked)
                throw new ConflictException($"Skydiver {s.Id} is manually blocked.");
            if (s.AssignedExitPlanId != null)
                throw new ConflictException($"Skydiver {s.Id} is already assigned to exit plan {s.AssignedExitPlanId}.");
        }

        foreach (var p in passengers)
        {
            if (p.ManualBlocked)
                throw new ConflictException($"Passenger {p.Id} is manually blocked.");
            if (p.AssignedExitPlanId != null)
                throw new ConflictException($"Passenger {p.Id} is already assigned to exit plan {p.AssignedExitPlanId}.");
        }

        foreach (var pr in parachutes)
        {
            if (pr.ManualBlocked)
                throw new ConflictException($"Parachute {pr.Id} is manually blocked.");
            if (pr.AssignedExitPlanId != null)
                throw new ConflictException($"Parachute {pr.Id} is already assigned to exit plan {pr.AssignedExitPlanId}.");
        }
    }

    private static void EnsureNotBlockedAndNotAssignedForUpdate(
        int exitPlanId,
        List<Skydiver> skydivers,
        List<Passenger> passengers,
        List<Parachute> parachutes)
    {
        foreach (var s in skydivers)
        {
            if (s.ManualBlocked)
                throw new ConflictException($"Skydiver {s.Id} is manually blocked.");
            if (s.AssignedExitPlanId != null && s.AssignedExitPlanId != exitPlanId)
                throw new ConflictException($"Skydiver {s.Id} is already assigned to exit plan {s.AssignedExitPlanId}.");
        }

        foreach (var p in passengers)
        {
            if (p.ManualBlocked)
                throw new ConflictException($"Passenger {p.Id} is manually blocked.");
            if (p.AssignedExitPlanId != null && p.AssignedExitPlanId != exitPlanId)
                throw new ConflictException($"Passenger {p.Id} is already assigned to exit plan {p.AssignedExitPlanId}.");
        }

        foreach (var pr in parachutes)
        {
            if (pr.ManualBlocked)
                throw new ConflictException($"Parachute {pr.Id} is manually blocked.");
            if (pr.AssignedExitPlanId != null && pr.AssignedExitPlanId != exitPlanId)
                throw new ConflictException($"Parachute {pr.Id} is already assigned to exit plan {pr.AssignedExitPlanId}.");
        }
    }

    private async Task ReleaseAssignedLocks(
        int exitPlanId,
        List<int> oldSkydiverIds,
        List<int> oldPassengerIds,
        List<int> oldParachuteIds,
        List<int> newSkydiverIds,
        List<int> newPassengerIds,
        List<int> newParachuteIds)
    {
        var skydiversToRelease = oldSkydiverIds.Except(newSkydiverIds).ToList();
        var passengersToRelease = oldPassengerIds.Except(newPassengerIds).ToList();
        var parachutesToRelease = oldParachuteIds.Except(newParachuteIds).ToList();

        if (skydiversToRelease.Count > 0)
        {
            var list = await _context.Skydivers.Where(s => skydiversToRelease.Contains(s.Id)).ToListAsync();
            foreach (var s in list)
                if (s.AssignedExitPlanId == exitPlanId) s.AssignedExitPlanId = null;
        }

        if (passengersToRelease.Count > 0)
        {
            var list = await _context.Passengers.Where(p => passengersToRelease.Contains(p.Id)).ToListAsync();
            foreach (var p in list)
                if (p.AssignedExitPlanId == exitPlanId) p.AssignedExitPlanId = null;
        }

        if (parachutesToRelease.Count > 0)
        {
            var list = await _context.Parachutes.Where(p => parachutesToRelease.Contains(p.Id)).ToListAsync();
            foreach (var pr in list)
                if (pr.AssignedExitPlanId == exitPlanId) pr.AssignedExitPlanId = null;
        }
    }

    private Task AssignLocksToPlan(
        int exitPlanId,
        List<Skydiver> skydivers,
        List<Passenger> passengers,
        List<Parachute> parachutes)
    {
        foreach (var s in skydivers)
            s.AssignedExitPlanId = exitPlanId;

        foreach (var p in passengers)
            p.AssignedExitPlanId = exitPlanId;

        foreach (var pr in parachutes)
            pr.AssignedExitPlanId = exitPlanId;

        return Task.CompletedTask;
    }

    private async Task ClearAssignedLocksForPlan(
        int exitPlanId,
        List<int> skydiverIds,
        List<int> passengerIds,
        List<int> parachuteIds)
    {
        if (skydiverIds.Count > 0)
        {
            var list = await _context.Skydivers.Where(s => skydiverIds.Contains(s.Id)).ToListAsync();
            foreach (var s in list)
                if (s.AssignedExitPlanId == exitPlanId) s.AssignedExitPlanId = null;
        }

        if (passengerIds.Count > 0)
        {
            var list = await _context.Passengers.Where(p => passengerIds.Contains(p.Id)).ToListAsync();
            foreach (var p in list)
                if (p.AssignedExitPlanId == exitPlanId) p.AssignedExitPlanId = null;
        }

        if (parachuteIds.Count > 0)
        {
            var list = await _context.Parachutes.Where(p => parachuteIds.Contains(p.Id)).ToListAsync();
            foreach (var pr in list)
                if (pr.AssignedExitPlanId == exitPlanId) pr.AssignedExitPlanId = null;
        }
    }
}
