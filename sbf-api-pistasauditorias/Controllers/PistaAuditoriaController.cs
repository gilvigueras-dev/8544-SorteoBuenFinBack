using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.DTOs.UserOperations;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PistaAuditoriaController : ControllerBase
{
    private readonly IPistaAuditoriaServices _pistaAuditoriaServices;

    public PistaAuditoriaController(IPistaAuditoriaServices pistaAuditoriaServices)
    {
        _pistaAuditoriaServices = pistaAuditoriaServices;
    }

    [HttpPost]
    [Route("Insertar")]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarPistaAuditoriaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarPistaAuditoriaDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarPistaAuditoriaDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<InsertarPistaAuditoriaDto>>>> Insertar([FromBody] InsertarPistaAuditoriaDto request)
    {
        var result = await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(request);

        if (result.IsSuccess)
            return Ok(ApiResponse<List<InsertarPistaAuditoriaDto>>.SuccessResponse(result.Value!, result.Message));

        return BadRequest(ApiResponse<InsertarPistaAuditoriaDto>.ErrorResponse(result.Message, result.Errors));
    }


    [HttpPost]
    [Route("InsertSystemAudit")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<string>>> InsertSystemAudit([FromBody] SystemAuditRequestDto request)
    {
        var result = await _pistaAuditoriaServices.InsertSystemAudiAsync(request);

        if (result.IsSuccess)
            return Ok(ApiResponse<string>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<string>.ErrorResponse(result.Message, result.Errors));
    }


    [HttpPut]
    [Route("UpdateSystemAudit")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<string>>> UpdateSystemAudit([FromBody] UpdateSystemAuditRequestDto request)
    {
        var result = await _pistaAuditoriaServices.UpdateSystemAuditAsync(request);

        if (result.IsSuccess)
            return Ok(ApiResponse<string>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<string>.ErrorResponse(result.Message, result.Errors));
    }

    [HttpPost("SetFinishedAudit")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<string>>> SetFinishedAuditAsync([FromBody] FinishStatusAuditSystemRequestDto request)
    {
        var result = await _pistaAuditoriaServices.SetFinishedAuditAsync(request);

        if (result.IsSuccess)
            return Ok(ApiResponse<string>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<string>.ErrorResponse(result.Message, result.Errors));
    }

}