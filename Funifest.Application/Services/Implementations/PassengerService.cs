using Funifest.Application.Services.Interfaces;
using Funifest.Domain.Models;
using Funifest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Funifest.Application.Services.Implementations;

public class PassengerService : IPassengerService
{
    private readonly FunifestContext _context;

    public PassengerService(FunifestContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Passenger>> GetAllAsync()
    {
        return await _context.Passengers.ToListAsync();
    }

    public async Task<Passenger?> GetByIdAsync(int id)
    {
        return await _context.Passengers.FindAsync(id);
    }

    public async Task<Passenger> CreateAsync(Passenger passenger)
    {
        _context.Passengers.Add(passenger);
        await _context.SaveChangesAsync();
        return passenger;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _context.Passengers.FindAsync(id);
        if (p == null) return false;

        _context.Passengers.Remove(p);
        await _context.SaveChangesAsync();
        return true;
    }
}

