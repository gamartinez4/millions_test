using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Million.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<PropertyResponse> CreatePropertyAsync(PropertyRequest propertyRequest)
        {
            var property = new Property(
                propertyRequest.Name,
                propertyRequest.Address,
                propertyRequest.Price,
                propertyRequest.Year,
                propertyRequest.OwnerId
            );

            await _propertyRepository.AddAsync(property);

            return new PropertyResponse
            {
                Id = property.Id,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                Year = property.Year,
  
            };
        }

        public async Task<PropertyResponse> GetPropertyByIdAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                return null;
            }

            return new PropertyResponse
            {
                Id = property.Id,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                Year = property.Year,
            };
        }

        public async Task<IEnumerable<PropertyResponse>> GetPropertiesFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            var properties = await _propertyRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(name))
            {
                properties = properties.Where(p => p.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(address))
            {
                properties = properties.Where(p => p.Address.Contains(address));
            }
            if (minPrice.HasValue)
            {
                properties = properties.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                properties = properties.Where(p => p.Price <= maxPrice.Value);
            }

            return properties.Select(p => new PropertyResponse
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                Year = p.Year
            });
        }

        public async Task UpdatePropertyAsync(int id, PropertyRequest propertyRequest)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property != null)
            {
                property.Name = propertyRequest.Name;
                property.Address = propertyRequest.Address;
                property.Price = propertyRequest.Price;
                property.Year = propertyRequest.Year;
                property.OwnerId = propertyRequest.OwnerId;
                _propertyRepository.Update(property);
            }
        }

      
    }
} 