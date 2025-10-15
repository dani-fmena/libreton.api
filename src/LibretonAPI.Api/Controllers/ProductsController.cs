using LibretonAPI.Api.DTOs;
using LibretonAPI.Api.Services;
using LibretonAPI.Api.Validators;
using LibretonAPI.Shared.Constants;
using LibretonAPI.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibretonAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ProductValidator _productValidator;

    public ProductsController(IProductService productService, ProductValidator productValidator)
    {
        _productService = productService;
        _productValidator = productValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(products));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductResponse>.ErrorResponse(
                AppConstants.ErrorMessages.ResourceNotFound));
        }

        return Ok(ApiResponse<ProductResponse>.SuccessResponse(product));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Create([FromBody] CreateProductRequest request)
    {
        var validationErrors = _productValidator.ValidateCreateProduct(request);
        if (validationErrors.Any())
        {
            return BadRequest(ApiResponse<ProductResponse>.ErrorResponse(
                AppConstants.ErrorMessages.ValidationFailed, 
                validationErrors));
        }

        var product = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, 
            ApiResponse<ProductResponse>.SuccessResponse(product, "Product created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var validationErrors = _productValidator.ValidateUpdateProduct(request);
        if (validationErrors.Any())
        {
            return BadRequest(ApiResponse<ProductResponse>.ErrorResponse(
                AppConstants.ErrorMessages.ValidationFailed, 
                validationErrors));
        }

        var product = await _productService.UpdateProductAsync(id, request);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductResponse>.ErrorResponse(
                AppConstants.ErrorMessages.ResourceNotFound));
        }

        return Ok(ApiResponse<ProductResponse>.SuccessResponse(product, "Product updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse(
                AppConstants.ErrorMessages.ResourceNotFound));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully"));
    }
}
