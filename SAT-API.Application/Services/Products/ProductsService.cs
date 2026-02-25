using SAT_API.Application.Common;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.Interfaces.Products;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Interfaces.Products;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Enums;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services.Products;

public class ProductsService : IProductsService
{
    private readonly IProductRepository _productRepository;
    private readonly IPistaAuditoriaServices _pistaAuditoriaServices;
    private readonly IJobConfigService _jobConfigService;
    private readonly IDatabricksRepository _databricksRepository;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;
    private readonly IRoleManagementRepository _roleManagementRepository;
    private readonly IProductsInfrastructure _infrastructure;
    private const string ApplicationPermissionError = "ApplicationProductsPermissionDenied";

    public ProductsService(
        IProductRepository productRepository,
        IPistaAuditoriaServices pistaAuditoriaServices,
        IJobConfigService jobConfigService,
        IDatabricksRepository databricksRepository,
        IRoleManagementRepository roleManagementRepository,
        IClaimService claimService,
        IProductsInfrastructure infrastructure)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _pistaAuditoriaServices = pistaAuditoriaServices ?? throw new ArgumentNullException(nameof(pistaAuditoriaServices));
        _jobConfigService = jobConfigService ?? throw new ArgumentNullException(nameof(jobConfigService));
        _databricksRepository = databricksRepository ?? throw new ArgumentNullException(nameof(databricksRepository));
        _roleManagementRepository = roleManagementRepository ?? throw new ArgumentNullException(nameof(roleManagementRepository));
        _currentUser = claimService?.GetCurrentUser() ?? throw new ArgumentNullException(nameof(claimService));
        _infrastructure = infrastructure ?? throw new ArgumentNullException(nameof(infrastructure));
    }

    public async Task<Result<List<ProductResponseDto>>> GetProducts(ProductRequestDto request)
    {
        // Check if the user has the required permissions
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
                    return Result<List<ProductResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<ProductResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<ProductResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        // Validate the request using the validation service
        await _infrastructure.ValidationService.ValidateAndThrowAsync(request);
        var input = _infrastructure.Mapper.Map<ProductRequest>(request);
        var products = await _productRepository.GetProducts(input);
        if (products == null || products.Count > 0)
        {
            return Result<List<ProductResponseDto>>.Failure(_infrastructure.Translator["ApplicationProductsFailure"]);
        }
        var productDtos = _infrastructure.Mapper.Map<List<ProductResponseDto>>(products);

        return Result<List<ProductResponseDto>>.Success(productDtos);
    }

    public async Task<Result<string>> SetProductStatus(ProductStatusRequestDto request)
    {
        // Check if the user has the required permissions
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
                    return Result<string>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<string>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<string>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        // Validate the request using the validation service
        await _infrastructure.ValidationService.ValidateAndThrowAsync(request);
        var input = _infrastructure.Mapper.Map<ProductStatusRequest>(request);
        var result = await _productRepository.SetProductStatus(input);
        if (string.IsNullOrEmpty(result) || result.Contains("Error"))
        {
            return Result<string>.Failure($"{_infrastructure.Translator["ApplicationProductsStatuFailure"]}{result}");
        }
        await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto()
        { EjecucionId = request.IdEjecucion, Actividad = $"SetProductStatus - {request.IdArchivo}", Descripcion = $"Se actualiza el estatus productos - idArchivo: {request.IdArchivo}, ejecuci�n: {request.IdEjecucion}" });
        return Result<string>.Success(result);
    }

    public async Task<Result<List<ControlNumbersResponseDto>>> ControlNumberList(ControlNumbersRequestDto request)
    {
        // Check if the user has the required permissions
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
                    return Result<List<ControlNumbersResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<ControlNumbersResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<ControlNumbersResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        // Validate the request using the validation service
        await _infrastructure.ValidationService.ValidateAndThrowAsync(request);

        var input = _infrastructure.Mapper.Map<ControlNumbersRequest>(request);

        var result = await _productRepository.ControlNumbersList(input);

        if (result.Count > 0)
        {
            var mappedList = _infrastructure.Mapper.Map<List<ControlNumbersResponseDto>>(result);
            return Result<List<ControlNumbersResponseDto>>.Success(mappedList, _infrastructure.Translator["SuccessMessage"]);
        }

        return Result<List<ControlNumbersResponseDto>>.Failure(_infrastructure.Translator["ApplicationControlNumbersFailure"]);
    }

    public async Task<Result<ControlNumbersUrlResponseDto?>> ControlNumbersUrl(ControlNumbersRequestDto request)
    {
        var input = _infrastructure.Mapper.Map<ControlNumbersRequest>(request);
        var result = await _productRepository.ControlNumbersUrl(input);

        if (result != null)
        {
            var mapped = _infrastructure.Mapper.Map<ControlNumbersUrlResponseDto>(result);
            return Result<ControlNumbersUrlResponseDto?>.Success(mapped, _infrastructure.Translator["SuccessMessage"]);
        }

        return Result<ControlNumbersUrlResponseDto?>.Failure(_infrastructure.Translator["ApplicationControlNumbersFailure"]);
    }

    public async Task<Result<MoveFileResponseDto>> GetMoverArchivo(int ejecucionId, int numeroEtapa, int productoId)
    {
        // Check if the user has the required permissions
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
                    return Result<MoveFileResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<MoveFileResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<MoveFileResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var jobName = "MoverArchivoaRepositorio";

            if (string.IsNullOrEmpty(jobName))
            {
                return Result<MoveFileResponseDto>.Failure(
                    $"{jobName} not found on app config",
                    "CONFIG_PRODUCTS_NOT_FOUND",
                    $"{jobName} not found on app config"
                );
            }


            var jobConfig = await _jobConfigService.GetByNombreAsync(jobName);

            if (jobConfig == null)
            {
                return Result<MoveFileResponseDto>.Failure(
                    "JobId not found on database",
                    "CONFIG_PRODUCTS__NOT_FOUND",
                    $"No se encontró JobId: {jobName}"
                );
            }

            long jobId = (long)Convert.ToDecimal(jobConfig.JobId);
            var jobParametros = new Dictionary<string, object>
                {
                    { "etapa", numeroEtapa },
                    { "id_ejecucion", ejecucionId },
                    { "id_producto", productoId },
                    { "rfc", _currentUser.GetRFC()},
                    { "ip", _infrastructure.AddressClientService.ObtenerIPCliente() }
                };
            var resultado = await _databricksRepository.RunJobAsync(jobId, jobParametros);

            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = ejecucionId, Actividad = $"MoverArchivoProducto - {numeroEtapa}", Descripcion = $"Se mueve archivo de la etapa: {numeroEtapa}, ejecución: {ejecucionId}, producto: {productoId}" });
            return Result<MoveFileResponseDto>.Success(new MoveFileResponseDto { NumberInJob = resultado.NumberInJob, RunId = resultado.RunId }, "Proceso ejecutado exitosamente");
        }
        catch (Exception ex)
        {
            return Result<MoveFileResponseDto>.Failure(
                "Error interno del servidor",
                "INTERNAL_ERROR",
                ex.Message
            );
        }
    }

    public async Task<Result<List<ControlNumbersFileResponseDto>>> ControlNumbersFile(ControlNumbersRequestDto request)
    {
        // Check if the user has the required permissions
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
                    return Result<List<ControlNumbersFileResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<ControlNumbersFileResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<ControlNumbersFileResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        var input = _infrastructure.Mapper.Map<ControlNumbersRequest>(request);
        var result = await _productRepository.ControlNumbersFile(input);
        if (result.Count > 0)
        {
            var mappedList = _infrastructure.Mapper.Map<List<ControlNumbersFileResponseDto>>(result);
            return Result<List<ControlNumbersFileResponseDto>>.Success(mappedList, _infrastructure.Translator["SuccessMessage"]);
        }
        return Result<List<ControlNumbersFileResponseDto>>.Failure(_infrastructure.Translator["ApplicationControlNumbersFailure"]);
    }

}
