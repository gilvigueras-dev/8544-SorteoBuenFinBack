using AutoMapper;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs.UserOperations;
using SAT_API.Application.Interfaces.UserOperations;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services.UserOperations;

public class RoleManagementService : IRoleManagementService
{
    private readonly IRoleManagementRepository _roleManagementRepository;
    private readonly IMapper _mapper;

    public RoleManagementService(IRoleManagementRepository roleManagementRepository, IMapper mapper)
    {
        _roleManagementRepository = roleManagementRepository ?? throw new ArgumentNullException(nameof(roleManagementRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<IEnumerable<RoleManagementReponseDto>>> GetAllRolesAsync()
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var rolesDto = _mapper.Map<IEnumerable<RoleManagementReponseDto>>(roles);
        return Result<IEnumerable<RoleManagementReponseDto>>.Success(rolesDto);
    }

    public async Task<Result<RoleManagementReponseDto>> GetRoleByIdAsync(int roleId)
    {
        var role = await _roleManagementRepository.GetRoleByIdAsync(roleId);
        if (role == null)
        {
            return Result<RoleManagementReponseDto>.Failure("Role not found");
        }

        var roleDto = _mapper.Map<RoleManagementReponseDto>(role);
        return Result<RoleManagementReponseDto>.Success(roleDto);
    }
}
