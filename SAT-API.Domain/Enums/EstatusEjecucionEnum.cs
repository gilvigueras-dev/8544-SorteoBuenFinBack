using System.ComponentModel;

namespace SAT_API.Domain.Enums;

/// <summary>
/// Enumeración que define los diferentes estatus de ejecución por etapas
/// </summary>
public enum EstatusEjecucion
{
    /// <summary>Etapa 1: Nuevo registro creado</summary>
    [Description("Nuevo registro creado - Etapa 1")]
    Etapa1_NuevoCreado = 1,

    /// <summary>Etapa 1: Validación de insumos en proceso</summary>
    [Description("Validación de insumos - Etapa 1")]
    Etapa1_ValidacionInsumos = 2,

    /// <summary>Etapa 1: Obtener estatus de los insumos validados</summary>
    [Description("Obtener estatus de los insumos - Etapa 1")]
    Etapa1_ObtenerEstatusValidacion = 3,

    /// <summary>Etapa 1: Parámetros guardados en el sistema</summary>
    [Description("Parámetros guardados - Etapa 1")]
    Etapa1_ParamerosGuardados = 4,

    /// <summary>Etapa 1: Ejecutando proceso de cruce</summary>
    [Description("Ejecución de cruce - Etapa 1")]
    Etapa1_EjecucionCruce = 5,

    /// <summary>Etapa 1: Productos validados exitosamente</summary>
    [Description("Productos validados - Etapa 1")]
    Etapa1_ProductosValidados = 6,

    /// <summary>Etapa 1: Tablas pobladas con datos</summary>
    [Description("Tablas pobladas - Etapa 1")]
    Etapa1_TablasPobladas = 7,

    /// <summary>Etapa 1: Etapa 1 completada exitosamente</summary>
    [Description("Etapa 1 finalizada - Etapa 1")]
    Etapa1_Etapa1Finalizada = 8,

    /// <summary>Etapa 2: Validación de insumos en proceso</summary>
    [Description("Validación de insumos - Etapa 2")]
    Etapa2_ValidacionInsumos = 9,

    /// <summary>Etapa 3: Validación de insumos en proceso</summary>
    [Description("Validación de insumos - Etapa 3")]
    Etapa3_ValidacionInsumos = 100,

    /// <summary>Etapa 4: Validación de insumos en proceso</summary>
    [Description("Validación de insumos - Etapa 4")]
    Etapa4_ValidacionInsumos = 25,

    /// <summary>Etapa 2: Insumos validados correctamente</summary>
    [Description("Insumos validados - Etapa 2")]
    Etapa2_InsumosValidados = 10,

    /// <summary>Etapa 2: Parámetros guardados en el sistema</summary>
    [Description("Parámetros guardados - Etapa 2")]
    Etapa2_ParamerosGuardados = 11,

    /// <summary>Etapa 2: Ejecutando proceso de cruce</summary>
    [Description("Ejecución de cruce - Etapa 2")]
    Etapa2_EjecucionCruce = 12,

    /// <summary>Etapa 2: Productos validados exitosamente</summary>
    [Description("Productos validados - Etapa 2")]
    Etapa2_ProductosValidados = 13,

    /// <summary>Etapa 2: Tablas pobladas con datos</summary>
    [Description("Tablas pobladas - Etapa 2")]
    Etapa2_TablasPobladas = 14,

    /// <summary>Etapa 2: Etapa 2 completada exitosamente</summary>
    [Description("Etapa 2 finalizada")]
    Etapa2_Etapa2Finalizada = 15,

    /// <summary>Etapa 3: Ejecutando proceso de cruce</summary>
    [Description("Ejecución de cruce - Etapa 3")]
    Etapa3_EjecucionCruce = 16,

    /// <summary>Etapa 3: Productos validados exitosamente</summary>
    [Description("Productos validados - Etapa 3")]
    Etapa3_ProductosValidados = 17,

    /// <summary>Etapa 3: Tablas pobladas con datos</summary>
    [Description("Tablas pobladas - Etapa 3")]
    Etapa3_TablasPobladas = 18,

    /// <summary>Etapa 3: Etapa 3 completada exitosamente</summary>
    [Description("Etapa 3 finalizada")]
    Etapa3_Etapa3Finalizada = 19,

    /// <summary>Etapa 4: Insumos validados correctamente</summary>
    [Description("Insumos validados - Etapa 4")]
    Etapa4_InsumosValidados = 21,

    /// <summary>Etapa 4: Ejecutando proceso de cruce</summary>
    [Description("Ejecución de cruce - Etapa 4")]
    Etapa4_EjecucionCruce = 22,

    /// <summary>Etapa 4: Productos validados exitosamente</summary>
    [Description("Productos validados - Etapa 4")]
    Etapa4_ProductosValidados = 23,

    /// <summary>Etapa 4: Etapa 4 completada exitosamente</summary>
    [Description("Etapa 4 finalizada")]
    Etapa4_Etapa4Finalizada = 24,

    /// <summary>Etapa 4: Etapa 4 completada exitosamente</summary>
    [Description("Cancelado")]
    Cancelado = 31
}
