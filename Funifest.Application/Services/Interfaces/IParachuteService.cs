using Funifest.Application.DTO;

namespace Funifest.Application.Services.Interfaces;

public interface IParachuteService
{
    Task<IEnumerable<ParachuteDto>> GetAllAsync();
    Task<ParachuteDto?> GetByIdAsync(int id);
    Task<ParachuteDto> CreateAsync(CreateParachuteDto dto);
    Task<bool> DeleteAsync(int id);
}


