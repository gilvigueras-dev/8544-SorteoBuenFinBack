namespace SAT_API.Infrastructure.Data;

/// <summary>
/// Define los esquemas de base de datos utilizados en la aplicación.
/// </summary>
public static class ConstantsSchemas
{
    public static readonly string dec08544_dd_sac_svsbf =
        Environment.GetEnvironmentVariable("SBF-ENV-DB-SCHEMA") ?? "dec08544_dd_sac_svsbf";

}