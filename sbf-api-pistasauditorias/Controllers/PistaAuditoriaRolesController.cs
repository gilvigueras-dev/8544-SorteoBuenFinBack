using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs.UserOperations;
using SAT_API.Application.Interfaces.UserOperations;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/PistaAuditoria")]
public class PistaAuditoriaRolesController : ControllerBase
{
    private readonly IRoleManagementService _roleManagementService;

    public PistaAuditoriaRolesController(IRoleManagementService roleManagementService)
    {
        _roleManagementService = roleManagementService;
    }

    [HttpGet]
    [Route("GetRoles")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleManagementReponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleManagementReponseDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleManagementReponseDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<RoleManagementReponseDto>>>> GetRoles()
    {
        var result = await _roleManagementService.GetAllRolesAsync();

        if (result.IsSuccess)
            return Ok(ApiResponse<List<RoleManagementReponseDto>>.SuccessResponse(result.Value!.ToList(), result.Message));

        return BadRequest(ApiResponse<List<RoleManagementReponseDto>>.ErrorResponse(result.Message, result.Errors));
    }
}
