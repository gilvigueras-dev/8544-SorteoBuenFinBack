using AutoMapper;
using SAT_API.Application.DTOs.Insumos;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Interfaces;

namespace SAT_API.Application.Services;

public class JobConfigService : IJobConfigService
{
    private readonly IJobConfigRepository _jobConfigRepository;
    private readonly IMapper _mapper;

    public JobConfigService(IJobConfigRepository jobConfigRepository, IMapper mapper)
    {
        _jobConfigRepository = jobConfigRepository;
        _mapper = mapper;
    }

    public async Task<JobConfigDto?> GetByNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        }

        var jobConfig = await _jobConfigRepository.GetByNombreAsync(nombre);

        if (jobConfig == null)
        {
            throw new KeyNotFoundException($"No job configuration found for nombre: {nombre}");
        }

        return _mapper.Map<JobConfigDto>(jobConfig);
    }
}
