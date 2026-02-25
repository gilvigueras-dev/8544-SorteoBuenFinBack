using AutoMapper;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.Insumos;
using SAT_API.Application.DTOs.Parameters;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.DTOs.Process;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.DTOs.Track;
using SAT_API.Application.DTOs.UserOperations;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Databricks;
using SAT_API.Domain.Entities.Parameters;
using SAT_API.Domain.Entities.Parametros;
using SAT_API.Domain.Entities.PistasAuditoria;
using SAT_API.Domain.Entities.Process;
using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Entities.Track;
using SAT_API.Domain.Entities.UserOperations;
using SAT_API.Domain.Enums;
using System.Data;

namespace SAT_API.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ParameterResponse, ParameterResponseDto>().ReverseMap();
            CreateMap<TrackEjecucion, TrackEjecucionDto>().ReverseMap();
            CreateMap<JobExecuteRunResponse, ProcesoValidacionDto>().ReverseMap();
            CreateMap<InsertarEjecucionRequestDto, Ejecucion>().ReverseMap();
            CreateMap<JobConfig, JobConfigDto>().ReverseMap();
            CreateMap<EstatusValidacion, EstatusValidacionDto>()
                .ForMember(dest => dest.Archivo, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.Validado, opt => opt.MapFrom(src => src.IsValidated))
                .ForMember(dest => dest.Existe, opt => opt.MapFrom(src => src.Exists))
                .ForMember(dest => dest.ArchivoId, opt => opt.MapFrom(src => src.FileId))
                .ForMember(dest => dest.Warning, opt => opt.MapFrom(src => src.Warning))
                .ForMember(dest => dest.ConsultaCC, opt => opt.MapFrom(src => src.CCGenerated))
                .ForMember(dest => dest.UrlCC, opt => opt.MapFrom(src => src.CCPath))
                .ForMember(dest => dest.Descargado, opt => opt.MapFrom(src => src.Downloaded))
                .ReverseMap();
            CreateMap<Insumo, InsumoDto>()
                .ForMember(dest => dest.ArchivoId, opt => opt.MapFrom(src => src.FileId))
                .ForMember(dest => dest.Archivo, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.Existe, opt => opt.MapFrom(src => src.Exists))
                .ForMember(dest => dest.Validado, opt => opt.MapFrom(src => src.IsValidated))
                .ForMember(dest => dest.Warning, opt => opt.MapFrom(src => src.Warning))
                .ForMember(dest => dest.ConsultaCC, opt => opt.MapFrom(src => src.CCGenerated))
                .ForMember(dest => dest.UrlCC, opt => opt.MapFrom(src => src.CCPath))
                .ForMember(dest => dest.Descargado, opt => opt.MapFrom(src => src.Downloaded))
                .ReverseMap();
            CreateMap<ProcesoValidacion, ProcesoValidacionDto>().ReverseMap();
            CreateMap<EjecucionDto, Ejecucion>().ReverseMap();
            CreateMap<InsertarEjecucionDto, Ejecucion>().ReverseMap();
            CreateMap<int, InsertarTrackEjecucionDto>()
            .ForMember(dest => dest.NuevoTrackId, opt => opt.MapFrom(src => src));
            CreateMap<int, ActualizarEstatusEjecucionIdResponseDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));
            CreateMap<ParameterInsertRequestDto, ParameterInsertRequest>()
                .ForMember(dest => dest.ExecutionId, opt => opt.MapFrom(src => src.ExecutionId))
                .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => string.Join(",", src.Parameters.Values.Select(e => $"'{e.Replace("'", "''")}'"))));
            CreateMap<ProductResponse, ProductResponseDto>()
                .ForMember(dest => dest.IdArchivo, opt => opt.MapFrom(src => src.IdArchivo))
                .ForMember(dest => dest.NombreArchivo, opt => opt.MapFrom(src => src.NombreArchivo))
                .ForMember(dest => dest.RutaArchivo, opt => opt.MapFrom(src => src.RutaArchivo))
                .ForMember(dest => dest.Estatus, opt => opt.MapFrom(src => src.Estatus))
                .ForMember(dest => dest.IdProducto, opt => opt.MapFrom(src => src.IdProducto))
                .ForMember(dest => dest.EstatusRepositorio, opt => opt.MapFrom(src => src.EstatusRepositorio))
                .ForMember(dest => dest.ConsultaCC, opt => opt.MapFrom(src => src.CCGenerated))
                .ForMember(dest => dest.UrlCC, opt => opt.MapFrom(src => src.CCPath))
                .ReverseMap();
            CreateMap<ProductRequestDto, ProductRequest>()
                .ForMember(dest => dest.IdEjecucion, opt => opt.MapFrom(src => src.IdEjecucion))
                .ForMember(dest => dest.IdEtapa, opt => opt.MapFrom(src => src.IdEtapa))
                .ReverseMap();
            CreateMap<IntersectProcessStatusRequestDto, IntersectProcessStatusRequest>();
            CreateMap<IntersectProcessStatusResponse, IntersectProcessStatusResponseDto>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();
            CreateMap<CancelarEjecucionRequestDto, ActualizarEstatusEjecucionRequestDto>()
                .ForMember(dest => dest.EjecucionId, opt => opt.MapFrom(src => src.EjecucionId))
                .ForMember(dest => dest.Comentario, opt => opt.MapFrom(src => src.Comentario))
                .ForMember(dest => dest.EstatusEjecucion, opt => opt.MapFrom(src => EstatusEjecucion.Cancelado))
                .ReverseMap();
            CreateMap<ProductStatusRequestDto, ProductStatusRequest>()
                .ForMember(dest => dest.IdEjecucion, opt => opt.MapFrom(src => src.IdEjecucion))
                .ForMember(dest => dest.IdArchivo, opt => opt.MapFrom(src => src.IdArchivo));
            CreateMap<ControlNumbersRequestDto, ControlNumbersRequest>();
            CreateMap<ControlNumbersResponse, ControlNumbersResponseDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ControlNumberName, opt => opt.MapFrom(src => src.ControlNumberName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ReverseMap();
            CreateMap<ControlNumbersUrlResponse, ControlNumbersUrlResponseDto>();
            CreateMap<DataPopulationValidationRequestDto, DataPopulationValidationRequest>()
                .ForMember(dest => dest.ExecutionId, opt => opt.MapFrom(src => src.ExecutionId))
                .ForMember(dest => dest.StageId, opt => opt.MapFrom(src => src.StageId));
            CreateMap<DataPopulationValidationResponse, DataPopulationValidationResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ReverseMap();
            CreateMap<ParameterGenericResponse, ParameterGenericResponseDto>().ReverseMap();
            CreateMap<JobRun, RunStatusJobResponseDto>()
                .ForMember(dest => dest.RunId, opt => opt.MapFrom(src => src.RunId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State.ResultState ?? string.Empty))
                .ForMember(dest => dest.RunState, opt => opt.MapFrom(src => src.State.LifeCicleState ?? string.Empty))
                .ReverseMap();
            CreateMap<ControlNumbersFileResponse, ControlNumbersFileResponseDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ControlNumber, opt => opt.MapFrom(src => src.ControlNumber))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ReverseMap();
            CreateMap<RoleManagement, RoleManagementReponseDto>()
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.RolOrigen, opt => opt.MapFrom(src => src.RoleOrigin))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.RoleDescription))
                .ForMember(dest => dest.RolTecnico, opt => opt.MapFrom(src => src.RoleTechnician))
            .ReverseMap();
            CreateMap<SystemAuditRequestDto, SystemAuditRequest>();
            CreateMap<UpdateSystemAuditRequestDto, UpdateSystemAuditRequest>();
            CreateMap<ProductsExcelExportRequestDto, ProductsExcelExportRequest>();
            CreateMap<StatusFileDownloadedResponse, FileStatusDownloadedResponseDto>().ReverseMap();
            CreateMap<FileStatusDownloadedRequestDto, StatusFileDownloadedRequest>();
            CreateMap<FinishStatusAuditSystemRequestDto, FinishStatusAuditSystemRequest>();
            CreateMap<StageCatalogResponse, StageCatalogResponseDto>().ReverseMap();
        }
    }
}