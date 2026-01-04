using Funifest.Application.DTO;
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

    public async Task<IEnumerable<ParachuteDto>> GetAllAsync()
    {
        return await _context.Parachutes
            .AsNoTracking()
            .Select(p => new ParachuteDto
            {
                Id = p.Id,
                Model = p.Model,
                Size = p.Size,
                Type = p.Type
            })
            .ToListAsync();
    }

    public async Task<ParachuteDto?> GetByIdAsync(int id)
    {
        return await _context.Parachutes
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ParachuteDto
            {
                Id = p.Id,
                Model = p.Model,
                Size = p.Size,
                Type = p.Type
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ParachuteDto> CreateAsync(CreateParachuteDto dto)
    {
        var parachute = new Parachute
        {
            Model = dto.Model,
            Size = dto.Size,
            Type = dto.Type
        };

        _context.Parachutes.Add(parachute);
        await _context.SaveChangesAsync();

        return new ParachuteDto
        {
            Id = parachute.Id,
            Model = parachute.Model,
            Size = parachute.Size,
            Type = parachute.Type
        };
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


