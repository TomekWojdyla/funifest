using Funifest.Application.DTO.ExitPlan;
using Funifest.Domain.Models;

namespace Funifest.Application.Services.Interfaces;

public interface IExitPlanService
{
    Task<IEnumerable<ExitPlan>> GetAllAsync();
    Task<ExitPlan?> GetByIdAsync(int id);
    Task<ExitPlan> CreateAsync(CreateExitPlanRequest request);
    Task<bool> DeleteAsync(int id);
}


