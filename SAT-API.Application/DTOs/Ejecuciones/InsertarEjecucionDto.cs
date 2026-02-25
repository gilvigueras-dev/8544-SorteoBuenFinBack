

namespace SAT_API.Application.DTOs.Ejecuciones;

/// <summary>
/// Modelo de respuesta que contiene la información del registro de ejecución insertado
/// </summary>
public class InsertarEjecucionDto
{
    /// <summary>
    /// Identificador único del nuevo registro de ejecución
    /// </summary>
    /// <example>456</example>
    public int NuevoId { get; set; }
}
