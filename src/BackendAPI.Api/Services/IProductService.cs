using BackendAPI.Api.DTOs;

namespace BackendAPI.Api.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse?> GetProductByIdAsync(Guid id);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<ProductResponse?> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(Guid id);
}
