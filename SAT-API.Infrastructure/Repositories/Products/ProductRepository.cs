using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Interfaces.Products;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories.Products;

public class ProductRepository : IProductRepository
{
    private readonly IDbContext _context;

    public ProductRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductResponse>> GetProducts(ProductRequest request)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_productos(@p_id_ejecucion, @p_id_etapa)";

        using var connection = _context.CreateConnection();

        var result = await connection.GetAsync<ProductResponse, ProductRequest>(query, request);

        if (result == null || !result.Any())
        {
            return new List<ProductResponse>();
        }
        return result.ToList();
    }

    public async Task<string> SetProductStatus(ProductStatusRequest request)
    {

        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_valida_producto(@p_id_ejecucion, @p_id_archivo)";

        using var connection = _context.CreateConnection();

        var result = await connection.GetFirstOrDefaultAsync<string, ProductStatusRequest>(query, request);

        if (result == null)
        {
            return string.Empty;
        }
        return result;
    }

    public async Task<List<ControlNumbersResponse>> ControlNumbersList(ControlNumbersRequest request)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_cifras_control(@p_id_ejecucion, @p_id_etapa)";

        using var connection = _context.CreateConnection();
        var result = await connection.GetAsync<ControlNumbersResponse, ControlNumbersRequest>(query, request);

        return result.ToList();
    }

    public async Task<ControlNumbersUrlResponse?> ControlNumbersUrl(ControlNumbersRequest request)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_url_cifras_control(@p_id_ejecucion, @p_id_etapa)";

        using var connection = _context.CreateConnection();
        var result = await connection.GetFirstOrDefaultAsync<ControlNumbersUrlResponse, ControlNumbersRequest>(query, request);

        return result;
    }

    public async Task<List<ControlNumbersFileResponse>> ControlNumbersFile(ControlNumbersRequest request)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_cifras_control_archivo(@p_id_ejecucion, @p_id_etapa, @p_id_archivo)";

        using var connection = _context.CreateConnection();
        var result = await connection.GetAsync<ControlNumbersFileResponse, ControlNumbersRequest>(query, request);

        return result.ToList();
    }
}
