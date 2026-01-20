using Funifest.Application.Services.Interfaces;
using Funifest.Domain.Models;
using Funifest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Funifest.Application.Services.Implementations;

public class SkydiverService : ISkydiverService
{
    private readonly FunifestContext _db;

    public SkydiverService(FunifestContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Skydiver>> GetAllAsync()
    {
        return await _db.Skydivers.ToListAsync();
    }

    public async Task<Skydiver?> GetByIdAsync(int id)
    {
        return await _db.Skydivers.FindAsync(id);
    }

    public async Task<Skydiver> CreateAsync(Skydiver skydiver)
    {
        _db.Skydivers.Add(skydiver);
        await _db.SaveChangesAsync();
        return skydiver;
    }

    public async Task<Skydiver?> BlockAsync(int id)
    {
        var s = await _db.Skydivers.FindAsync(id);
        if (s == null)
            return null;

        s.ManualBlocked = true;
        s.ManualBlockedByExitPlanId = null;

        await _db.SaveChangesAsync();
        return s;
    }

    public async Task<Skydiver?> UnblockAsync(int id)
    {
        var s = await _db.Skydivers.FindAsync(id);
        if (s == null)
            return null;

        s.ManualBlocked = false;
        s.ManualBlockedByExitPlanId = null;

        await _db.SaveChangesAsync();
        return s;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var s = await _db.Skydivers.FindAsync(id);
        if (s == null)
            return false;

        _db.Skydivers.Remove(s);
        await _db.SaveChangesAsync();
        return true;
    }
}
