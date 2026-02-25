using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.Track;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrackEjecucionController : ControllerBase
{
    private readonly ITrackEjecucionServices _trackEjecucionServices;
    private readonly IMapper _mapper;

    public TrackEjecucionController(ITrackEjecucionServices trackEjecucionServices, IMapper mapper)
    {
        _trackEjecucionServices = trackEjecucionServices;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Inserta un nuevo registro de seguimiento de ejecución y actualiza el registro anterior
    /// </summary>
    /// <remarks>
    /// Este endpoint permite insertar un nuevo registro en la tabla de seguimiento de ejecución 
    /// y automáticamente actualiza la fecha de cambio del registro anterior de la misma ejecución.
    /// 
    /// **Estatus disponibles por etapa:**
    /// 
    /// **Etapa 1:**
    /// - 1: Nuevo creado
    /// - 2: Validación insumos  
    /// - 3: Insumos validados
    /// - 4: Parámetros guardados
    /// - 5: Ejecución cruce
    /// - 6: Productos validados
    /// - 7: Tablas pobladas
    /// - 8: Etapa 1 finalizada
    /// 
    /// **Etapa 2:**
    /// - 9: Validación insumos
    /// - 10: Insumos validados
    /// - 11: Parámetros guardados
    /// - 12: Ejecución cruce
    /// - 13: Productos validados
    /// - 14: Tablas pobladas
    /// - 15: Etapa 2 finalizada
    /// 
    /// **Etapa 3:**
    /// - 16: Ejecución cruce
    /// - 17: Productos validados
    /// - 18: Tablas pobladas
    /// - 19: Etapa 3 finalizada
    /// 
    /// **Etapa 4:**
    /// - 20: Validación insumos
    /// - 21: Insumos validados
    /// - 22: Ejecución cruce
    /// - 23: Productos validados
    /// - 24: Etapa 4 finalizada
    /// </remarks>
    /// <param name="request">Datos requeridos para crear el registro de seguimiento</param>
    /// <returns>Información del nuevo registro creado</returns>
    /// <response code="200">Registro creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o error en el procesamiento</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarTrackEjecucionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarTrackEjecucionDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarTrackEjecucionDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<InsertarTrackEjecucionDto>>>> InsertarTrackEjecucionAsync([FromBody] InsertarTrackEjecucionRequestDto request)
    {
        var insertarTrackEjecucionRequestDto = _mapper.Map<InsertarTrackEjecucionRequestDto>(request);
        var result = await _trackEjecucionServices.InsertarTrackEjecucionAsync(insertarTrackEjecucionRequestDto);

        if (result.IsSuccess)
            return Ok(ApiResponse<List<InsertarTrackEjecucionDto>>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<List<InsertarTrackEjecucionDto>>.ErrorResponse(result.Message, result.Errors));
    }

    /// <summary>
    /// Obtiene lista de seguimiento de ejecución filtrando por el ID de ejecución
    /// </summary>    
    /// <param name="ejecucionId">el ID de ejecución</param>
    /// <returns>Lista de seguimiento de ejecución</returns>
    /// <response code="200">Registro creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o error en el procesamiento</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{ejecucionId}")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackEjecucionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<TrackEjecucionDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<TrackEjecucionDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<TrackEjecucionDto>>>> ObtenerTrackEjecucionAsync([FromRoute] int ejecucionId)
    {
        var result = await _trackEjecucionServices.ObtenerTrackEjecucionAsync(ejecucionId);

        if (result.IsSuccess)
            return Ok(ApiResponse<List<TrackEjecucionDto>>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<List<TrackEjecucionDto>>.ErrorResponse(result.Message, result.Errors));
    }

    /// <summary>
    /// Valida la población de datos para una ejecución específica y etapa
    /// </summary>
    /// <remarks>
    /// Este endpoint permite validar la población de datos para una ejecución específica y etapa.
    /// La validación se realiza enviando el ID de la ejecución y el ID de la etapa.
    /// </remarks>
    /// <param name="request">Datos requeridos para la validación de población de datos</param>
    /// <returns>Resultado de la validación de población de datos</returns>
    /// <response code="200">Validación exitosa</response>
    /// <response code="400">Datos de entrada inválidos o error en el procesamiento</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("validate-data-population")]
    [ProducesResponseType(typeof(ApiResponse<DataPopulationValidationResponseDto?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DataPopulationValidationResponseDto?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<DataPopulationValidationResponseDto?>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<DataPopulationValidationResponseDto?>>> DataPopulationValidationAsync([FromRoute] DataPopulationValidationRequestDto request)
    {
        var requestDto = _mapper.Map<DataPopulationValidationRequestDto>(request);
        var result = await _trackEjecucionServices.DataPopulationValidation(requestDto);
        if (result.IsSuccess)
            return Ok(ApiResponse<DataPopulationValidationResponseDto?>.SuccessResponse(result.Value, result.Message));
        return Ok(ApiResponse<DataPopulationValidationResponseDto?>.ErrorResponse(result.Message, result.Errors));
    }

    /// <summary>
    /// Realiza la población de datos para una ejecución específica
    /// </summary>
    /// <remarks>
    /// Este endpoint permite realizar la población de datos para una ejecución específica.
    /// La población de datos se realiza enviando el ID de la etapa.
    /// StageId debe contener el ID de la etapa para la cual se desea realizar la población de datos.
    /// </remarks>
    /// <param name="request"></param>
    /// <returns>El estatus del proceso población de datos</returns>
    /// <response code="200">Población de datos completada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o error en el procesamiento</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("data-population")]
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RunJobResponseDto>>> DataPopulationAsync([FromBody] DataPopulationRequestDto request)
    {
        var requestDto = _mapper.Map<DataPopulationRequestDto>(request);
        var result = await _trackEjecucionServices.DataPopulation(requestDto);
        if (result.IsSuccess)
            return Ok(ApiResponse<RunJobResponseDto>.SuccessResponse(result.Value!, result.Message));
        return Ok(ApiResponse<RunJobResponseDto>.ErrorResponse(result.Message, result.Errors));
    }
}