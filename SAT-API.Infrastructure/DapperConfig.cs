using System.Reflection;
using Dapper;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Authentication;
using SAT_API.Domain.Entities.Databricks;
using SAT_API.Domain.Entities.Parameters;
using SAT_API.Domain.Entities.PistasAuditoria;
using SAT_API.Domain.Entities.Process;
using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Entities.Track;
using SAT_API.Domain.Entities.UserOperations;

namespace SAT_API.Infrastructure;

public static class DapperConfig
{
    public static void Configure()
    {
        // Configurar para usar snake_case automáticamente
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        // Configurar para usar el mapeo de columnas personalizado
        ConfigureColumnMapping<DataPopulationValidationResponse>();
        ConfigureColumnMapping<ProductResponse>();
        ConfigureColumnMapping<ControlNumbersResponse>();
        ConfigureColumnMapping<ControlNumbersUrlResponse>();
        ConfigureColumnMapping<Ejecucion>();
        ConfigureColumnMapping<IntersectProcessStatusResponse>();
        ConfigureColumnMapping<ParameterResponse>();
        ConfigureColumnMapping<ParameterGenericResponse>();
        ConfigureColumnMapping<Insumo>();
        ConfigureColumnMapping<EstatusValidacion>();
        ConfigureColumnMapping<DbfsFilePathResponse>();
        ConfigureColumnMapping<ControlNumbersFileResponse>();
        ConfigureColumnMapping<UserRoleResponse>();
        ConfigureColumnMapping<RoleManagement>();
        ConfigureColumnMapping<SystemAuditRequest>();
        ConfigureColumnMapping<StatusFileDownloadedResponse>();
        ConfigureColumnMapping<StatusFileDownloadedRequest>();
        ConfigureColumnMapping<FinishStatusAuditSystemRequest>();
        ConfigureColumnMapping<StageCatalogResponse>();
    }

    private static void ConfigureColumnMapping<T>()
    {
        var map = new CustomPropertyTypeMap(typeof(T), (type, columnName) =>
        {
            // Buscar propiedades con atributo [Column]
            var propertyWithColumn = type.GetProperties()
                .FirstOrDefault(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnMapAttribute>();
                    return columnAttr?.Name?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true;
                });

            if (propertyWithColumn != null)
                return propertyWithColumn;

            // Si no encuentra por atributo, buscar por nombre
            return type.GetProperties()
                .FirstOrDefault(prop => prop.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                !; // null-forgiving operator to suppress nullable warning
        });

        SqlMapper.SetTypeMap(typeof(T), map);
    }
}