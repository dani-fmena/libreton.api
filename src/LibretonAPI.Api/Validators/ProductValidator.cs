using LibretonAPI.Api.DTOs;
using LibretonAPI.Shared.Constants;

namespace LibretonAPI.Api.Validators;

public class ProductValidator
{
    public List<string> ValidateCreateProduct(CreateProductRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Name"));
        else if (request.Name.Length > 200)
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Name", 200));

        if (request.Price <= 0)
            errors.Add("Price must be greater than zero.");

        if (request.Stock < 0)
            errors.Add("Stock cannot be negative.");

        if (request.Description?.Length > 1000)
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Description", 1000));

        if (request.Category?.Length > 100)
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Category", 100));

        return errors;
    }

    public List<string> ValidateUpdateProduct(UpdateProductRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Name"));
        else if (request.Name.Length > 200)
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Name", 200));

        if (request.Price <= 0)
            errors.Add("Price must be greater than zero.");

        if (request.Stock < 0)
            errors.Add("Stock cannot be negative.");

        if (request.Description?.Length > 1000)
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Description", 1000));

        if (request.Category?.Length > 100)
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Category", 100));

        return errors;
    }
}
