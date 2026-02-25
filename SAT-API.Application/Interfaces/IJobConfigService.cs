using SAT_API.Application.DTOs.Insumos;

namespace SAT_API.Application.Interfaces;

public interface IJobConfigService
{
    Task<JobConfigDto?> GetByNombreAsync(string nombre);
}
