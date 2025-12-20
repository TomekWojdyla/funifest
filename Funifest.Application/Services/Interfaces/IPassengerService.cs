using Funifest.Domain.Models;

namespace Funifest.Application.Services.Interfaces;

public interface IPassengerService
{
    Task<IEnumerable<Passenger>> GetAllAsync();
    Task<Passenger?> GetByIdAsync(int id);
    Task<Passenger> CreateAsync(Passenger passenger);
    Task<bool> DeleteAsync(int id);
}

