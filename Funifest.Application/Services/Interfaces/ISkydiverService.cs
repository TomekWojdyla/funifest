using Funifest.Domain.Models;

namespace Funifest.Application.Services.Interfaces;

public interface ISkydiverService
{
    Task<IEnumerable<Skydiver>> GetAllAsync();
    Task<Skydiver?> GetByIdAsync(int id);
    Task<Skydiver> CreateAsync(Skydiver skydiver);
    Task<Skydiver?> BlockAsync(int id);
    Task<Skydiver?> UnblockAsync(int id);
    Task<bool> DeleteAsync(int id);
}
