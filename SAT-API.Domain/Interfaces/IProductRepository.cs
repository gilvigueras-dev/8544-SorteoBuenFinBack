using SAT_API.Domain.Entities.Products;

namespace SAT_API.Domain.Interfaces.Products;

public interface IProductRepository
{
    public Task<List<ProductResponse>> GetProducts(ProductRequest request);
    Task<string> SetProductStatus(ProductStatusRequest request);
    Task<List<ControlNumbersResponse>> ControlNumbersList(ControlNumbersRequest request);
    Task<ControlNumbersUrlResponse?> ControlNumbersUrl(ControlNumbersRequest request);
    Task<List<ControlNumbersFileResponse>> ControlNumbersFile(ControlNumbersRequest request);
}
