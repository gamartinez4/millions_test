using Million.Application.Contracts;
using Million.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Million.Infrastructure.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly List<PropertyImage> _propertyImages = new List<PropertyImage>();
        private int _nextId = 1;

        public PropertyImageRepository()
        {
            _propertyImages.Add(new PropertyImage(1, "https://example.com/image1.jpg", true) { Id = _nextId++ });
            _propertyImages.Add(new PropertyImage(1, "https://example.com/image2.jpg", true) { Id = _nextId++ });
        }

        public Task<PropertyImage> GetByIdAsync(int id)
        {
            return Task.FromResult(_propertyImages.FirstOrDefault(pi => pi.Id == id));
        }

        public Task<IEnumerable<PropertyImage>> GetAllAsync()
        {
            return Task.FromResult(_propertyImages.AsEnumerable());
        }

        public Task<IEnumerable<PropertyImage>> FindAsync(Expression<Func<PropertyImage, bool>> expression)
        {
            return Task.FromResult(_propertyImages.AsQueryable().Where(expression).AsEnumerable());
        }

        public Task AddAsync(PropertyImage entity)
        {
            entity.Id = _nextId++;
            _propertyImages.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(PropertyImage entity)
        {
            var existingImage = _propertyImages.FirstOrDefault(pi => pi.Id == entity.Id);
            if (existingImage != null)
            {
                existingImage.PropertyId = entity.PropertyId;
                existingImage.File = entity.File;
                existingImage.Enabled = entity.Enabled;
            }
        }

        public void Remove(PropertyImage entity)
        {
            _propertyImages.Remove(entity);
        }
    }
} 