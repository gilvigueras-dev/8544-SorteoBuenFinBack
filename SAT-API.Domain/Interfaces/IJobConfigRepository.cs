using SAT_API.Domain.Entities;

namespace SAT_API.Domain.Interfaces;

public interface IJobConfigRepository
{
    Task<JobConfig?> GetByNombreAsync(string nombre);
}
