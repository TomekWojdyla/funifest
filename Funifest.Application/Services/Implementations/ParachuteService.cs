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
            .Select(parachute => new ParachuteDto
            {
                Id = parachute.Id,
                Model = parachute.Model,
                CustomName = parachute.CustomName,
                Size = parachute.Size,
                Type = parachute.Type
            })
            .ToListAsync();
    }

    public async Task<ParachuteDto?> GetByIdAsync(int id)
    {
        var parachute = await _context.Parachutes
            .FirstOrDefaultAsync(p => p.Id == id);

        if (parachute == null)
            return null;

        return new ParachuteDto
        {
            Id = parachute.Id,
            Model = parachute.Model,
            CustomName = parachute.CustomName,
            Size = parachute.Size,
            Type = parachute.Type
        };
    }

    public async Task<ParachuteDto> CreateAsync(CreateParachuteDto dto)
    {
        var parachute = new Parachute
        {
            Model = dto.Model,
            CustomName = dto.CustomName,
            Size = dto.Size,
            Type = dto.Type
        };

        _context.Parachutes.Add(parachute);
        await _context.SaveChangesAsync();

        return new ParachuteDto
        {
            Id = parachute.Id,
            Model = parachute.Model,
            CustomName = parachute.CustomName,
            Size = parachute.Size,
            Type = parachute.Type
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var parachute = await _context.Parachutes
            .FirstOrDefaultAsync(p => p.Id == id);

        if (parachute == null)
            return false;

        _context.Parachutes.Remove(parachute);
        await _context.SaveChangesAsync();

        return true;
    }
}
