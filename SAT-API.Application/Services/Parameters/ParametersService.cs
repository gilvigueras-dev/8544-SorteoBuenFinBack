using AutoMapper;
using Microsoft.Extensions.Logging;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs.Parameters;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Parameters;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Domain.Entities.Parametros;
using SAT_API.Domain.Enums;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Parameters;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services.Parameters;

public class ParametersService(
    IParametersRepository parametersRepository,
    IMapper mapper,
    ITranslator translator,
    IValidationService validationService,
    ILogger<ParametersService> logger,
    IClaimService claimService,
    IRoleManagementRepository roleManagementRepository) : IParametersService
{
    private readonly IParametersRepository _parametersRepository = parametersRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ITranslator _translator = translator;
    private readonly IValidationService _validationService = validationService;
    private readonly ILogger<ParametersService> _logger = logger;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser = claimService.GetCurrentUser();
    private readonly IRoleManagementRepository _roleManagementRepository = roleManagementRepository;
    private const string ApplicationPermissionError = "ApplicationPermissionDenied";
    public async Task<Result<string>> SetParameters(ParameterInsertRequestDto request)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario }.Contains(role.RoleName))
                {
                     return Result<string>.Failure(_translator[ApplicationPermissionError]);
                }
            }
            else
            {
                 return Result<string>.Failure(_translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<string>.Failure(_translator[ApplicationPermissionError]);
        }

        _logger.LogInformation("Setting parameters for execution ID: {ExecutionId}", request.ExecutionId);
        await _validationService.ValidateAndThrowAsync(request);

        var parameterInsertRequest = _mapper.Map<ParameterInsertRequest>(request);
        _logger.LogInformation("Mapped ParameterInsertRequest: {@ParameterInsertRequest}", parameterInsertRequest);
        var result = await _parametersRepository.SetParametros(parameterInsertRequest);

        if (string.IsNullOrEmpty(result))
        {
            return Result<string>.Failure(_translator["ApplicationParametersSetFailed"], "PARAMETERS_SET_FAILED", _translator["ApplicationParametersDbError"]);
        }

        return Result<string>.Success(result);
    }

    public async Task<Result<List<ParameterResponseDto>>> ObtenerParametros(int ejecucionId)
    {
         var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario, RolesUsuario.Invitado }.Contains(role.RoleName))
                {
                     return Result<List<ParameterResponseDto>>.Failure(_translator[ApplicationPermissionError]);
                }
            }
            else
            {
                 return Result<List<ParameterResponseDto>>.Failure(_translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<ParameterResponseDto>>.Failure(_translator[ApplicationPermissionError]);
        }

        if (ejecucionId <= 0)
            throw new ArgumentException(_translator["ApplicationParametersExecutionIdInvalid"], nameof(ejecucionId));

        var result = await _parametersRepository.ObtenerParametros(ejecucionId);
        var mappedResult = _mapper.Map<List<ParameterResponseDto>>(result);
        return Result<List<ParameterResponseDto>>.Success(mappedResult, $"{_translator["ApplciationParameterSuccessEjecutionId"]} {ejecucionId}");
    }

    public async Task<Result<List<ParameterGenericResponseDto>>> AllActiveParameters()
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario, RolesUsuario.Invitado }.Contains(role.RoleName))
                {
                    return Result<List<ParameterGenericResponseDto>>.Failure(_translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<ParameterGenericResponseDto>>.Failure(_translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<ParameterGenericResponseDto>>.Failure(_translator[ApplicationPermissionError]);
        }

        var result = await _parametersRepository.AllActiveParameters();
        var mappedResult = _mapper.Map<List<ParameterGenericResponseDto>>(result);
        return Result<List<ParameterGenericResponseDto>>.Success(mappedResult, _translator["ApplicationParametersAllActiveSuccess"]);
    }

}
