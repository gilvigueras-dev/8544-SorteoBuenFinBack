using SAT_API.Application.Common;
using SAT_API.Application.DTOs.Parameters;

namespace SAT_API.Application.Interfaces.Parameters;

public interface IParametersService
{
    Task<Result<string>> SetParameters(ParameterInsertRequestDto request);
    Task<Result<List<ParameterResponseDto>>> ObtenerParametros(int ejecucionId);
    Task<Result<List<ParameterGenericResponseDto>>> AllActiveParameters();
}
