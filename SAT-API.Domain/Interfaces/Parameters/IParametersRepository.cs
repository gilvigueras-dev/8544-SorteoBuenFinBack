using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Parameters;
using SAT_API.Domain.Entities.Parametros;

namespace SAT_API.Domain.Interfaces.Parameters;

public interface IParametersRepository
{
    Task<string> SetParametros(ParameterInsertRequest request);
    Task<List<ParameterResponse>> ObtenerParametros(int ejecucionId);
    Task<List<ParameterGenericResponse>> AllActiveParameters();
}
