using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyResponse>> GetAllPropertiesAsync();
        Task<PropertyResponse> GetPropertyByIdAsync(int id);
        Task<PropertyResponse> CreatePropertyAsync(PropertyRequest propertyRequest);
        Task UpdatePropertyAsync(int id, PropertyRequest propertyRequest);
        Task UpdatePropertyForSaleAsync(int id, UpdatePropertyForSaleRequest request);
        Task UpdatePropertyOwnerAsync(int id, UpdatePropertyOwnerRequest request);
        Task UpdatePropertyPriceAsync(int id, UpdatePropertyPriceRequest request);
    }
}
