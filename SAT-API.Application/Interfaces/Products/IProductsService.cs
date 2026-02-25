using SAT_API.Application.Common;
using SAT_API.Application.DTOs.Products;

namespace SAT_API.Application.Interfaces.Products;

public interface IProductsService
{
    Task<Result<List<ProductResponseDto>>> GetProducts(ProductRequestDto request);
    Task<Result<string>> SetProductStatus(ProductStatusRequestDto request);
    Task<Result<List<ControlNumbersResponseDto>>> ControlNumberList(ControlNumbersRequestDto request);
    Task<Result<ControlNumbersUrlResponseDto?>> ControlNumbersUrl(ControlNumbersRequestDto request);
    Task<Result<MoveFileResponseDto>> GetMoverArchivo(int ejecucionId, int numeroEtapa, int productoId);
   Task<Result<List<ControlNumbersFileResponseDto>>> ControlNumbersFile(ControlNumbersRequestDto request);
}
