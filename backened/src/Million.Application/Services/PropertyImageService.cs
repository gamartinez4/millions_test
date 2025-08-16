using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Million.Application.Services
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IPropertyImageRepository _imageRepository;

        public PropertyImageService(IPropertyImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<PropertyImageResponse> AddImageToPropertyAsync(PropertyImageRequest imageRequest)
        {
            var propertyImage = new PropertyImage(
                imageRequest.PropertyId,
                imageRequest.FileUrl,
                imageRequest.Enabled
            );

            await _imageRepository.AddAsync(propertyImage);

            return MapToImageResponse(propertyImage);
        }

        public async Task DeleteImageAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image != null)
            {
                _imageRepository.Remove(image);
            }
        }

        public async Task<PropertyImageResponse> GetImageByIdAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            return image == null ? null : MapToImageResponse(image);
        }

        public async Task<IEnumerable<PropertyImageResponse>> GetAllImagesAsync()
        {
            var images = await _imageRepository.GetAllAsync();
            return images.Select(MapToImageResponse);
        }

        public async Task<IEnumerable<PropertyImageResponse>> GetImagesForPropertyAsync(int propertyId)
        {
            var images = await _imageRepository.FindAsync(pi => pi.PropertyId == propertyId);
            return images.Select(MapToImageResponse);
        }

        public async Task UpdateImageAsync(int id, PropertyImageRequest imageRequest)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image != null)
            {
                image.File = imageRequest.FileUrl;
                image.Enabled = imageRequest.Enabled;
                image.PropertyId = imageRequest.PropertyId;
                _imageRepository.Update(image);
            }
        }

        private PropertyImageResponse MapToImageResponse(PropertyImage image)
        {
            return new PropertyImageResponse
            {
                Id = image.Id,
                PropertyId = image.PropertyId,
                FileUrl = image.File,
                Enabled = image.Enabled
            };
        }
    }
} 