using Funifest.Domain.Models;

namespace Funifest.Application.Services.Interfaces;

public interface IParachuteService
{
    Task<IEnumerable<Parachute>> GetAllAsync();
    Task<Parachute?> GetByIdAsync(int id);
    Task<Parachute> CreateAsync(Parachute parachute);
    Task<bool> DeleteAsync(int id);
}

