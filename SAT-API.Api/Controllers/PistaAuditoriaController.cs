using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PistaAuditoriaController : ControllerBase
{
    private readonly IPistaAuditoriaServices _pistaAuditoriaServices;

    public PistaAuditoriaController(IPistaAuditoriaServices pistaAuditoriaServices)
    {
        _pistaAuditoriaServices = pistaAuditoriaServices;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarPistaAuditoriaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarPistaAuditoriaDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<InsertarPistaAuditoriaDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<InsertarPistaAuditoriaDto>>>> Insertar([FromBody] InsertarPistaAuditoriaDto request)
    {

        var result = await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(request);

        if (result.IsSuccess)
            return Ok(ApiResponse<List<InsertarPistaAuditoriaDto>>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<List<InsertarPistaAuditoriaDto>>.ErrorResponse(result.Message, result.Errors));
    }

}
