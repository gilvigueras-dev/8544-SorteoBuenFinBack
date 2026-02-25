using AutoMapper;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.DTOs.UserOperations;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Excel;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Entities.UserOperations;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services;

public class PistaAuditoriaServices : IPistaAuditoriaServices
{
    private readonly IPistaAuditoriaRepository _pistaAuditoriaRepository;
    private readonly ITranslator _translator;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IValidationService _validationService;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;
    private readonly IAddressClientService _addressClientService;

    public PistaAuditoriaServices(
        IPistaAuditoriaRepository pistaAuditoriaRepository,
        ITranslator translator,
        IMapper mapper,
        IExcelService excelService,
        IValidationService validationService,
        IAddressClientService addressClientService,
        IClaimService claimService)
    {
        _pistaAuditoriaRepository = pistaAuditoriaRepository ?? throw new ArgumentNullException(nameof(pistaAuditoriaRepository));
        _translator = translator ?? throw new ArgumentNullException(nameof(translator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _addressClientService = addressClientService ?? throw new ArgumentNullException(nameof(addressClientService));
        _currentUser = claimService?.GetCurrentUser() ?? throw new ArgumentNullException(nameof(claimService));
    }

    public async Task<Result<List<InsertarPistaAuditoriaDto>>> InsertarPistaAuditoriaAsync(InsertarPistaAuditoriaDto request)
    {
        try
        {
            await _pistaAuditoriaRepository.InsertarTrackEjecucionAsync(request.EjecucionId, request.Actividad, request.Descripcion);
            List<InsertarPistaAuditoriaDto> lista = new List<InsertarPistaAuditoriaDto> { request };
            return Result<List<InsertarPistaAuditoriaDto>>.Success(lista, _translator["ApplicationAuditTrailSuccess"]);
        }
        catch (Exception ex)
        {
            return Result<List<InsertarPistaAuditoriaDto>>.Failure(
                _translator["ApplicationAuditTrailFailure"],
                "INTERNAL_ERROR",
                ex.Message
            );
        }
    }

    public async Task<Result<string>> InsertSystemAudiAsync(SystemAuditRequestDto request)
    {
        var systemAuditRequest = _mapper.Map<SystemAuditRequest>(request);
        systemAuditRequest.MacAddressIp = _addressClientService.ObtenerIPCliente();
        systemAuditRequest.User = _currentUser.GetRFC();

        var result = await _pistaAuditoriaRepository.InsertSystemAudiAsync(systemAuditRequest);

        if (!string.IsNullOrEmpty(result))
        {
            return Result<string>.Success(result, _translator["ApplicationSystemAuditSuccess"]);
        }

        // Lanzar excepción de negocio para que sea manejada por el middleware
        throw new Middlewares.BusinessException(
            _translator["ApplicationSystemAuditFailure"],
            "No se pudo insertar la auditoría del sistema"
        );
    }

    public async Task<Result<string>> UpdateSystemAuditAsync(UpdateSystemAuditRequestDto request)
    {
        var systemAuditRequest = _mapper.Map<UpdateSystemAuditRequest>(request);

        if (request.Id < 1)
        {
            return Result<string>.Failure(_translator["ApplicationUpdatedAuditFailurelessZero"]);
        }
        var result = await _pistaAuditoriaRepository.UpdateSystemAuditAsync(systemAuditRequest);

        if (!string.IsNullOrEmpty(result))
        {
            return Result<string>.Success(result, _translator["ApplicationUpdateSystemAuditSuccess"]);
        }

        throw new Middlewares.BusinessException(
            _translator["ApplicationUpdateSystemAuditFailure"],
            "No se pudo actualizar la auditoría del sistema"
        );
    }

    public async Task<Result<byte[]>> GetAuditTrailsAsync(ProductsExcelExportRequestDto request)
    {
        var productsExcelExportRequest = _mapper.Map<ProductsExcelExportRequest>(request);

        if (productsExcelExportRequest.EndDate.HasValue)
        {
            productsExcelExportRequest.EndDate = productsExcelExportRequest.EndDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        var result = await _pistaAuditoriaRepository.GetAuditTrailsAsync(productsExcelExportRequest);

        if (result != null)
        {
            var excelBytes = await _excelService.GenerarExcelProductos(result);
            return Result<byte[]>.Success(excelBytes, _translator["ApplicationGetAuditTrailsSuccess"]);
        }

        // Lanzar excepción de negocio para que sea manejada por el middleware
        throw new Middlewares.BusinessException(
            _translator["ApplicationGetAuditTrailsFailure"],
            "No se pudieron obtener las pistas de auditoría"
        );
    }

    public async Task<Result<List<ActionsCatalogResponse>>> GetActionsCatalogAsync()
    {
        try
        {
            var actions = await _pistaAuditoriaRepository.GetActionsCatalogAsync();
            var response = _mapper.Map<List<ActionsCatalogResponse>>(actions);
            return Result<List<ActionsCatalogResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<ActionsCatalogResponse>>.Failure(
                _translator["ApplicationGetActionsCatalogFailure"],
                "INTERNAL_ERROR",
                ex.Message
            );
        }
    }

    public async Task<Result<string>> SetFinishedAuditAsync(FinishStatusAuditSystemRequestDto request)
    {
        // Validar el request
        await _validationService.ValidateAndThrowAsync(request);

        var finishStatusAuditSystemRequest = _mapper.Map<Domain.Entities.PistasAuditoria.FinishStatusAuditSystemRequest>(request);
        var result = await _pistaAuditoriaRepository.SetFinishedAuditAsync(finishStatusAuditSystemRequest);

        if (!string.IsNullOrEmpty(result))
        {
            return Result<string>.Success(result, _translator["ApplicationSetFinishedAuditSuccess"]);
        }

        // Lanzar excepción de negocio para que sea manejada por el middleware
        throw new Middlewares.BusinessException(
            _translator["ApplicationSetFinishedAuditFailure"],
            string.Format(_translator["ApplicationSetFinishedAuditFailureDetails"], request.AuditSystemId)
        );
    }

    public async Task<Result<List<StageCatalogResponseDto>>> GetStagesCatalogAsync()
    {
        var stages = await _pistaAuditoriaRepository.GetStagesCatalogAsync();

        if(stages == null || stages.Count > 0)
        {
            return Result<List<StageCatalogResponseDto>>.Failure(
                _translator["ApplicationGetStagesCatalogFailure"]
            );
        }

        var response = _mapper.Map<List<StageCatalogResponseDto>>(stages);
        return Result<List<StageCatalogResponseDto>>.Success(response);
    }
}