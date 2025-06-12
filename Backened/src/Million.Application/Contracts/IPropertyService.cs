using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyResponse>> GetPropertiesFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice);
        Task<PropertyResponse> GetPropertyByIdAsync(int id);
        Task<PropertyResponse> CreatePropertyAsync(PropertyRequest propertyRequest);
        Task UpdatePropertyAsync(int id, PropertyRequest propertyRequest);
    }
} 