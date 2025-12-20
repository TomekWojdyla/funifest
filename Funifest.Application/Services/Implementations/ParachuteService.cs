using Funifest.Application.Services.Interfaces;
using Funifest.Domain.Models;
using Funifest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Funifest.Application.Services.Implementations;

public class ParachuteService : IParachuteService
{
    private readonly FunifestContext _context;

    public ParachuteService(FunifestContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Parachute>> GetAllAsync()
    {
        return await _context.Parachutes.ToListAsync();
    }

    public async Task<Parachute?> GetByIdAsync(int id)
    {
        return await _context.Parachutes.FindAsync(id);
    }

    public async Task<Parachute> CreateAsync(Parachute parachute)
    {
        _context.Parachutes.Add(parachute);
        await _context.SaveChangesAsync();
        return parachute;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _context.Parachutes.FindAsync(id);
        if (p == null) return false;

        _context.Parachutes.Remove(p);
        await _context.SaveChangesAsync();
        return true;
    }
}

