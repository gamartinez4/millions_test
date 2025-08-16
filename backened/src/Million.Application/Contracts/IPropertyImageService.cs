using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Application.Contracts
{
    public interface IPropertyImageService
    {
        Task<IEnumerable<PropertyImageResponse>> GetAllImagesAsync();
        Task<IEnumerable<PropertyImageResponse>> GetImagesForPropertyAsync(int propertyId);
        Task<PropertyImageResponse> GetImageByIdAsync(int id);
        Task<PropertyImageResponse> AddImageToPropertyAsync(PropertyImageRequest imageRequest);
        Task UpdateImageAsync(int id, PropertyImageRequest imageRequest);
        Task DeleteImageAsync(int id);
    }
} 