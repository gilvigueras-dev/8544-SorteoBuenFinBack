using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs.Parameters;
using SAT_API.Application.Interfaces.Parameters;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametersController : ControllerBase
    {
        private readonly IParametersService _parametersService;

        public ParametersController(IParametersService parametersService)
        {
            _parametersService = parametersService;
        }

        /// <summary>
        /// Setea los parámetros de la fase 'N'
        /// </summary>
        /// <param name="request">Parámetros a setear</param>
        /// <returns>Resultado de la operación</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("SetParameters")]
        public async Task<ActionResult<ApiResponse<string>>> SetParameters([FromBody] ParameterInsertRequestDto request)
        {
            var result = await _parametersService.SetParameters(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(result.Value!, result.Message));
            }
            return Ok(ApiResponse<string>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Se obtiene los parámetros de la ejecución especificada
        /// </summary>
        /// <param name="ejecucionId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<List<ParameterResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ParameterResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<ParameterResponseDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<ParameterResponseDto>>), StatusCodes.Status500InternalServerError)]
        [HttpGet("{ejecucionId}")]
        public async Task<ActionResult<ApiResponse<List<ParameterResponseDto>>>> ObtenerParametros([FromRoute] int ejecucionId)
        {
            var result = await _parametersService.ObtenerParametros(ejecucionId);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<ParameterResponseDto>>.SuccessResponse(result.Value!, result.Message));
            }
            return Ok(ApiResponse<List<ParameterResponseDto>>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Obtiene todos los parámetros activos
        /// </summary>
        /// <returns>Lista de parámetros activos</returns>
        [ProducesResponseType(typeof(ApiResponse<List<ParameterGenericResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ParameterGenericResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<ParameterGenericResponseDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<ParameterGenericResponseDto>>), StatusCodes.Status500InternalServerError)]
        [HttpGet("AllActiveParameters")]
        public async Task<ActionResult<ApiResponse<List<ParameterGenericResponseDto>>>> AllActiveParameters()
        {
            var result = await _parametersService.AllActiveParameters();
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<ParameterGenericResponseDto>>.SuccessResponse(result.Value!, result.Message));
            }
            return Ok(ApiResponse<List<ParameterGenericResponseDto>>.ErrorResponse(result.Message, result.Errors));
        }
    }
}
