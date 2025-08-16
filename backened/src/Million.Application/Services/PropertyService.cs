using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System;
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
                propertyRequest.OwnerId,
                propertyRequest.ForSale ?? (propertyRequest.OwnerId == null)
            );

            await _propertyRepository.AddAsync(property);

            return new PropertyResponse
            {
                Id = property.Id,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                Year = property.Year,
                OwnerId = property.OwnerId.ToString(),
                ForSale = property.ForSale
  
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
                OwnerId = property.OwnerId.ToString(),
                ForSale = property.ForSale
            };
        }

        public async Task<IEnumerable<PropertyResponse>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();
            return properties.Select(p => new PropertyResponse
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                Year = p.Year,
                OwnerId = p.OwnerId.ToString(),
                ForSale = p.ForSale
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
                property.ForSale = propertyRequest.ForSale ?? (propertyRequest.OwnerId == null);
                _propertyRepository.Update(property);
            }
        }

        public async Task UpdatePropertyForSaleAsync(int id, UpdatePropertyForSaleRequest request)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                throw new ArgumentException($"Property with ID {id} not found.");
            }

            property.ForSale = request.ForSale;
            _propertyRepository.Update(property);
        }

        public async Task UpdatePropertyOwnerAsync(int id, UpdatePropertyOwnerRequest request)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                throw new ArgumentException($"Property with ID {id} not found.");
            }

            Console.WriteLine($"[PropertyService] Before update - Property ID: {property.Id}, Current OwnerId: {property.OwnerId}");
            Console.WriteLine($"[PropertyService] Request OwnerId: {request.OwnerId}");
            
            property.OwnerId = request.OwnerId;
            
            Console.WriteLine($"[PropertyService] After assignment - Property OwnerId: {property.OwnerId}");
            
            _propertyRepository.Update(property);
            
            Console.WriteLine($"[PropertyService] Update called for property {property.Id}");
        }

        public async Task UpdatePropertyPriceAsync(int id, UpdatePropertyPriceRequest request)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                throw new ArgumentException($"Property with ID {id} not found.");
            }

            Console.WriteLine($"[PropertyService] Before price update - Property ID: {property.Id}, Current Price: {property.Price}");
            Console.WriteLine($"[PropertyService] Request Price: {request.Price}");
            
            property.Price = request.Price;
            
            Console.WriteLine($"[PropertyService] After assignment - Property Price: {property.Price}");
            
            _propertyRepository.Update(property);
            
            Console.WriteLine($"[PropertyService] Update called for property {property.Id}");
        }

      
    }
} 