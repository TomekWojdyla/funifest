using Funifest.Application.DTO;

namespace Funifest.Application.Services.Interfaces;

public interface IParachuteService
{
    Task<IEnumerable<ParachuteDto>> GetAllAsync();
    Task<ParachuteDto?> GetByIdAsync(int id);
    Task<ParachuteDto> CreateAsync(CreateParachuteDto dto);
    Task<ParachuteDto?> BlockAsync(int id);
    Task<ParachuteDto?> UnblockAsync(int id);
    Task<bool> DeleteAsync(int id);
}
